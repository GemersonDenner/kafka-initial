using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{

    private readonly ILogger<MessageController> _logger;

    public MessageController(ILogger<MessageController> logger)
    {
        _logger = logger;
    }

    [HttpPost("PostJson")]
    public IResult PostJson(string text)
    {
        var message = new Message(text);
        return Results.Created("/PostJson", message);
    }

    [HttpPost("PostAvro")]
    public IResult PostAvro(string text)
    {
        var message = new Message(text);
        return Results.Created("/PostAvro", message);
    }
}
