using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SlotsGameEngine.BL.BLL;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.BL.DAL;
using SlotsGameEngine.BL.Helpers;
using System;

namespace SlotsGameEngine.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSlotsGameEngineServices(this IServiceCollection services, IConfiguration config)
        {
            // Http client for RGS (wallet/WRS side)
            services.AddHttpClient<RgsClient>();

            // Http client for operator wallet
            services.AddHttpClient<OperatorWalletClient>();

            // 🔹 Game engine HTTP client (math server)
            services.AddHttpClient<GameEngineClient>(client =>
            {
                // This MUST be configured, otherwise we want the app to fail fast
                var baseUrl = config["GameEngine:BaseUrl"];   // e.g. http://staginggoldcrushgameengine.gameonstudios.bet:30090

                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    throw new InvalidOperationException("GameEngine:BaseUrl is not configured. Please set it in appsettings.json.");
                }

                // Result: http://staginggoldcrushgameengine.gameonstudios.bet:30090/
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            });

            // Helpers
            services.AddSingleton<TokenParser>();
            services.AddSingleton<RngNumberGenerator>();

            // Optional DB helper + logs
            var connStr = config.GetConnectionString("DefaultConnection") ?? string.Empty;
            services.AddSingleton(new DBHelper(connStr));
            services.AddSingleton<ApiLogRepository>();

            // Core services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoundService, RoundService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IBetService, BetService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IRngService, RngService>();
            services.AddScoped<ILoggingService, LoggingService>();

            return services;
        }
    }
}