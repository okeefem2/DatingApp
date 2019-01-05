using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public Boolean IsMain { get; set; }
        public Boolean IsApproved { get; set; }
        // This ties the tables together, and will cause a cascade delete to occur (User deleted causes the photo to delete as well)
        public User User { get; set; } // Newtonsoft.Json.JsonSerializationException: Self referencing loop detected for property
        public int UserId { get; set; }
        public string PublicId { get; set; }
    }
}