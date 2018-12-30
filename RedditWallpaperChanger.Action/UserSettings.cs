using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditWallpaperChanger.Action
{
    public class UserSettings
    {
        public string Subreddit { get; set; }
        public string WallpaperFit { get; set; }
        public string Sorting { get; set; }
        public int Frequency_in_Hours { get; set; }
    }
}
