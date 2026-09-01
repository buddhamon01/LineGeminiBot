var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddHttpClient<LineService>();

var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => "LINE Gemini Bot is running!");

app.Run();