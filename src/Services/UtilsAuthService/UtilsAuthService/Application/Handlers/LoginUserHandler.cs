using MediatR;
using UtilsAuthService.Application.Commands.LoginUser;
using UtilsAuthService.Application.Common.Interfaces;
using UtilsAuthService.Application.DTOs;
using UtilsAuthService.Domain.Interfaces;

namespace UtilsAuthService.Application.Handlers
{
    public sealed class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserDto>
    {
        private readonly IAuthUsersRepository _repo;
        private readonly ITokenService _tokenService;

        public LoginUserHandler(IAuthUsersRepository repo, ITokenService tokenService)
        {
            _repo = repo;
            _tokenService = tokenService;
        }

        public async Task<LoginUserDto> Handle(LoginUserCommand request, CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _repo.GetByEmailAsync(email, ct);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            var token = _tokenService.CreateToken(user.Id, user.Email);

            return new LoginUserDto
            {
                Email = user.Email,
                Token = token
            };
        }
    }
}
