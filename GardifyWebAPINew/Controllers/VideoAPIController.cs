using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GardifyWebAPI.Controllers
{
    public class VideoAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private HttpClient client = new HttpClient();
        private readonly string API_KEY = "AIzaSyDXk0z3xQYEeSPTewZig9WXcgfcTVGi9mw";

        [HttpGet]
        [Route("api/VideosAPI")]
        public async Task<IEnumerable<VideoEntryViewModel>> GetVideos(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
           
            
                if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.Videos, EventObjectType.PageName);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.Videos, EventObjectType.PageName);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Videos, EventObjectType.PageName);
                }

           

            // get ids of uploaded videos
            var UPLOADS_URL = "https://www.googleapis.com/youtube/v3/playlistItems?part=contentDetails&maxResults=50&playlistId=UU0zFLps8o_CWOBOAA3oUuWg&key=" + API_KEY;
            var idesResponse = await client.GetAsync(UPLOADS_URL);

            // TODO: add a fallback in case api limit was reached
            var idesJson = await idesResponse.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<YTResponse>(idesJson);

            var ids = items.items.Select(v => v.contentDetails.videoId);
            var idsStr = string.Join(",", ids);

            // get video details by their ids
            var VIDEOS_URL = "https://www.googleapis.com/youtube/v3/videos?part=statistics%2Csnippet%2CcontentDetails&maxResults=50&id=" + idsStr + "&key=" + API_KEY;
            var vidsResponse = await client.GetAsync(VIDEOS_URL);
            var vidsJson = await vidsResponse.Content.ReadAsStringAsync();
            var videos = JsonConvert.DeserializeObject<YTResponse>(vidsJson);


            Double repeat = 0.0;
            var nextPageToken = items.nextPageToken;
            if (items.pageInfo.totalResults > items.pageInfo.resultsPerPage)
            {
                repeat = Math.Ceiling((items.pageInfo.totalResults - items.pageInfo.resultsPerPage) / items.pageInfo.resultsPerPage);
            }

            for(int x=0; x<repeat; x++)
            {
                var next_url = UPLOADS_URL + "&pageToken=" + nextPageToken;
                idesResponse = await client.GetAsync(next_url);
                idesJson = await idesResponse.Content.ReadAsStringAsync();
                var nextItem = JsonConvert.DeserializeObject<YTResponse>(idesJson);
                nextPageToken = nextItem.nextPageToken;

                ids = nextItem.items.Select(v => v.contentDetails.videoId);
                idsStr = string.Join(",", ids);

                // get video details by their ids
                VIDEOS_URL = "https://www.googleapis.com/youtube/v3/videos?part=statistics%2Csnippet%2CcontentDetails&maxResults=50&id=" + idsStr + "&key=" + API_KEY;
                vidsResponse = await client.GetAsync(VIDEOS_URL);
                vidsJson = await vidsResponse.Content.ReadAsStringAsync();
                var nextvideos = JsonConvert.DeserializeObject<YTResponse>(vidsJson);

                videos.items.AddRange(nextvideos.items);

            }

            // parse IDs
            
            var res = videos.items.Select(v => new VideoEntryViewModel()
            {
                YTLink = "https://www.youtube.com/watch?v=" + v.id,
                ViewCount = v.statistics.viewCount,
                Text = v.snippet.description,
                Title = v.snippet.title,
                SubTitle = "",
                Duration = v.contentDetails.duration,
                Date = v.snippet.publishedAt,
                Tags = v.snippet.tags.Select(t => t.ToLower()).ToList(),
            }).OrderByDescending(v => v.Date);

            return res;
        }

        [HttpGet]
        [Route("api/VideosAPI/getPlaylist")]
        public async Task<IEnumerable<VideoEntryViewModel>> GetPlaylist(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
           
           
                if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.Videos, EventObjectType.PageName);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.Videos, EventObjectType.PageName);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Videos, EventObjectType.PageName);
                }
                else
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), 0, (int)GardifyPages.Videos, EventObjectType.PageName);
                }   



            // get ids of uploaded videos
            var UPLOADS_URL = "https://www.googleapis.com/youtube/v3/playlists?part=contentDetails%2Csnippet&maxResults=50&channelId=UC0zFLps8o_CWOBOAA3oUuWg&key=" + API_KEY;
            var idesResponse = await client.GetAsync(UPLOADS_URL);

            // TODO: add a fallback in case api limit was reached
            var idesJson = await idesResponse.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<YTPlaylistResponse>(idesJson);

            //// parse IDs
            //var ids = items.items.Select(v => v.contentDetails.videoId);
            //var idsStr = string.Join(",", ids);

            //// get video details by their ids
            //var VIDEOS_URL = "https://www.googleapis.com/youtube/v3/videos?part=statistics%2Csnippet%2CcontentDetails&maxResults=50&id=" + idsStr + "&key=" + API_KEY;
            //var vidsResponse = await client.GetAsync(VIDEOS_URL);
            //var vidsJson = await vidsResponse.Content.ReadAsStringAsync();
            //var videos = JsonConvert.DeserializeObject<YTResponse>(vidsJson);

            var res = items.items.Select(v => new VideoEntryViewModel()
            {
                YTLink = "https://www.youtube.com/embed/videoseries?list=" + v.id,
                ViewCount = 0,
                Text = v.snippet.description,
                Title = v.snippet.title,
                SubTitle = "",
                Duration = "",
                Date = v.snippet.publishedAt
            }).OrderByDescending(v => v.Date);

            return res;
        }

        [HttpGet]
        [Route("api/VideosAPI/topics")]
        public IEnumerable<string> GetVideoTopics()
        {
            return new List<string>();
        }
    }

    public class YTResponse
    {
        public string nextPageToken { get; set; }
        public List<VideoItem> items { get; set; }

        public PageInfo pageInfo { get; set; }

    }

    public partial class PageInfo
    {
        public double totalResults { get; set; }

        public double resultsPerPage { get; set; }
    }

    public class YTPlaylistResponse
    {
        public List<PlaylistItem> items { get; set; }
    }

    public class VideoItem
    {
        public string id { get; set; }
        public VideoItemDetails contentDetails { get; set; }
        public VideoItemSnippet snippet { get; set; }
        public VideoItemStats statistics { get; set; }
    }

    public class PlaylistItem
    {
        public string id { get; set; }

        //public VideoItemDetails contentDetails { get; set; }
        public VideoItemSnippet snippet { get; set; }
    }

    public class VideoItemDetails
    {
        public string videoId { get; set; }
        public DateTime videoPublishedAt { get; set; }
        public string duration { get; set; }
    }

    public class VideoItemSnippet
    {
        public VideoItemSnippet() 
        {
            tags = new List<string>();
        }

        public DateTime publishedAt { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Thumbnail thumbnails { get; set; }
        public List<string> tags { get; set; }
    }

    public class VideoItemStats
    {
        public int viewCount { get; set; }
    }

    public class Thumbnail
    {
        public MediumThumb medium { get; set; }
    }

    public class MediumThumb
    {
        public string url { get; set; }
    }
}