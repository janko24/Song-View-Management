namespace SongViewLibrary.Model
{
    public class SongViewDto
    {
        public int SongId { get; set; }
        public string SongName { get; set; }
        public long ViewsCount { get; set; }
        public DateTime ExtractedTimestamp { get; set; }
    }
}
