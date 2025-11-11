using MassTransit;
using MediatR;

namespace UtilsUserService.API.Messaging
{
    public class UserLoginConsumer : IConsumer<UserLogged>
    {
        private readonly ISender _sender;

        public UserRegisteredConsumer(ISender sender) => _sender = sender;

        public Task Consume(ConsumeContext<Userlogged> ctx)
            => _sender.Send()
       

    }
}
