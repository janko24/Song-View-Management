using ImageSender.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageSender.Controllers
{
    public class FileController : Controller
    {
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;


        public FileController(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail()
        {
            string recipientEmail = _configuration["RecipientEmail"];
            string folderPath = _configuration["FolderPath"];
            //var emailService = new EmailService();
            var isEmailSent = await _emailService.SendEmailAndUpdateDatabase(recipientEmail, folderPath);

            if (isEmailSent)
            {
                ViewBag.Message = "Email sent successfully!";
            }
            else
            {
                ViewBag.Message = "Failed to send email.";
            }

            return View("Index");
        }
    }

}
