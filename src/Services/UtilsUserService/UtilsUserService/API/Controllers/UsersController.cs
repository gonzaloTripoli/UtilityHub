using MediatR;
using Microsoft.AspNetCore.Mvc;
using UtilsUserService.Application.Commands;
using UtilsUserService.Application.Queries;

namespace UtilsUserService.API.Controllers
{
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;
        public UsersController(ISender sender) => _sender = sender;

        public record CreateUserRequest(string Email, string Password, string? Role);

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest body, CancellationToken ct)
        {
            var dto = await _sender.Send(new CreateUserCommand(body.Email, body.Password, body.Role), ct);
            return CreatedAtAction(nameof(Create), new { id = dto.Id }, dto);
        }

        [HttpGet("{id:guid}")] 
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var dto = await _sender.Send(new GetUserByIdQuery(id), ct);
            return dto is null ? NotFound() : Ok(dto);
        }
    }
}
