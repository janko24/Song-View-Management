namespace SongViewLibrary.Model
{
    public class Song
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public virtual ICollection<SongView> SongViews { get; set; }
    }
}
