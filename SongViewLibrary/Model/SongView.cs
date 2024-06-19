using System.ComponentModel.DataAnnotations.Schema;

namespace SongViewLibrary.Model
{
    public class SongView
    {
        public int Id { get; set; }

        [ForeignKey("Songs")]
        public int SongId { get; set; }
        public long ViewsCount { get; set; }
        public DateTime ExtractedTimestamp { get; set; }
        public virtual Song Song { get; set; }
        public string SongName { get; set; }
    }
}
