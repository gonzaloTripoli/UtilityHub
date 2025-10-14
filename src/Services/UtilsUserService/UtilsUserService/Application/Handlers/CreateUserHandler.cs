using MediatR;
using System.Text;
using UtilsUserService.Application.Commands;
using UtilsUserService.Application.DTOs;
using UtilsUserService.Domain.Entities;
using UtilsUserService.Domain.Interfaces;

namespace UtilsUserService.Application.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserRepository _repo;
        public CreateUserHandler(IUserRepository repo) => _repo = repo;

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var exists = await _repo.GetByEmailAsync(email, ct);
            if (exists is not null) throw new InvalidOperationException("Email ya registrado.");

            var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = email,
                PasswordHash = hash,
                Role = string.IsNullOrWhiteSpace(request.Role) ? "user" : request.Role!
            };

            await _repo.AddAsync(user, ct);
            return new UserDto(user.Id, user.Email, user.Role, user.CreatedAt);
        }
    }
    }
