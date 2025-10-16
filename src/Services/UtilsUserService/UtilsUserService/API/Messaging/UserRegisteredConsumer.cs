using MassTransit;
using MediatR;
using UtilityHub.Contracts.Identity;
using UtilsUserService.Application.Commands.UpsertUserFromAuth;

namespace UtilsUserService.API.Messaging;

public class UserRegisteredConsumer : IConsumer<UserRegistered>
{
    private readonly ISender _sender;
    public UserRegisteredConsumer(ISender sender) => _sender = sender;

    public Task Consume(ConsumeContext<UserRegistered> ctx)
        => _sender.Send(new UpsertUserFromAuthCommand(
            ctx.Message.UserId,
            ctx.Message.Email,
            ctx.Message.OccurredAtUtc
        ), ctx.CancellationToken);
}
