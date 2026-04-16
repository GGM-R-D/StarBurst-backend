using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using SlotsGameEngine.DTO.Engine.Requests;
using SlotsGameEngine.DTO.Engine.Responses;

namespace SlotsGameEngine.BL.DAL
{
    public class GameEngineClient
    {
        private readonly HttpClient _http;

        public GameEngineClient(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Override the base URL for the next calls (e.g. per gameId).
        /// Examples:
        ///  - "http://staginggoldcrushgameengine.gameonstudios.bet:30090"
        ///  - "http://203.123.83.157:30201"  (starburst)
        /// </summary>
        public void OverrideBaseAddress(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be empty.", nameof(baseUrl));

            _http.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        }

        public async Task<EnginePlayResponse> PlayAsync(EnginePlayRequest request, CancellationToken ct)
        {
            // Calls {BaseAddress}/play
            var response = await _http.PostAsJsonAsync("play", request, ct);

            // Read raw content even if it's 4xx/5xx
            var raw = await response.Content.ReadAsStringAsync(ct);

            Console.WriteLine($"[ENGINE STATUS] {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine("[ENGINE RAW BODY] " + raw);

            if (!response.IsSuccessStatusCode)
            {
                // Bubble up a clear error including the engine response
                throw new InvalidOperationException(
                    $"Engine /play returned {(int)response.StatusCode} ({response.StatusCode}): {raw}");
            }

            var body = JsonSerializer.Deserialize<EnginePlayResponse>(
                raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (body == null)
                throw new InvalidOperationException("GameEngine /play returned empty or invalid body.");

            return body;
        }
    }
}