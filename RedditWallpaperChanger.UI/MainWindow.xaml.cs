using RedditWallpaperChanger.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RedditWallpaperChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            new RedditWallpaper().InitializeDb();
            ReturnObject<UserSettings> response = new RedditWallpaper().GetUserSettingsAsync().GetAwaiter().GetResult();

            if (!response.IsSuccess || response.Value == null)
            {
                return;
            }

            fill.IsChecked = false;
            fit.IsChecked = false;
            stretch.IsChecked = false;
            tile.IsChecked = false;
            span.IsChecked = false;
            center.IsChecked = false;

            best.IsChecked = false;
            top.IsChecked = false;
            hot.IsChecked = false;
            rising.IsChecked = false;
            controversial.IsChecked = false;
            @new.IsChecked = false;

            switch (response.Value.WallpaperFit)
            {
                case "Fit":
                    fit.IsChecked = true;
                    break;
                case "Fill":
                    fill.IsChecked = true;
                    break;
                case "Stretch":
                    stretch.IsChecked = true;
                    break;
                case "Tile":
                    tile.IsChecked = true;
                    break;
                case "Center":
                    center.IsChecked = true;
                    break;
                case "Span":
                    span.IsChecked = true;
                    break;
                default:
                    fill.IsChecked = true;
                    break;
            }

            switch (response.Value.Sorting)
            {
                case "Best":
                    best.IsChecked = true;
                    break;
                case "Top":
                    top.IsChecked = true;
                    break;
                case "Hot":
                    hot.IsChecked = true;
                    break;
                case "New":
                    @new.IsChecked = true;
                    break;
                case "Controversial":
                    controversial.IsChecked = true;
                    break;
                case "Rising":
                    rising.IsChecked = true;
                    break;
                default:
                    hot.IsChecked = true;
                    break;
            }

            subredditList.SelectedValue = response.Value.Subreddit;

            if(response.Value.Frequency_in_Hours == 24)
            {
                frequency.SelectedValue = "1 day";
            }
            else
            {
                frequency.SelectedValue = response.Value.Frequency_in_Hours + " hours";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem boxItem = (ComboBoxItem)subredditList.SelectedItem;
            if(boxItem == null)
            {
                MessageBox.Show("Please select a valid subreddit.", "Invalid subreddit", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                return;
            }

            string subreddit = boxItem.Content.ToString();
            Wallpaper.Style style;
            RedditWallpaper.Sorting sort;

            RadioButton fitRadioButton = this.FindDescendants<RadioButton>(r => r.IsChecked == true && r.GroupName== "wallpaperFit").FirstOrDefault();
            switch(fitRadioButton.Name)
            {
                case "fit": style = Wallpaper.Style.Fit;
                    break;
                case "fill":
                    style = Wallpaper.Style.Fill;
                    break;
                case "stretch":
                    style = Wallpaper.Style.Stretch;
                    break;
                case "tile":
                    style = Wallpaper.Style.Tile;
                    break;
                case "center":
                    style = Wallpaper.Style.Center;
                    break;
                case "span":
                    style = Wallpaper.Style.Span;
                    break;
                default: style = Wallpaper.Style.Fit;
                    break;
            }

            RadioButton sortRadioButton = this.FindDescendants<RadioButton>(r => r.IsChecked == true && r.GroupName == "postSort").FirstOrDefault();
            switch (sortRadioButton.Name)
            {
                case "best":
                    sort = RedditWallpaper.Sorting.Best;
                    break;
                case "top":
                    sort = RedditWallpaper.Sorting.Top;
                    break;
                case "hot":
                    sort = RedditWallpaper.Sorting.Hot;
                    break;
                case "new":
                    sort = RedditWallpaper.Sorting.New;
                    break;
                case "controversial":
                    sort = RedditWallpaper.Sorting.Controversial;
                    break;
                case "rising":
                    sort = RedditWallpaper.Sorting.Rising;
                    break;
                default:
                    sort = RedditWallpaper.Sorting.Hot;
                    break;
            }

            ComboBoxItem frequencyItem = (ComboBoxItem)frequency.SelectedItem;
            if (frequencyItem == null)
            {
                MessageBox.Show("Please select a valid frequency.", "Invalid frequency", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                return;
            }

            int frequency_in_hours;
            if (frequencyItem.Content.ToString() == "1 day")
            {
                frequency_in_hours = 24;
            }
            else
            {
                frequency_in_hours = Convert.ToInt32(frequencyItem.Content.ToString().Substring(0, 2));
            }

            RedditWallpaper wallpaper = new RedditWallpaper();
            ReturnObject<string> response = wallpaper.GetWallpaper(subreddit, style, sort, frequency_in_hours);
            if(response.IsSuccess)
            {
                wallpaperImg.Source = wallpaper.GetImage(response.Value).Value;
            }
        }

        private void FitChange_Click(object sender, RoutedEventArgs e)
        {
            Wallpaper.Style style;
            RadioButton fitRadioButton = this.FindDescendants<RadioButton>(r => r.IsChecked == true && r.GroupName == "wallpaperFit").FirstOrDefault();
            switch (fitRadioButton.Name)
            {
                case "fit":
                    style = Wallpaper.Style.Fit;
                    break;
                case "fill":
                    style = Wallpaper.Style.Fill;
                    break;
                case "stretch":
                    style = Wallpaper.Style.Stretch;
                    break;
                case "tile":
                    style = Wallpaper.Style.Tile;
                    break;
                case "center":
                    style = Wallpaper.Style.Center;
                    break;
                case "span":
                    style = Wallpaper.Style.Span;
                    break;
                default:
                    style = Wallpaper.Style.Fit;
                    break;
            }

            new RedditWallpaper().ChangeWallpaperFit(style);
        }
    }
}
