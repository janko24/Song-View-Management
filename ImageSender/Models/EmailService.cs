using ImageSender.Data;

namespace ImageSender.Models
{
    public class EmailService
    {
        private readonly FileDbContext _dbContext;

        public EmailService(FileDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<bool> SendEmailAndUpdateDatabase(string recipientEmail, string folderPath)
        {
            try
            {
                var isEmailSent = await GmailServiceHelper.SendEmailWithAttachments(recipientEmail, folderPath);

                var folderFiles = new FolderFiles
                {
                    EmailID = recipientEmail,
                    FolderPath = folderPath,
                    IsMailSend = isEmailSent
                };
                _dbContext.Folders.Add(folderFiles);

                await _dbContext.SaveChangesAsync();

                if (isEmailSent)
                {
                    Console.WriteLine("Successfully send email.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Failed to send email.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }
    }
}
