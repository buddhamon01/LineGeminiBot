using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("api/line")]
public class LineController : ControllerBase
{
    private readonly GeminiService _gemini;
    private readonly LineService _line;

    public LineController(
        GeminiService gemini,
        LineService line)
    {
        _gemini = gemini;
        _line = line;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
        [FromBody] JsonElement body)
    {
        if (!body.TryGetProperty(
                "events",
                out var events))
        {
            return Ok();
        }

        foreach (var ev in events.EnumerateArray())
        {
            if (!ev.TryGetProperty(
                    "type",
                    out var type))
                continue;

            if (type.GetString() != "message")
                continue;

            var message = ev.GetProperty("message");

            if (message.GetProperty("type").GetString()
                != "text")
                continue;

            var userText =
                message.GetProperty("text").GetString();

            var replyToken =
                ev.GetProperty("replyToken").GetString();

            if (string.IsNullOrWhiteSpace(userText) ||
                string.IsNullOrWhiteSpace(replyToken))
                continue;

            var aiAnswer =
                await _gemini.AskAsync(userText);

            await _line.ReplyAsync(
                replyToken,
                aiAnswer);
        }

        return Ok();
    }
}