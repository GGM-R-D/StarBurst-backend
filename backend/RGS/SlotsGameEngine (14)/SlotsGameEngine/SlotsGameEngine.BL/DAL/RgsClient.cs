using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using SlotsGameEngine.DTO.Client.Requests;
using SlotsGameEngine.DTO.Shared.DataObjects;
using SlotsGameEngine.DTO.Engine.Requests;
using SlotsGameEngine.DTO.Engine.Responses;

namespace SlotsGameEngine.BL.DAL
{
    public class RgsClient
    {
        private readonly HttpClient _httpClient;
        private readonly GameEngineClient _engineClient;
        private readonly IConfiguration _config;

        // Web defaults (camelCase etc.) so logs/payload match typical HTTP JSON behavior
        private static readonly JsonSerializerOptions JsonWebOptions =
            new(JsonSerializerDefaults.Web)
            { WriteIndented = false };

        private static void Log(string msg)
        {
            Console.WriteLine(msg);
            System.Diagnostics.Debug.WriteLine(msg);
        }

        public RgsClient(HttpClient httpClient, GameEngineClient engineClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _engineClient = engineClient;
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<BetResult> PlayAsync(
            long operatorId,
            string gameId,
            TokenData token,
            PlayRequest request,
            string roundId,
            CancellationToken ct)
        {
            var totalBet = request.Bets?.Sum(b => b.Amount) ?? 0m;

            // 1) Decide engine URL
            var engineBaseUrl = GetEngineBaseUrlForGame(gameId);
            if (!string.IsNullOrWhiteSpace(engineBaseUrl))
                _engineClient.OverrideBaseAddress(engineBaseUrl);

            Log("[ENGINE BASE URL] " + (engineBaseUrl ?? "(default)"));
            Log("[API INCOMING userPayload] " + (request.UserPayload.HasValue ? request.UserPayload.Value.GetRawText() : "null"));

            // 2) Build bet lines for engine
            var betsForEngine = (request.Bets ?? new List<SlotsGameEngine.DTO.Client.Requests.BetLine>())
                .Select(b => new SlotsGameEngine.DTO.Engine.Requests.BetLine
                {
                    Amount = b.Amount
                })
                .ToList();

            // 3) Forward userPayload AS-IS to engine (still do this)
            object? forwardedUserPayload = null;
            if (request.UserPayload.HasValue)
            {
                try
                {
                    forwardedUserPayload = JsonSerializer.Deserialize<object>(
                        request.UserPayload.Value.GetRawText(),
                        JsonWebOptions
                    );
                }
                catch (Exception ex)
                {
                    Log("[WARN] Failed to deserialize userPayload. Sending null. Error: " + ex.Message);
                    forwardedUserPayload = null;
                }
            }

            var engineReq = new EnginePlayRequest
            {
                GameId = gameId,
                PlayerToken =
                    !string.IsNullOrWhiteSpace(token.RawToken) ? token.RawToken :
                    token.PlayerId.ToString(),
                SessionId = request.SessionId,
                Bets = betsForEngine,
                Bet = totalBet,
                TotalBet = totalBet,
                UserPayload = forwardedUserPayload,
                Currency = new Currency { Id = token.Currency ?? "USD" },
                Mode = 0,
                RtpLevel = 1,
                LastResponse = null
            };

            // Starburst special-case (leave as-is)
            if (gameId.Equals("starburst", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var bet in engineReq.Bets)
                    bet.BetType = "Base";
            }

            Log("[ENGINE PLAY REQUEST OBJ] " + JsonSerializer.Serialize(engineReq, JsonWebOptions));

            // 4) Call engine
            var engineResp = await _engineClient.PlayAsync(engineReq, ct);

            Log("[ENGINE PLAY RESPONSE] " + JsonSerializer.Serialize(engineResp, JsonWebOptions));

            // 5) PATCH: force tumbleResult.stops in the API response from Swagger userPayload (NOT from engine)
            // Supports both "cheat" and "cheats"
            object? patchedResultsObj = engineResp.Results;

            if (gameId.Equals("starburst", StringComparison.OrdinalIgnoreCase) &&
                engineResp.Results is JsonElement jsonElement &&
                jsonElement.ValueKind == JsonValueKind.Object)
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, object?>>(
                    jsonElement.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (dict != null)
                {
                    // Normalize FinalGridSymbols → finalGridSymbols
                    if (dict.TryGetValue("FinalGridSymbols", out var finalGrid))
                        dict["finalGridSymbols"] = finalGrid;

                    // Normalize Stops → stops
                    if (dict.TryGetValue("Stops", out var stops))
                        dict["stops"] = stops;

                    // Normalize Wins → wins
                    if (dict.TryGetValue("Wins", out var wins))
                        dict["wins"] = wins;

                    patchedResultsObj = dict;
                }
            }

            if (request.UserPayload.HasValue &&
                TryExtractCheatFromUserPayload(request.UserPayload.Value, out var forcedStops, out var forcedReelSetId))
            {
                patchedResultsObj = ApplyForcedStopsToResults(patchedResultsObj, forcedStops, forcedReelSetId);

                Log("[CHEAT PATCH APPLIED] reelSetId=" + (forcedReelSetId ?? "(unchanged)") +
                    " stops=[" + string.Join(",", forcedStops) + "]");
            }

            if (gameId.Equals("starburst", StringComparison.OrdinalIgnoreCase))
            {
                if (patchedResultsObj is not Dictionary<string, object?> resultsDict ||
                    !resultsDict.ContainsKey("finalGridSymbols"))
                {
                    return new BetResult
                    {
                        RoundId = roundId,
                        BetAmount = totalBet,
                        WinAmount = 0m,
                        Payload = new Dictionary<string, object?>
                        {
                            ["statusCode"] = 5001,
                            ["message"] = "Engine response missing finalGridSymbols",
                            ["win"] = 0m,
                            ["freeSpins"] = 0,
                            ["results"] = patchedResultsObj
                        }
                    };
                }
            }

            // 6) Wrap payload for API response
            var payload = new Dictionary<string, object?>
            {
                ["statusCode"] = engineResp.StatusCode,
                ["message"] = engineResp.Message,
                ["win"] = engineResp.Win,
                ["freeSpins"] = engineResp.FreeSpins,
                ["results"] = patchedResultsObj // <-- THIS is what Swagger sees
            };

            return new BetResult
            {
                RoundId = roundId,
                BetAmount = totalBet,
                WinAmount = engineResp.Win,
                Payload = payload
            };
        }

        /// <summary>
        /// Reads Swagger userPayload and extracts forced stops.
        /// Accepts:
        /// - userPayload.cheat[0].stops
        /// - userPayload.cheats[0].stops
        ///
        /// If stops has 7 numbers and the first is 1 or 861:
        /// - 1  => REEL_SET_LOW
        /// - 861 => REEL_SET_HIGH
        /// then we treat the remaining 6 as the actual tumbleResult.stops.
        /// Otherwise, we use all numbers as-is.
        /// </summary>
        private static bool TryExtractCheatFromUserPayload(
            JsonElement userPayload,
            out int[] forcedStops,
            out string? forcedReelSetId)
        {
            forcedStops = Array.Empty<int>();
            forcedReelSetId = null;

            try
            {
                if (userPayload.ValueKind != JsonValueKind.Object)
                    return false;

                // debugEnabled is optional for this patch, but you can enforce it if you want:
                // if (!userPayload.TryGetProperty("debugEnabled", out var dbg) || dbg.ValueKind != JsonValueKind.True)
                //     return false;

                JsonElement cheatArray;

                // support both spellings
                if (userPayload.TryGetProperty("cheat", out cheatArray) == false &&
                    userPayload.TryGetProperty("cheats", out cheatArray) == false)
                {
                    return false;
                }

                if (cheatArray.ValueKind != JsonValueKind.Array || cheatArray.GetArrayLength() == 0)
                    return false;

                var firstCheat = cheatArray[0];
                if (firstCheat.ValueKind != JsonValueKind.Object)
                    return false;

                if (!firstCheat.TryGetProperty("stops", out var stopsEl) || stopsEl.ValueKind != JsonValueKind.Array)
                    return false;

                var stops = new List<int>();
                foreach (var s in stopsEl.EnumerateArray())
                {
                    if (s.ValueKind == JsonValueKind.Number && s.TryGetInt32(out var v))
                        stops.Add(v);
                }

                if (stops.Count == 0)
                    return false;

                // If they send 7 values where first indicates reel set (1 low, 861 high)
                if (stops.Count == 7 && (stops[0] == 1 || stops[0] == 861))
                {
                    forcedReelSetId = (stops[0] == 1) ? "REEL_SET_LOW" : "REEL_SET_HIGH";
                    forcedStops = stops.Skip(1).ToArray(); // remaining 6 are the actual tumble stops
                    return true;
                }

                // Otherwise, just force exactly what they provided
                forcedStops = stops.ToArray();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Applies forced stops (and optional reelSetId) into the engine "results" object
        /// at results.tumbleResult.stops (and results.tumbleResult.reelSetId).
        /// </summary>
        private static object? ApplyForcedStopsToResults(object? engineResultsObj, int[] forcedStops, string? forcedReelSetId)
        {
            if (engineResultsObj == null)
                return null;

            // Convert the typed object to JSON tree, patch it, then convert back to object
            var json = JsonSerializer.Serialize(engineResultsObj, JsonWebOptions);
            var node = JsonNode.Parse(json);

            if (node is not JsonObject rootObj)
                return engineResultsObj;

            // In your engine results, tumbleResult is usually at root.tumbleResult
            var tumbleObj = rootObj["tumbleResult"] as JsonObject;

            // Fallback: some engines might nest it differently
            if (tumbleObj == null && rootObj["results"] is JsonObject inner && inner["tumbleResult"] is JsonObject innerTumble)
                tumbleObj = innerTumble;

            if (tumbleObj == null)
                return engineResultsObj; // can't patch if we can't find tumbleResult

            // Patch reelSetId if provided
            if (!string.IsNullOrWhiteSpace(forcedReelSetId))
                tumbleObj["reelSetId"] = forcedReelSetId;

            // Patch stops
            var arr = new JsonArray();
            foreach (var s in forcedStops)
                arr.Add(s);

            tumbleObj["stops"] = arr;

            // Convert patched JSON back to an object so ASP.NET returns it as JSON properly
            var patchedJson = rootObj.ToJsonString(JsonWebOptions);
            return JsonSerializer.Deserialize<object>(patchedJson, JsonWebOptions);
        }

        private string? GetEngineBaseUrlForGame(string gameId)
        {
            if (string.IsNullOrWhiteSpace(gameId))
                return null;

            // Allow override via config (e.g. GameEngines:starburst = http://localhost:5102 for local backend / cheat)
            var configKey = "GameEngines:" + gameId;
            var fromConfig = _config[configKey];
            if (!string.IsNullOrWhiteSpace(fromConfig))
                return fromConfig.Trim();

            switch (gameId.ToLowerInvariant())
            {
                case "goldcrush":
                    return "http://staginggoldcrushgameengine.gameonstudios.bet:30090";

                case "starburst":
                    return "http://203.123.83.157:30201";

                default:
                    return null;
            }
        }
    }
}
