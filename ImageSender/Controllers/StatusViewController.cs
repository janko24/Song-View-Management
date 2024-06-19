using ImageSender.Helper;
using Microsoft.AspNetCore.Mvc;

namespace ImageSender.Controllers
{
    public class StatusViewController : Controller
    {
        private readonly StatusViewHelper statusView;

        public StatusViewController(StatusViewHelper statusView)
        {
            this.statusView = statusView;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var status = await statusView.GetStatusOfAllMailAsync();
            return View(status);
        }
    }
}
