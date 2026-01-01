using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatBotPjn.Models;
using DotNetEnv;  

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();



// HttpClient “OpenAI”
builder.Services.AddHttpClient("openai", client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    client.Timeout = TimeSpan.FromSeconds(120);
}).ConfigureHttpClient((sp, client) =>
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    if (string.IsNullOrWhiteSpace(apiKey))
        throw new InvalidOperationException("OPENAI_API_KEY no configurada.");

    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", apiKey);

    client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}


{
};

{


{
}
});

// --- /ai/ping ---
app.MapGet("/ai/ping", () =>
{
    var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "(default:gpt-4o-mini)";
    return Results.Ok(new { hasKey = !string.IsNullOrWhiteSpace(key), model });
});
app.Run();
