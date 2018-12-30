using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RedditWallpaperChanger.Action
{
    public class RedditWallpaper
    {
        public enum Sorting
        {
            Best,
            Hot,
            New,
            Controversial,
            Rising,
            Top
        }

        private ReturnObject<string> PickRedditPost(string subreddit, Sorting sorting)
        {
            ReturnObject<string> urlResponse = new ReturnObject<string>();
            try
            {
                ReturnObject<string> response = WebLibrary.Get("https://www.reddit.com/r/" + subreddit + "/"+ sorting.ToString().ToLower() +".json?limit=50");
                if(response.IsSuccess)
                {
                    Random r = new Random();
                    JObject jObject = JObject.Parse(response.Value);
                    bool isPicked = false;

                    //isPicked will be set to true if we find a landscape image
                    //if all images are searched, and an appropriate image is not found, give an error
                    for (int i = 0; i < 50 && !isPicked; i++)
                    {
                        int postNumber = r.Next(0, 49);

                        var postList = jObject["data"]["children"];

                        bool isRedditMediaDomain = Convert.ToBoolean(postList[postNumber]["data"]["is_reddit_media_domain"]);

                        if (isRedditMediaDomain)
                        {
                            int width, height;

                            width = Convert.ToInt32(postList[postNumber]["data"]["preview"]["images"][0]["source"]["width"]);
                            height = Convert.ToInt32(postList[postNumber]["data"]["preview"]["images"][0]["source"]["height"]);

                            if (width < height)
                            {
                                continue;
                            }

                            urlResponse.Value = Convert.ToString(postList[postNumber]["data"]["url"]);

                            //If the domain is imgur.com, add .jpg to the URL
                            if (Convert.ToString(postList[3]["data"]["domain"]).Equals("imgur.com"))
                            {
                                urlResponse.Value += ".jpg";
                            }

                            isPicked = true;
                        }
                        else
                        {
                            urlResponse.IsSuccess = false;
                        }
                    }

                    if(!isPicked)
                    {
                        urlResponse.IsSuccess = false;
                    }
                    else
                    {
                        urlResponse.IsSuccess = true;
                    }
                }
                else
                {
                    urlResponse.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                urlResponse.Exception = ex;
                urlResponse.IsSuccess = false;
            }

            return urlResponse;
        }

        public ReturnObject<string> GetWallpaper(string subreddit, Wallpaper.Style style, Sorting sorting, int freqency_in_hours)
        {
            UserSettings userSettings = new UserSettings()
            {
                Frequency_in_Hours = freqency_in_hours,
                Sorting = sorting.ToString(),
                WallpaperFit = style.ToString(),
                Subreddit = subreddit
            };

            new DbManager().AddOrUpdateUserSettingsAsync(userSettings).GetAwaiter().GetResult();

            //Get a subreddit post to apply as wallpaper
            ReturnObject<string> urlResponse = PickRedditPost(subreddit, sorting);

            ReturnObject<string> response;
            if (urlResponse.IsSuccess)
            {
                //Save the image as a file and get the file path
                ReturnObject<string> wallpaperPath = WebLibrary.GetFile(urlResponse.Value);
                if(wallpaperPath.IsSuccess)
                {
                    Wallpaper.Set(wallpaperPath.Value, style);

                    response = new ReturnObject<string>
                    {
                        Value = wallpaperPath.Value,
                        IsSuccess = true
                    };
                }
                else
                {
                    response = new ReturnObject<string>
                    {
                        Exception = wallpaperPath.Exception,
                        IsSuccess = false
                    };
                }
            }
            else
            {
                response = new ReturnObject<string>
                {
                    Exception = urlResponse.Exception,
                    IsSuccess = false
                };
            }

            return response;
        }

        public ReturnObject<BitmapImage> GetImage(string path)
        {
            return WebLibrary.GetImage(path);
        }

        public void ChangeWallpaperFit(Wallpaper.Style style)
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            localPath += "/RedditWallpaperChanger/RedditWallpaperChanger/1.0.0.0";
            Wallpaper.Set(localPath + "/Wallpapers/wallpaper.jpg", style);
            new DbManager().UpdateFit(style.ToString()).GetAwaiter().GetResult();
        }

        public void InitializeDb()
        {
            new DbManager().InitDatabaseAsync().GetAwaiter().GetResult();
        }

        public async Task<ReturnObject<UserSettings>> GetUserSettingsAsync()
        {
            return await new DbManager().GetUserSettingsAsync();
        }
    }
}
