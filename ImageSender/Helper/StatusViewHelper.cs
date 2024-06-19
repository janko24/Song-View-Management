using ImageSender.Data;
using ImageSender.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageSender.Helper
{
    public class StatusViewHelper
    {
        private readonly FileDbContext _dbContext;

        public StatusViewHelper(FileDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<FolderFiles>> GetStatusOfAllMailAsync()
        {
            return await _dbContext.Folders.ToListAsync();
        }
    }
}
