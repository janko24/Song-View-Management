using Microsoft.EntityFrameworkCore;
using SongViewLibrary.Model;
using SongViewManagement.Data;

namespace SongViewManagement.Helper
{
    public class SongViewHelper
    {
        private readonly SongDbContext _context;
        private readonly YouTubeViewCountService _youTubeService;

        public SongViewHelper(SongDbContext context, YouTubeViewCountService youTubeService)
        {
            _context = context;
            _youTubeService = youTubeService;
        }



        public async Task<List<SongView>> GetSongViewsAsync()
        {
            return await _context.SongViews.Include(sv => sv.Song).ToListAsync();
        }

        public async Task<SongView> GetSongViewByIdAsync(int id)
        {
            return await _context.SongViews.FirstOrDefaultAsync(sv => sv.SongId == id);
        }

        public async Task<List<SongViewDto>> AddNewSongViewRowsAsync()
        {
            var songs = await _context.Songs.ToListAsync();
            var songViewDtos = new List<SongViewDto>();

            foreach (var song in songs)
            {
                long youtubeViewCount = await _youTubeService.GetViewCountFromUrl(song.Url);

                var newSongViewDTO = new SongViewDto
                {
                    SongId = song.Id,
                    SongName = song.Name,
                    ViewsCount = youtubeViewCount,
                    ExtractedTimestamp = DateTime.UtcNow
                };

                songViewDtos.Add(newSongViewDTO);
            }

            await SaveSongViewDtosAsync(songViewDtos);

            return songViewDtos;
        }

        private async Task SaveSongViewDtosAsync(List<SongViewDto> songViewDtos)
        {
            foreach (var songViewDto in songViewDtos)
            {
                var newSongView = new SongView
                {
                    SongId = songViewDto.SongId,
                    SongName = songViewDto.SongName,
                    ViewsCount = songViewDto.ViewsCount,
                    ExtractedTimestamp = songViewDto.ExtractedTimestamp
                };

                await _context.SongViews.AddAsync(newSongView);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteSongAndViewAsync(int id)
        {
            var songView = await _context.SongViews.FindAsync(id);
            if (songView == null)
            {
                return false;
            }

            var song = await _context.Songs.FirstOrDefaultAsync(s => s.Id == songView.SongId);
            if (song == null)
            {
                return false;
            }

            try
            {
                _context.SongViews.Remove(songView);
                await _context.SaveChangesAsync();

                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //for pdf generator
        public List<SongDetails> GetSongDetails(List<int> songIds)
        {
            var songDetails = new List<SongDetails>();

            foreach (var songId in songIds)
            {
                var allSongViews = _context.SongViews.Where(sv => sv.SongId == songId).ToList();
                if (allSongViews.Any())
                {
                    var latestSongView = allSongViews.OrderByDescending(sv => sv.ExtractedTimestamp).FirstOrDefault();
                    if (latestSongView != null)
                    {
                        var song = _context.Songs.FirstOrDefault(s => s.Id == latestSongView.SongId);
                        if (song != null)
                        {
                            var songDetail = new SongDetails
                            {
                                SongId = song.Id,
                                SongName = song.Name,
                                SongUrl = song.Url,
                                ViewsCount = latestSongView.ViewsCount,
                                DailyViews = CalculateDailyView(allSongViews),
                                WeeklyViews = CalculateWeeklyView(allSongViews),
                                MonthlyViews = CalculateMonthlyView(allSongViews)
                            };

                            songDetails.Add(songDetail);
                        }
                    }
                }
            }

            return songDetails;
        }


        public long CalculateDailyView(List<SongView> songViewsForSong)
        {
            long dailyView = 0;

            var currentDay = DateTime.Today;
            var previousDay = currentDay.AddDays(-1);
            var previousDayMinus1 = previousDay.AddDays(-1);

            var previousDayViews = songViewsForSong
                .FirstOrDefault(sv => sv.ExtractedTimestamp.Date == previousDay.Date)?.ViewsCount ?? 0;

            var previousDayMinus1Views = songViewsForSong
                .FirstOrDefault(sv => sv.ExtractedTimestamp.Date == previousDayMinus1.Date)?.ViewsCount ?? 0;

            dailyView = previousDayViews - previousDayMinus1Views;

            return Math.Max(0, dailyView);
        }

        public long CalculateWeeklyView(List<SongView> songViewsForSong)
        {
            var currentDate = DateTime.Now.Date;
            var currentWeekStart = currentDate.AddDays(-(int)currentDate.DayOfWeek + 1);
            var previousWeekStart = currentWeekStart.AddDays(-7);
            var previousWeekEnd = previousWeekStart.AddDays(6);
            var previousWeekMinusStart = previousWeekStart.AddDays(-7);
            var previousWeekMinusEnd = previousWeekMinusStart.AddDays(6);

            var previousWeekViews = songViewsForSong
                .FirstOrDefault(sv => sv.ExtractedTimestamp.Date == previousWeekEnd.Date)?.ViewsCount ?? 0;

            var previousWeekMinusViews = songViewsForSong
                .FirstOrDefault(sv => sv.ExtractedTimestamp.Date == previousWeekMinusEnd.Date)?.ViewsCount ?? 0;

            var weeklyView = previousWeekViews - previousWeekMinusViews;

            return Math.Max(0, weeklyView);
        }

        public long CalculateMonthlyView(List<SongView> songViewsForSong)
        {
            long monthlyView = 0;

            var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var previousMonthEnd = currentMonthStart.AddDays(-1);
            var previousMonthMinusStart = previousMonthStart.AddMonths(-1);
            var previousMonthMinusEnd = previousMonthStart.AddDays(-1);

            var previousMonthViews = songViewsForSong
                .FirstOrDefault(sv => sv.ExtractedTimestamp.Date == previousMonthEnd)?.ViewsCount ?? 0;

            var previousMonthMinusViews = songViewsForSong
            .FirstOrDefault(sv => sv.ExtractedTimestamp.Date == previousMonthMinusEnd)?.ViewsCount ?? 0;

            monthlyView = previousMonthViews - previousMonthMinusViews;

            return Math.Max(0, monthlyView);
        }

        public string FormatNumberWithCommasCustom(long number)
        {
            return string.Format("{0:n0}", number);
        }

    }
}
