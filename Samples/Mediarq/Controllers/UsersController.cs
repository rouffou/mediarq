using Mediarq.Core.Mediators;
using Mediarq.Samples.Queries;
using Mediarq.Samples.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Mediarq.Samples.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id));
        return result is not null ? Ok(result) : NotFound();
    }
}
