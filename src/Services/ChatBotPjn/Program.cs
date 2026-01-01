using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
})
.ConfigureHttpClient((sp, client) =>
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.MapGet("/", () =>
    Results.Ok(new { ok = true, service = "MiniApi", ts = DateTime.UtcNow })
);

app.MapGet("/health", () => Results.Ok("healthy"));

// --- DEMO CRUD ---
var todos = new List<Todo>();

app.MapGet("/todos", () => Results.Ok(todos));

app.MapGet("/todos/{id:int}", (int id) =>
    todos.FirstOrDefault(t => t.Id == id) is { } found
        ? Results.Ok(found)
        : Results.NotFound()
);

app.MapPost("/todos", (Todo dto) =>
{
    dto.Id = todos.Any() ? todos.Max(t => t.Id) + 1 : 1;
    todos.Add(dto);
    return Results.Created($"/todos/{dto.Id}", dto);
});

app.MapPut("/todos/{id:int}", (int id, Todo dto) =>
{
    var idx = todos.FindIndex(t => t.Id == id);
    if (idx < 0) return Results.NotFound();

    dto.Id = id;
    todos[idx] = dto;

    return Results.NoContent();
});

app.MapDelete("/todos/{id:int}", (int id) =>
{
    var removed = todos.RemoveAll(t => t.Id == id) > 0;
    return removed ? Results.NoContent() : Results.NotFound();
});

// --- /ai/chat ---
app.MapPost("/ai/chat", async (ChatRequest req, IHttpClientFactory cf, CancellationToken ct) =>
{
    var model =
        req.Model ??
        Environment.GetEnvironmentVariable("OPENAI_MODEL") ??
        "gpt-4o-mini";

    var payload = new
    {
        model,
        messages = req.Messages.Select(m => new
        {
            role = m.Role,
            content = m.Content
        }),
        temperature = req.Temperature ?? 0.2f,
        max_tokens = req.MaxTokens ?? 800
    };

    var http = cf.CreateClient("openai");

    using var content =
        new StringContent(JsonSerializer.Serialize(payload),
                          Encoding.UTF8,
                          "application/json");

    var resp = await http.PostAsync("v1/chat/completions", content, ct);
    var json = await resp.Content.ReadAsStringAsync(ct);

    if (!resp.IsSuccessStatusCode)
        return Results.Problem(
            title: "OpenAI error",
            detail: json,
            statusCode: (int)resp.StatusCode
        );

    using var doc = JsonDocument.Parse(json);

    string text =
        doc.RootElement
           .GetProperty("choices")[0]
           .GetProperty("message")
           .GetProperty("content")
           .GetString() ?? "";

    var rawClone = doc.RootElement.Clone();

    Console.WriteLine(rawClone.ToString());

    return Results.Ok(new ChatResponse(text, rawClone));
});

// --- /ai/stream ---
app.MapPost("/ai/stream", async (ChatRequest req, IHttpClientFactory cf, HttpResponse response, CancellationToken ct) =>
{
    var model =
        req.Model ??
        Environment.GetEnvironmentVariable("OPENAI_MODEL") ??
        "gpt-4o-mini";

    var payload = new
    {
        model,
        messages = req.Messages.Select(m => new
        {
            role = m.Role,
            content = m.Content
        }),
        temperature = req.Temperature ?? 0.2f,
        max_tokens = req.MaxTokens ?? 800,
        stream = true
    };

    var http = cf.CreateClient("openai");

    using var request = new HttpRequestMessage(
        HttpMethod.Post,
        "v1/chat/completions")
    {
        Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json")
    };

    var resp = await http.SendAsync(
        request,
        HttpCompletionOption.ResponseHeadersRead,
        ct
    );

    if (!resp.IsSuccessStatusCode)
    {
        response.StatusCode = (int)resp.StatusCode;
        await response.WriteAsync(await resp.Content.ReadAsStringAsync(ct), ct);
        return;
    }

    response.Headers.ContentType = "text/event-stream";
    response.Headers.CacheControl = "no-cache";
    response.Headers.Connection = "keep-alive";

    await using var stream = await resp.Content.ReadAsStreamAsync(ct);
    using var reader = new StreamReader(stream);

    while (!reader.EndOfStream && !ct.IsCancellationRequested)
    {
        var line = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(line)) continue;

        await response.WriteAsync(line + "\n", ct);
        await response.Body.FlushAsync(ct);

        if (line.Contains("[DONE]")) break;
    }
});

// --- /ai/ping ---
app.MapGet("/ai/ping", () =>
{
    var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    var model =
        Environment.GetEnvironmentVariable("OPENAI_MODEL")
        ?? "(default:gpt-4o-mini)";

    return Results.Ok(new
    {
        hasKey = !string.IsNullOrWhiteSpace(key),
        model
    });
});

app.Run();
