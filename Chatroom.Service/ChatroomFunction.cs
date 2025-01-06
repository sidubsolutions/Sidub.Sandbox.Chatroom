using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidub.Messaging.Host.SignalR;
using Sidub.Platform.Authentication.IsolatedFunction;

namespace Chatroom.Service
{

    [MessagingHub("Chatroom")]
    public partial class ChatroomFunction : MessagingHub
    {

        public ChatroomFunction(IOptionsSnapshot<MessageServerOptions> options, IMessagingHubConnectionStore connectionStore, ILoggerFactory loggerFactory, IOptionsSnapshot<AuthenticationOptions> options2)
            : base(options, connectionStore, loggerFactory)
        {

        }

    }
}
