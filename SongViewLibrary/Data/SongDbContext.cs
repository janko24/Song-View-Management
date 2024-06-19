using Microsoft.EntityFrameworkCore;
using SongViewLibrary.Model;

namespace SongViewManagement.Data
{
    public class SongDbContext : DbContext
    {
        public SongDbContext(DbContextOptions<SongDbContext> options) : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }
        public DbSet<SongView> SongViews { get; set; }

    }
}
