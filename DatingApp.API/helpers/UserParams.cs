namespace DatingApp.API.helpers
{
    public class UserParams
    {
        public int PageNumber { get; set; } = 1;
        public int UserId { get; set; }
        public int MaxAge { get; set; } = 99;
        public int MinAge { get; set; } = 18;
        public string Gender { get; set; }
        public string OrderBy { get; set; }
        public bool Likees { get; set; } = false;
        public bool Likers { get; set; } = false;
        public int PageSize
        {
            get { return pageSize = 5; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
        private int pageSize = 5;
        private const int MaxPageSize = 50;
    }
}