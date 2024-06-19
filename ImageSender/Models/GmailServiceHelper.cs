using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MimeKit;

namespace ImageSender.Models
{
    public class GmailServiceHelper
    {
        static string ApplicationName = "ImageSender";

        public static async Task<bool> SendEmailWithAttachments(string recipientEmail, string folderPath)
        {
            try
            {
                // Load client ID, client secret, and refresh token
                var clientId = "40xxxxxxxxxxxx2-lxxxxxxxxxxxp7.apps.googleusercontent.com";
                var clientSecret = "GxxxxX-mxxxxxxxxxxxxxxxxN";
                var redirectUri = "https://developers.google.com/oauthplayground";
                var refreshToken = "1//04BxxxxxxxxxxxxxGAQSNwF-L9IrzEug-2ayUxxxxxxxxxafGo_mxxxxxxxxxxx9-j58V-3_kcldxxxxxxxxxxxwvPO-MzRrQT8";

                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    },
                    Scopes = new[] { GmailService.Scope.GmailSend },
                    DataStore = new FileDataStore("Google.Apis.Auth.OAuth2.Responses.TokenResponse"),
                });

                // Authenticate with OAuth 2.0
                var token = new Google.Apis.Auth.OAuth2.Responses.TokenResponse { RefreshToken = refreshToken };
                var credential = new UserCredential(flow, "user", token);

                // Create Gmail API service
                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Create MimeMessage
                var message = new MimeMessage();
                //here it is not requied to add sender emailaddress because program get it from Oauth 2.0 Client ID
                //message.From.Add(new MailboxAddress("Sender", "YourMail@gmail.com"));
                message.To.Add(new MailboxAddress("Recipient", recipientEmail));
                message.Subject = "Email with attachments";

                // Attach files from folder
                var files = Directory.GetFiles(folderPath);
                var multipart = new Multipart("mixed");
                foreach (var file in files)
                {
                    var attachment = new MimePart("image", "jpeg")
                    {
                        Content = new MimeContent(File.OpenRead(file), ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(file)
                    };
                    multipart.Add(attachment);
                }
                multipart.Add(new TextPart("plain")
                {
                    Text = "Please find attached files."
                });
                message.Body = multipart;

                // Convert MimeMessage to raw string
                var rawMessage = Base64UrlEncode(message.ToString());

                // Send the message
                var result = await service.Users.Messages.Send(new Message { Raw = rawMessage }, "me").ExecuteAsync().ConfigureAwait(false);

                // Return true if email sent successfully
                return result != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        private static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }
    }
}
