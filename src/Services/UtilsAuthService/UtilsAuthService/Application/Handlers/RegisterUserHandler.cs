using MassTransit;
using MediatR;
using UtilityHub.Contracts.Identity;
using UtilsAuthService.Application.Commands;
using UtilsAuthService.Application.Commands.RegisterUser;
using UtilsAuthService.Domain.Entities;
using UtilsAuthService.Domain.Interfaces;


namespace UtilsAuthService.Application.Handlers
{
    public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IAuthUsersRepository _repo;
        private readonly IPublishEndpoint _publish;

        public RegisterUserHandler(IAuthUsersRepository repo, IPublishEndpoint publish)
        {
            _repo = repo; _publish = publish;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (await _repo.EmailExistsAsync(email, ct))
                throw new InvalidOperationException("Email ya registrado.");

            var user = new AuthUser
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _repo.AddAsync(user, ct);

            await _publish.Publish(new UserRegistered(user.Id, user.Email, DateTime.UtcNow), ct);

            return user.Id;
        }
    }
}
