<p>
    The solution comprises of 3 projects:<br />
    <ol>
        <li>UI - A WPF application with a User Interface</li>
        <li>Notification - A Windows Forms application for running in the System Tray</li>
        <li>Action - A class library project comprising of all the logic and database functions</li>
    </ol>
</p>
<p>
    The data used for this application is stored in the User's AppData folder: <br />
    <code>
        AppData\Local\RedditWallpaperChanger\RedditWallpaperChanger\1.0.0.0
    </code>
    <br />
    The Wallpaper folder is a temporary storage for the currently applied wallpaper.<br />
    The Data folder stores the Sqlite which stores the user's settings (Changed in the UI).<br /><br />
    The application additionally uses the regedit folder to control the wallpaper's settings.
</p>
<p>
    If you want to use the application, kindly go to <code>Notification.cs</code> file and change the <code>basePath</code> field to the same path as the solution.
</p>
