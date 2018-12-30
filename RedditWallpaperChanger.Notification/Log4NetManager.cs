using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditWallpaperChanger.Notification
{
    public class Log4NetManager
    {
        public static readonly ILog Log = LogManager.GetLogger("RedditWallpaperChanger.Notification");

        public static void InitializeLog4Net()
        {
            //initialize the log4net configuration based on the log4net.config file
            XmlConfigurator.ConfigureAndWatch(new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"\log4net.config"));

            var loggers = LogManager.GetCurrentLoggers();

            if (LogManager.GetRepository() is Hierarchy hier)
            {
                FileAppender fileAppender = hier.GetAppenders()
                    .Where(x => x.Name.Equals("FileAppender", StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefault() as FileAppender;
            }
        }
    }
}
