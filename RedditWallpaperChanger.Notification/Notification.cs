using IWshRuntimeLibrary;
using Microsoft.Win32;
using RedditWallpaperChanger.Action;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedditWallpaperChanger.Notification
{
    public class Notification
    {
        private NotifyIcon notifyIcon;
        private ContextMenuStrip menuStrip;
        private Timer timer;

        public Notification()
        {
            notifyIcon = new NotifyIcon();
            menuStrip = new ContextMenuStrip();

            ReturnObject<UserSettings> response = new RedditWallpaper().GetUserSettingsAsync().GetAwaiter().GetResult();

            if (response.IsSuccess && response.Value != null)
            {
                timer = new Timer
                {
                    Interval = response.Value.Frequency_in_Hours * 60 * 60 * 1000
                };
            }
            else
            {
                //Default time as 1 hour
                timer = new Timer
                {
                    Interval = 1 * 60 * 60 * 1000
                };
            }
            

            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RedditWallpaper wallObj = new RedditWallpaper();
            ReturnObject<UserSettings> response = wallObj.GetUserSettingsAsync().GetAwaiter().GetResult();

            if (!response.IsSuccess || response.Value == null)
            {
                return;
            }

            Wallpaper.Style style;
            switch (response.Value.WallpaperFit)
            {
                case "Fit":
                    style = Wallpaper.Style.Fit;
                    break;
                case "Fill":
                    style = Wallpaper.Style.Fill;
                    break;
                case "Stretch":
                    style = Wallpaper.Style.Stretch;
                    break;
                case "Tile":
                    style = Wallpaper.Style.Tile;
                    break;
                case "Center":
                    style = Wallpaper.Style.Center;
                    break;
                case "Span":
                    style = Wallpaper.Style.Span;
                    break;
                default:
                    style = Wallpaper.Style.Fit;
                    break;
            }

            RedditWallpaper.Sorting sort;
            switch (response.Value.Sorting)
            {
                case "Best":
                    sort = RedditWallpaper.Sorting.Best;
                    break;
                case "Top":
                    sort = RedditWallpaper.Sorting.Top;
                    break;
                case "Hot":
                    sort = RedditWallpaper.Sorting.Hot;
                    break;
                case "New":
                    sort = RedditWallpaper.Sorting.New;
                    break;
                case "Controversial":
                    sort = RedditWallpaper.Sorting.Controversial;
                    break;
                case "Rising":
                    sort = RedditWallpaper.Sorting.Rising;
                    break;
                default:
                    sort = RedditWallpaper.Sorting.Hot;
                    break;
            }

            ReturnObject<string> pathResponse = wallObj.GetWallpaper(response.Value.Subreddit, style, sort, response.Value.Frequency_in_Hours);

            //Update timer interval
            timer.Interval = response.Value.Frequency_in_Hours * 60 * 60 * 1000;

            //Notify that the wallpaper has changed
            notifyIcon.ShowBalloonTip(10000, "Reddit Wallpaper Changer", "Your wallpaper has changed", ToolTipIcon.Info);
            Log4NetManager.Log.Info("Wallpaper changed");
            Log4NetManager.Log.Info("Subreddit: " + response.Value.Subreddit);
            Log4NetManager.Log.Info("Fit: " + response.Value.WallpaperFit);
            Log4NetManager.Log.Info("Sorting: " + response.Value.Sorting);
        }

        public void Create()
        {
            //Setting up the menu
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem()
            {
                Text = "Exit"
            };
            exitMenuItem.Click += ExitMenuItem_Click;

            ToolStripMenuItem settingsMenuItem = new ToolStripMenuItem()
            {
                Text = "Settings"
            };
            settingsMenuItem.Click += SettingsMenuItem_Click;

            menuStrip.Items.Add(settingsMenuItem);
            menuStrip.Items.Add(exitMenuItem);

            //Setting up the notification icon
            notifyIcon.Icon = new System.Drawing.Icon("H:/Sanketh/Projects/C#/RedditWallpaperChanger/RedditWallpaperChanger.Notification/Images/RedditWallpaperChanger_PC.ico");
            
            notifyIcon.ContextMenuStrip = menuStrip;
            notifyIcon.Visible = true;
            
            Timer_Tick(timer, new EventArgs());
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("H:/Sanketh/Projects/C#/RedditWallpaperChanger/RedditWallpaperChanger.UI/bin/debug/RedditWallpaperChanger.UI.exe");
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void RegisterApp()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rkApp.GetValue("RedditWallpaperChanger") == null || rkApp.GetValue("RedditWallpaperChanger").ToString() != Application.ExecutablePath.ToString())
            {
                rkApp.SetValue("RedditWallpaperChanger", Application.ExecutablePath.ToString());
            }

            AppShortcutToDesktop("RedditWallpaperChanger - Shortcut");
        }

        private void AppShortcutToDesktop(string linkName)
        {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            if(!System.IO.File.Exists(deskDir + "\\" + linkName + ".lnk"))
            {
                WshShell shell = new WshShell();
                string shortcutAddress = deskDir + "\\" + linkName + ".lnk";
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
                shortcut.Description = "Shortcut for RedditWallpaperChanger";
                shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                shortcut.Save();
            }
        }
    }
}
