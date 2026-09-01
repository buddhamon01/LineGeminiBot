using System.Net.Http.Headers;
using System.Net.Http.Json;

public class LineService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public LineService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task ReplyAsync(
        string replyToken,
        string message)
    {
        var token = _config["Line:ChannelAccessToken"];

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var body = new
        {
            replyToken = replyToken,
            messages = new[]
            {
                new
                {
                    type = "text",
                    text = message
                }
            }
        };

        var response = await _http.PostAsJsonAsync(
            "https://api.line.me/v2/bot/message/reply",
            body);

        response.EnsureSuccessStatusCode();
    }
}