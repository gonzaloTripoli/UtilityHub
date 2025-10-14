using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UtilsUserService.Application.Commands;              
using UtilsUserService.Domain.Interfaces;
using UtilsUserService.Infrastructure.Persistence;      
using UtilsUserService.Infrastructure.Repositories;
using UtilsUserService.Application.Common;               
using MediatR.Pipeline;                                  

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR + FluentValidation
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateUserCommand>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommand>();

// Registrar el pipeline de validación (aplica a TODOS los requests de MediatR)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// DbContext (Postgres)
var cs = builder.Configuration.GetConnectionString("UsersDb")
         ?? "Host=localhost;Port=5432;Database=users;Username=postgres;Password=drowssap";
builder.Services.AddDbContext<UsersDbContext>(o => o.UseNpgsql(cs));

// Repo
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();
app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (ValidationException ex)
    {
        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
        await ctx.Response.WriteAsJsonAsync(new
        {
            error = "ValidationFailed",
            errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
        });
    }
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
