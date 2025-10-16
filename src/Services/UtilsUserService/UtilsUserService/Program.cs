using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UtilsUserService.Application.Commands;              
using UtilsUserService.Domain.Interfaces;
using UtilsUserService.Infrastructure.Persistence;      
using UtilsUserService.Infrastructure.Repositories;
using UtilsUserService.Application.Common;               
using MediatR.Pipeline;
using UtilsUserService.Application.Commands.UpsertUserFromAuth;
using MassTransit;
using UtilityHub.Bootstrap;
using UtilsUserService.API.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR + FluentValidation
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UpsertUserFromAuthCommand>());
builder.Services.AddValidatorsFromAssemblyContaining<UpsertUserFromAuthCommand>();

// Registrar el pipeline de validación (aplica a TODOS los requests de MediatR)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var usersCs = builder.Configuration.GetConnectionString("UsersDb")
    ?? throw new InvalidOperationException("Missing connection string 'UsersDb'");

builder.Services.AddDbContext<UsersDbContext>(o => o.UseNpgsql(usersCs));

// Repo
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredConsumer>();      // <-- REGISTRA el consumer
    x.SetKebabCaseEndpointNameFormatter();        // nombres legibles (opcional)

    x.UsingRabbitMq((ctx, cfg) =>

    {
        var host = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
        var user = builder.Configuration["RabbitMQ:User"] ?? "guest";
        var pass = builder.Configuration["RabbitMQ:Pass"] ?? "guest";

        cfg.Host(host, "/", h => { h.Username(user); h.Password(pass); });

        // Opción A: endpoint auto (por convención)
        // cfg.ConfigureEndpoints(ctx);

        // Opción B: endpoint con nombre fijo
        cfg.ReceiveEndpoint("users-user-registered", e =>
        {
            e.ConfigureConsumer<UserRegisteredConsumer>(ctx);
        });
    });
});

var app = builder.Build();

if (app.Configuration.GetValue("AutoMigrate", true))
{
    await app.Services.ApplyMigrationsAsync<UtilsUserService.Infrastructure.Persistence.UsersDbContext>(
      app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Migrations"));
}
app.UseUtilityHubErrorHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
