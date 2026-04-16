using System.Text.Json;
using System.Text.Json.Serialization;
using GameEngine;
using GameEngine.Configuration;
using GameEngine.Play;
using GameEngine.Services;
using GameEngineHost.Services;
using Microsoft.AspNetCore.OpenApi;
using RNGClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new MoneyJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TEMPORARY CORS: allow any origin to call the engine (browser-friendly, no credentials)
// NOTE: Do NOT use AllowCredentials with AllowAnyOrigin.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowRGS", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var configDirectory = ResolvePath(builder.Configuration["GameEngine:ConfigurationDirectory"] ?? "configs", builder.Environment);
var manifestPath = ResolvePath(builder.Configuration["GameEngine:ControlProgramManifest"] ?? "control-program-manifest.json", builder.Environment);

builder.Services.AddGameEngine(configDirectory, manifestPath);
builder.Services.AddSingleton<ISpinTelemetrySink, NullSpinTelemetrySink>();
builder.Services.AddSingleton<IEngineClient, LocalEngineClient>();

// Use integrated RNG service with Fortuna PRNG instead of external HTTP service
builder.Services.AddSingleton<IRngClient>(sp =>
{
    var fortunaPrng = sp.GetRequiredService<FortunaPrng>();
    return new IntegratedRngClient(fortunaPrng);
});

var app = builder.Build();

app.UseExceptionHandler();

// Enable CORS early so OPTIONS and API calls succeed
app.UseCors("AllowRGS");

// Health check endpoint for Kubernetes
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithTags("Health")
    .WithName("HealthCheck");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

static string ResolvePath(string path, IWebHostEnvironment environment)
{
    if (Path.IsPathRooted(path))
    {
        return path;
    }

    return Path.GetFullPath(Path.Combine(environment.ContentRootPath, path));
}
