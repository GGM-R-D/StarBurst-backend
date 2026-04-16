using Microsoft.OpenApi.Models;
using SlotsGameEngine.API.Extensions;
using SlotsGameEngine.API.Middleware;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// 1) SERVICES
// ==================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SlotsGameEngine API",
        Version = "v1"
    });

    // Avoid schema name clashes (BetLine, Currency, etc.)
    c.CustomSchemaIds(type =>
        type.FullName?.Replace("+", ".") ?? type.Name);

    // Ensure JsonElement renders correctly in Swagger
    c.MapType<JsonElement>(() => new OpenApiSchema { Type = "object" });
    c.MapType<JsonElement?>(() => new OpenApiSchema { Type = "object", Nullable = true });
});

// -------------------------------
// CORS Strategy (matches your need)
// - If FE URL is unknown: allow any origin (NO credentials)
// - If FE URL is known: allow only that origin (can enable credentials)
// -------------------------------
var corsOriginsRaw = builder.Configuration["AppSettings:CORSOrigin"]; // can be null/empty

var corsOrigins = (corsOriginsRaw ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(o => o.TrimEnd('/'))
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("GlobalCORSPolicy", policy =>
    {
        // If you DON'T have FE URL yet => open CORS temporarily
        if (corsOrigins.Length == 0)
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            // If you DO have FE URL(s) => locked-down CORS
            policy
                .WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                // Only keep AllowCredentials() if you truly need cookies/auth credentials
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition");
        }
    });
});

// DI registrations (DbContexts, services, repos, etc.)
builder.Services.AddSlotsGameEngineServices(builder.Configuration);

Console.WriteLine(corsOrigins.Length == 0
    ? "[CORS] No FE origin configured -> AllowAnyOrigin enabled (TEMP)."
    : "[CORS] Allowed Origins: " + string.Join(" | ", corsOrigins));

var app = builder.Build();

// ==================================================
// 2) MIDDLEWARE PIPELINE (ORDER MATTERS)
// ==================================================

// Helpful in production (optional but good practice)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Swagger (enabled in all environments)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SlotsGameEngine API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

// ✅ Routing must come before CORS
app.UseRouting();

// ✅ CORS must come after routing and before authorization/endpoints
app.UseCors("GlobalCORSPolicy");

// If you add authentication later, it should go here:
// app.UseAuthentication();

app.UseAuthorization();

// Custom request logging middleware (kept from your code)
app.UseRequestLogging();

// Map controllers LAST
app.MapControllers();

app.Run();


