using System.Text;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UtilityHub.Bootstrap;
using UtilsAuthService.Application.Commands.RegisterUser;
using UtilsAuthService.Application.Common;
using UtilsAuthService.Domain.Interfaces;
using UtilsAuthService.Infrastructure.Persistance;
using UtilsAuthService.Infrastructure.Repositories;

var b = WebApplication.CreateBuilder(args);
b.Services.AddControllers();
b.Services.AddEndpointsApiExplorer();
b.Services.AddSwaggerGen();

b.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());
b.Services.AddValidatorsFromAssemblyContaining<RegisterUserCommand>();
b.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); // si lo tenés en Application.Common


b.Services.AddAuthentication(JwtBearerDefaults
    .AuthenticationScheme).
    AddJwtBearer(options => {

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(b.Configuration["Jwt:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = b.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = b.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
        };
    
    
    
    
    });

b.Services.AddAuthorization();



var authCs = b.Configuration.GetConnectionString("AuthDb")
    ?? throw new InvalidOperationException("Missing connection string 'AuthDb'");
b.Services.AddDbContext<AuthDbContext>(o => o.UseNpgsql(authCs));

b.Services.AddScoped<IAuthUsersRepository, AuthUsersRepository>();
b.Services.AddMassTransit(x =>
{

    x.UsingRabbitMq((ctx, cfg) =>
    {
        var host = b.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
        var user = b.Configuration["RabbitMQ:User"] ?? "guest";
        var pass = b.Configuration["RabbitMQ:Pass"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(user);
            h.Password(pass);
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = b.Build();
app.UseAuthentication();
app.UseAuthorization(); 

if (app.Configuration.GetValue("AutoMigrate", true))
{
    await app.Services.ApplyMigrationsAsync<UtilsAuthService.Infrastructure.Persistance.AuthDbContext>(
        app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Migrations"));
}

app.UseUtilityHubErrorHandler();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.Run();