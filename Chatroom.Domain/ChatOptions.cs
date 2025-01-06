namespace Chatroom.Domain
{
    public class ChatOptions
    {

        public string ChatServiceUri { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ClientId { get; set; }
        public string? ClientSecret { get; set; }


    }
}
