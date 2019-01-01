using System;
using DatingApp.API.Models;

namespace DatingApp.API.Dtos
{
    public class MessageForDetailDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        // Auto mapper should actually be able to grab these fields automatically since it knows there is a Sender with a string KnownAs etc.
        public string SenderKnownAs { get; set; }
        public string SenderPhotoUrl { get; set; }
        public string RecipientKnownAs { get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime DateSent { get; set; }
    }
}