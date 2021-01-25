using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace SonosTwitch.Models
{
    public class YoutubeVideoOffer : Item
    {
        private BaseClientService.Initializer _baseClientService;
        private string _url;

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged(nameof(Url));
            }
        }

        private string _username;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _videoTitle;

        public string VideoTitle
        {
            get => _videoTitle;
            set
            {
                _videoTitle = value;
                OnPropertyChanged(nameof(VideoTitle));
            }
        }

        private string _videoChannel;

        public string VideoChannel
        {
            get => _videoChannel;
            set
            {
                _videoChannel = value;
                OnPropertyChanged(nameof(VideoChannel));
            }
        }

        public YoutubeVideoOffer(string url, string username, bool isUrl = true)
        {
            _baseClientService = new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyD7IDc1i7nbaYb313WohrZOBr9c3CROuK8",
                ApplicationName = "SonosTwitch"
            };
            if (isUrl)
            {
                Url = url.Replace("watch?v=", "embed/");
                Username = username;
                try
                {
                    var youtubeService = new YouTubeService(_baseClientService);

                    var searchListRequest = youtubeService.Search.List("snippet");
                    searchListRequest.Q = url.Replace("embed/", "watch?v="); // Replace with your search term.
                    searchListRequest.MaxResults = 5;

                    // Call the search.list method to retrieve results matching the specified query term.
                    var searchListResponse = searchListRequest.ExecuteAsync().Result;
                    var video = searchListResponse.Items.First(x => x.Id.Kind == "youtube#video");
                    VideoTitle = video.Snippet.Title;
                    VideoChannel = video.Snippet.ChannelTitle;
                   // Url = Url.IndexOf("https://www.youtube.com/embed/") < 0 ? $"https://www.youtube.com/embed/{video.Id}" : Url;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                Username = username;
                try
                {
                    var youtubeService = new YouTubeService(_baseClientService);

                    var searchListRequest = youtubeService.Search.List("snippet");
                    searchListRequest.Q = url; 
                    searchListRequest.MaxResults = 5;

                    // Call the search.list method to retrieve results matching the specified query term.
                    var searchListResponse = searchListRequest.ExecuteAsync().Result;
                    var video = searchListResponse.Items.First(x => x.Id.Kind == "youtube#video");
                    VideoTitle = video.Snippet.Title;
                    VideoChannel = video.Snippet.ChannelTitle;
                    Url = $"https://www.youtube.com/embed/{video.Id.VideoId}";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            
        }
    }
}
