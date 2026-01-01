using MediatR;
using Microsoft.AspNetCore.Mvc;
using UtilsAuthService.Application.Commands.LoginUser;
using UtilsAuthService.Application.Commands.RegisterUser;

namespace UtilsAuthService.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        public AuthController(ISender sender) => _sender = sender;

        public record RegisterRequest(string Email, string Password);

        public record LoginRequest(string Email, string Password);

        public record DeleteRequest(Guid id);

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest body, CancellationToken ct)
        {
            var id = await _sender.Send(new RegisterUserCommand(body.Email, body.Password), ct);
            return CreatedAtAction(nameof(Register), new { id }, new { userId = id, email = body.Email.ToLowerInvariant() });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest body, CancellationToken ct)
        {
            var user = await _sender.Send(new LoginUserCommand(body.Email, body.Password), ct);

            return Ok(user);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest, CancellationToken ct) 
        {


            var userDeleted = await _sender.Send(new DeleteUserCommand());
        }

    }
}
