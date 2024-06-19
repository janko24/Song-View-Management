using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace SongViewManagement.Data
{
    public class YouTubeServiceInitializer
    {
        public static YouTubeService Initialize(string apiKey)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = apiKey,
                ApplicationName = "SongViewManagement"
            });

            return youtubeService;
        }
    }
}
