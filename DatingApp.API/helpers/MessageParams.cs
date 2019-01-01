namespace DatingApp.API.helpers
{
    public class MessageParams : PageableParams
    {
                public string MessageContainer { get; set; } = "Unread"; // Should have an enum for this
    }
}