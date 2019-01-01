namespace DatingApp.API.helpers
{
    public abstract class PageableParams
    {
        public int PageNumber { get; set; } = 1;
        public int UserId { get; set; }
        public int PageSize
        {
            get { return pageSize = 5; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
        protected int pageSize = 5;
        protected const int MaxPageSize = 50;
    }
}