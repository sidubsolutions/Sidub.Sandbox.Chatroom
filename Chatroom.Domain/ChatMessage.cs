using Sidub.Platform.Core.Attributes;
using Sidub.Platform.Core.Entity;

namespace Chatroom.Domain
{

    [Entity("ChatMessage")]
    public class ChatMessage : IEntity
    {

        [EntityField<string>("Message")]
        public string Message { get; set; }

        public bool IsRetrievedFromStorage { get; set; }
    }


}