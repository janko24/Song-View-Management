using Google.Apis.YouTube.v3;
using System.Web;

namespace SongViewManagement.Data
{
    public class YouTubeViewCountService
    {
        private readonly YouTubeService _youTubeService;

        public YouTubeViewCountService(string apiKey)
        {
            _youTubeService = YouTubeServiceInitializer.Initialize(apiKey);

        }

        public static string GetVideoIdFromUrl(string url)
        {
            // Check if the URL is valid and uses the HTTPS scheme
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps)
            {
                // Extract the query parameters from the URL
                var queryString = uriResult.Query;

                // Check if the URL contains the "v=" parameter
                if (!string.IsNullOrEmpty(queryString))
                {
                    var queryParams = HttpUtility.ParseQueryString(queryString);
                    if (queryParams.AllKeys.Contains("v"))
                    {
                        return queryParams["v"];
                    }
                }

                // If the URL doesn't contain the "v=" parameter, attempt to parse the video ID from the path
                string[] segments = uriResult.Segments;
                foreach (string segment in segments)
                {
                    if (!string.IsNullOrWhiteSpace(segment) && segment.Trim('/') != "watch")
                    {
                        return segment.Trim('/');
                    }
                }
            }
            return null;
        }

        public async Task<long> GetViewCount(string videoId)
        {
            var videoRequest = _youTubeService.Videos.List("statistics");
            videoRequest.Id = videoId;

            var response = await videoRequest.ExecuteAsync();

            var video = response.Items.FirstOrDefault();
            if (video != null && video.Statistics != null)
            {
                return (long)video.Statistics.ViewCount;
            }
            return 0;
        }

        public async Task<long> GetViewCountFromUrl(string videoUrl)
        {
            string videoId = GetVideoIdFromUrl(videoUrl);
            if (!string.IsNullOrEmpty(videoId))
            {
                return await GetViewCount(videoId);
            }
            return 0; // or throw an exception indicating invalid URL
        }
    }
}
