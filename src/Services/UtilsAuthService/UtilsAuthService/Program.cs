using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UtilsAuthService.Application.Commands.RegisterUser;
using UtilsAuthService.Application.Common;
using UtilsAuthService.Domain.Interfaces;
using UtilsAuthService.Infrastructure.Persistance;
using UtilsAuthService.Infrastructure.Repositories;

var b = WebApplication.CreateBuilder(args);
b.Services.AddControllers();
b.Services.AddEndpointsApiExplorer();
b.Services.AddSwaggerGen();

// MediatR + Validators + ValidationBehavior (si lo usás)
b.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());
b.Services.AddValidatorsFromAssemblyContaining<RegisterUserCommand>();
b.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); // si lo tenés en Application.Common

// EF Core
var cs = b.Configuration.GetConnectionString("AuthDb")
         ?? "Host=localhost;Port=5433;Database=auth;Username=postgres;Password=postgres";
b.Services.AddDbContext<AuthDbContext>(o => o.UseNpgsql(cs));

// Repo
b.Services.AddScoped<IAuthUsersRepository, AuthUsersRepository>();

// MassTransit
b.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(ctx);
    });
});

var app = b.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.Run();