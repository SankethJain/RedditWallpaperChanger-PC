using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RedditWallpaperChanger.Action
{
    public static class WebLibrary
    {
        public static ReturnObject<string> Get(string url)
        {
            ReturnObject<string> response = new ReturnObject<string>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.ProtocolVersion = HttpVersion.Version10;
            try
            {
                using (WebResponse webResponse = request.GetResponse())
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                        response.Value = reader.ReadToEnd();
                    }
                }
                response.IsSuccess = true;
            }
            catch (WebException ex)
            {
                response.IsSuccess = false;
                response.Exception = ex;
            }

            return response;
        }

        public static ReturnObject<BitmapImage> GetImage(string url)
        {
            ReturnObject<BitmapImage> bitmapResponse = new ReturnObject<BitmapImage>();
            try
            {
                bitmapResponse.Value = new BitmapImage();
                bitmapResponse.Value.BeginInit();
                bitmapResponse.Value.UriSource = new Uri(url, UriKind.Absolute);
                bitmapResponse.Value.EndInit();

                bitmapResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                bitmapResponse.IsSuccess = false;
                bitmapResponse.Exception = ex;
            }

            return bitmapResponse;
        }

        public static ReturnObject<string> GetFile(string url)
        {
            ReturnObject<string> response = new ReturnObject<string>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ProtocolVersion = HttpVersion.Version10;
            try
            {
                string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                localPath += "/RedditWallpaperChanger/RedditWallpaperChanger/1.0.0.0";
                using (WebResponse webResponse = request.GetResponse())
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        if (!Directory.Exists(localPath + "/Wallpapers"))
                        {
                            Directory.CreateDirectory(localPath + "/Wallpapers");
                        }
                        using (FileStream fs = File.Create(localPath + "/Wallpapers/wallpaper.jpg"))
                        {
                            byte[] buffer = new byte[2048];
                            int bytesRead;
                            do
                            {
                                bytesRead = responseStream.Read(buffer, 0, buffer.Length);

                                fs.Write(buffer, 0, bytesRead);

                            } while (bytesRead > 0);
                        }
                    }
                }

                response.Value = Path.GetFullPath(localPath + "/Wallpapers/wallpaper.jpg");
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Exception = ex;
            }

            return response;
        }
    }
}
