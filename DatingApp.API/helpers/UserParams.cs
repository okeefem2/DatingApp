namespace DatingApp.API.helpers
{
    public class UserParams : PageableParams
    {
        public int MaxAge { get; set; } = 99;
        public int MinAge { get; set; } = 18;
        public string Gender { get; set; }
        public string OrderBy { get; set; }
        public bool Likees { get; set; } = false;
        public bool Likers { get; set; } = false;
    }
}