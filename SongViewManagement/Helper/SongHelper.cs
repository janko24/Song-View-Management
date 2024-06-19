using Microsoft.EntityFrameworkCore;
using SongViewLibrary.Model;
using SongViewManagement.Data;

namespace SongViewManagement.Helper
{
    public class SongHelper
    {
        private readonly SongDbContext _context;
        private readonly YouTubeViewCountService _youTubeService;

        public SongHelper(SongDbContext context, YouTubeViewCountService youTubeService)
        {
            _context = context;
            _youTubeService = youTubeService;
        }

        public async Task<List<Song>> GetAllSongsAsync()
        {
            return await _context.Songs.ToListAsync();
        }

        public async Task<Song> GetSongByIdAsync(int id)
        {
            return await _context.Songs.FindAsync(id);
        }

        //public async Task<bool> AddSongAsync(AddSong addSong)
        public async Task<int> AddSongAsync(AddSong addSong)
        {
            var existingSong = await _context.Songs.FirstOrDefaultAsync(
                s => s.Name == addSong.Name || s.Url == addSong.Url);

            if (existingSong != null)
            {
                //return false;
                return -1;
            }

            var song = new Song()
            {
                Name = addSong.Name,
                Url = addSong.Url
            };

            await _context.Songs.AddAsync(song);
            await _context.SaveChangesAsync();

            var songView = new SongView
            {
                SongId = song.Id,
                Song = song,
                SongName = song.Name,
                ExtractedTimestamp = DateTime.UtcNow,
                ViewsCount = 0,
            };

            long youtubeViewCount = await _youTubeService.GetViewCountFromUrl(songView.Song.Url);
            songView.ViewsCount = youtubeViewCount;

            await _context.SongViews.AddAsync(songView);
            await _context.SaveChangesAsync();

            return song.Id;
            //return true;
        }

        public async Task<bool> UpdateSongAsync(UpdateSong update)
        {
            var song = await _context.Songs.FindAsync(update.Id);

            if (song == null)
            {
                return false;
            }

            song.Name = update.Name;
            song.Url = update.Url;

            await _context.SaveChangesAsync();

            var oldSongName = song.Name;
            var oldSongUrl = song.Url;
            var songViewsToUpdate = await _context.SongViews
                                                    .Where(sv => sv.SongId == song.Id)
                                                    .ToListAsync();

            foreach (var songView in songViewsToUpdate)
            {
                songView.SongName = update.Name;
                songView.ExtractedTimestamp = DateTime.UtcNow;

                // If the URL has changed, you may want to update view count based on the new URL
                if (oldSongUrl != update.Url)
                {
                    long youtubeViewCount = await _youTubeService.GetViewCountFromUrl(update.Url);
                    songView.ViewsCount = youtubeViewCount;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSongAsync(int id)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return false;
            }

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
