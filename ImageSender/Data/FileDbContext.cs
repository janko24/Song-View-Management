using ImageSender.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageSender.Data
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
        {
        }

        public DbSet<FolderFiles> Folders { get; set; }
    }
}
