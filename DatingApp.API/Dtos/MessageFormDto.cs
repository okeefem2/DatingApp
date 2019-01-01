using System;

namespace DatingApp.API.Dtos
{
    public class MessageFormDto
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateSent { get; set; }

        public MessageFormDto()
        {
            DateSent = DateTime.Now;
        }
    }
}