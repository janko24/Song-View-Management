namespace SongViewManagement.Helper
{
    //this class is for pdf generator 
    public class SongDetails
    {
        public int SongId { get; set; }
        public string SongName { get; set; }
        public string SongUrl { get; set; }
        public long ViewsCount { get; set; }
        public long DailyViews { get; set; }
        public long WeeklyViews { get; set; }
        public long MonthlyViews { get; set; }
    }
}
