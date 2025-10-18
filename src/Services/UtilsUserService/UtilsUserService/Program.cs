using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UtilityHub.Bootstrap;
using UtilsUserService.API.Messaging;
using UtilsUserService.Application.Commands.UpsertUserFromAuth;
using UtilsUserService.Application.Common;
using UtilsUserService.Domain.Interfaces;
using UtilsUserService.Infrastructure.Persistence;
using UtilsUserService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UpsertUserFromAuthCommand>());
builder.Services.AddValidatorsFromAssemblyContaining<UpsertUserFromAuthCommand>();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var usersCs = builder.Configuration.GetConnectionString("UsersDb")
    ?? throw new InvalidOperationException("Missing connection string 'UsersDb'");

builder.Services.AddDbContext<UsersDbContext>(o => o.UseNpgsql(usersCs));

// Repo
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredConsumer>();      
    x.SetKebabCaseEndpointNameFormatter();       

    x.UsingRabbitMq((ctx, cfg) =>

    {
        var host = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
        var user = builder.Configuration["RabbitMQ:User"] ?? "guest";
        var pass = builder.Configuration["RabbitMQ:Pass"] ?? "guest";

        cfg.Host(host, "/", h => { h.Username(user); h.Password(pass); });

        
        // cfg.ConfigureEndpoints(ctx);

      
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
