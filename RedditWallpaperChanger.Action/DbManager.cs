using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditWallpaperChanger.Action
{
    public class DbManager
    {
        private SQLiteConnection GetConnection()
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string connStr = "Data Source="+ localPath + "/RedditWallpaperChanger/RedditWallpaperChanger/1.0.0.0/Data/RedditWallpaperChanger.sqlite;Version=3;";

            return new SQLiteConnection(connStr);
        }
        private void CreateDatabase()
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            localPath += "/RedditWallpaperChanger/RedditWallpaperChanger/1.0.0.0";
            SQLiteConnection.CreateFile(localPath + "/Data/RedditWallpaperChanger.sqlite");
        }

        private async Task CreateTablesAsync()
        {
            using (SQLiteConnection conn = GetConnection())
            {
                await conn.OpenAsync();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "CREATE TABLE UserSettings (Subreddit VARCHAR(100), WallpaperFit VARCHAR(10), Sorting VARCHAR(20), Frequency_in_Hours INT)";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task AddOrUpdateUserSettingsAsync(UserSettings settings)
        {
            ReturnObject<UserSettings> tempResponse = await GetUserSettingsAsync();
            using (SQLiteConnection conn = GetConnection())
            {
                await conn.OpenAsync();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    if (tempResponse.Value != null)
                    {
                        cmd.CommandText = "UPDATE UserSettings SET Subreddit = @subreddit, WallpaperFit = @wallpaperFit, Sorting = @sorting, Frequency_in_Hours = @frequency";
                    }
                    else
                    {
                        cmd.CommandText = "INSERT INTO UserSettings (Subreddit, WallpaperFit, Sorting, Frequency_in_Hours) VALUES (@subreddit, @wallpaperFit, @sorting, @frequency)";
                    }
                    cmd.Parameters.AddWithValue("@subreddit", settings.Subreddit);
                    cmd.Parameters.AddWithValue("@wallpaperFit", settings.WallpaperFit);
                    cmd.Parameters.AddWithValue("@sorting", settings.Sorting);
                    cmd.Parameters.AddWithValue("@frequency", settings.Frequency_in_Hours);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateFit(string fit)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                await conn.OpenAsync();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "UPDATE UserSettings SET WallpaperFit = @wallpaperFit";

                    cmd.Parameters.AddWithValue("@wallpaperFit", fit);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<ReturnObject<UserSettings>> GetUserSettingsAsync()
        {
            ReturnObject<UserSettings> response = new ReturnObject<UserSettings>();
            try
            {
                using (SQLiteConnection conn = GetConnection())
                {
                    await conn.OpenAsync();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = "SELECT Subreddit, WallpaperFit, Sorting, Frequency_in_Hours FROM UserSettings";
                        DbDataReader dr = await cmd.ExecuteReaderAsync();

                        DataTable dt = new DataTable();
                        dt.Load(dr);

                        if(dt.Rows.Count > 0)
                        {
                            response.Value = new UserSettings()
                            {
                                Frequency_in_Hours = Convert.ToInt32(dt.Rows[0]["Frequency_in_Hours"]),
                                Sorting = Convert.ToString(dt.Rows[0]["Sorting"]),
                                Subreddit = Convert.ToString(dt.Rows[0]["Subreddit"]),
                                WallpaperFit = Convert.ToString(dt.Rows[0]["WallpaperFit"])
                            };
                        }
                    }
                }
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Exception = ex;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task InitDatabaseAsync()
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            localPath += "/RedditWallpaperChanger/RedditWallpaperChanger/1.0.0.0";

            if (!File.Exists(localPath + "/Data/RedditWallpaperChanger.sqlite"))
            {
                CreateDatabase();
                await CreateTablesAsync();
            }

        }
    }
}
