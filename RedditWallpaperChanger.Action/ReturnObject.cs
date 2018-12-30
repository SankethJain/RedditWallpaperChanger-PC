using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditWallpaperChanger.Action
{
    public class ReturnObject<T> where T : class
    {
        public bool IsSuccess { get; set; }
        public Exception Exception { get; set; }
        public T Value { get; set; }
    }
}
