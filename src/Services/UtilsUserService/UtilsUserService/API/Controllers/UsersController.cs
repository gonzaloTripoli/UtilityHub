using MediatR;
using Microsoft.AspNetCore.Mvc;
using UtilsUserService.Application.Queries;

namespace UtilsUserService.API.Controllers
{
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;
        public UsersController(ISender sender) => _sender = sender;

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var dto = await _sender.Send(new GetUserByIdQuery(id), ct);
            return dto is null ? NotFound() : Ok(dto);
        }
    }
}
