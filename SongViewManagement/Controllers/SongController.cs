using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SongViewLibrary.Model;
using SongViewManagement.Helper;

namespace SongViewManagement.Controllers
{
    public class SongController : Controller
    {
        private readonly SongHelper _songHelper;
        private readonly IHubContext<SongHub> _hubContext;

        public SongController(SongHelper songHelper, IHubContext<SongHub> hubContext)
        {
            _songHelper = songHelper;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var songs = await _songHelper.GetAllSongsAsync();
            return View(songs);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddSong addSong)
        {
            if (!ModelState.IsValid)
            {
                return View(addSong);
            }

            //var isAdded = await _songHelper.AddSongAsync(addSong);
            var newSongId = await _songHelper.AddSongAsync(addSong);


            if (newSongId == -1)
            {
                ModelState.AddModelError(string.Empty, "The song already exists.");
                return View(addSong);
            }

            await _hubContext.Clients.All.SendAsync("ReceiveSongUpdate", "add", newSongId);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var song = await _songHelper.GetSongByIdAsync(id);

            if (song == null)
            {
                return RedirectToAction("Index");
            }

            var updateSong = new UpdateSong()
            {
                Id = song.Id,
                Name = song.Name,
                Url = song.Url
            };

            return View(updateSong);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateSong update)
        {
            if (!ModelState.IsValid)
            {
                return View(update);
            }

            var isUpdated = await _songHelper.UpdateSongAsync(update);

            if (isUpdated)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveSongUpdate", "edit", update.Id);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePopover(int id)
        {
            var isDeleted = await _songHelper.DeleteSongAsync(id);

            if (isDeleted)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveSongUpdate", "delete", id);
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        //[HttpPost]
        //public async Task<IActionResult> Delete(UpdateSong update)
        //{
        //    var isDeleted = await _songHelper.DeleteSongAsync(update.Id);

        //    if (isDeleted)
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    return RedirectToAction("Index");
        //}

    }

}
