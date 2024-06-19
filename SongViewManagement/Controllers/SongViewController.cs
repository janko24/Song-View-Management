using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SongViewManagement.Helper;

namespace SongViewManagement.Controllers
{
    public class SongViewController : Controller
    {
        private readonly SongViewHelper _songViewHelper;
        private readonly SongViewPdfDocument _pdfDocument;

        public SongViewController(SongViewHelper songViewHelper, SongViewPdfDocument pdfDocument)
        {
            _songViewHelper = songViewHelper;
            _pdfDocument = pdfDocument;
        }

        public async Task<IActionResult> Index()
        {
            var songViews = await _songViewHelper.GetSongViewsAsync();

            var calculatedViews = new List<SongViewCalculations>();

            foreach (var song in songViews.GroupBy(sv => sv.SongId))
            {
                var songViewsForSong = song.ToList();
                long dailyView = _songViewHelper.CalculateDailyView(songViewsForSong);
                long weeklyView = _songViewHelper.CalculateWeeklyView(songViewsForSong);
                long monthlyView = _songViewHelper.CalculateMonthlyView(songViewsForSong);

                var calculation = new SongViewCalculations
                {
                    SongId = song.Key,
                    DailyView = dailyView,
                    WeeklyView = weeklyView,
                    MonthlyView = monthlyView
                };

                calculatedViews.Add(calculation);
            }

            ViewBag.CalculatedViews = calculatedViews;

            // Pass the SongViewHelper instance to the view using ViewBag
            ViewBag.SongViewHelper = _songViewHelper;


            return View(songViews);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewSongViewRows()
        {
            await _songViewHelper.AddNewSongViewRowsAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var songView = await _songViewHelper.GetSongViewByIdAsync(id.Value);
            if (songView == null)
            {
                return NotFound();
            }


            return View(songView);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isDeleted = await _songViewHelper.DeleteSongAndViewAsync(id);
            if (!isDeleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ExtractPdf(string selectedSongs)
        {
            // Convert selectedSongs string to list of song ids
            List<int> songIds = selectedSongs.Split(',').Select(int.Parse).ToList();

            // Retrieve song details based on the selected song ids
            var songDetails = _songViewHelper.GetSongDetails(songIds);

            return View(songDetails);

        }

        [HttpPost]
        public IActionResult ExportPdf(string songDetailsJson)
        {
            if (string.IsNullOrEmpty(songDetailsJson))
            {
                return BadRequest("No song details provided.");
            }

            // Deserialize the JSON string to get the song details
            var songDetails = JsonConvert.DeserializeObject<List<SongDetails>>(songDetailsJson);

            // Generate the PDF using the provided song details
            var pdfStream = _pdfDocument.GeneratePdf(songDetails);
            // Return the PDF file
            return File(pdfStream, "application/pdf", "song_details.pdf");
        }


    }
}
