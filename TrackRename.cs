using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private MusicBeeApiInterface mbApiInterface;
        private PluginInfo about = new PluginInfo();

        private string Menu0 = "context.Main/" + Properties.Resources.Menu0;
        private string Menu0_0 = "context.Main/" + Properties.Resources.Menu0 + "/" + Properties.Resources.Menu0_0;
        private string Menu0_1 = "context.Main/" + Properties.Resources.Menu0 + "/" + Properties.Resources.Menu0_1;
        private string Menu0_2 = "context.Main/" + Properties.Resources.Menu0 + "/" + Properties.Resources.Menu0_2;

        private Logger Mylog = null;

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            mbApiInterface = new MusicBeeApiInterface();
            mbApiInterface.Initialise(apiInterfacePtr);
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = Properties.Resources.MyName;
            about.Description = Properties.Resources.MyDescription;
            about.Author = Properties.Resources.Author;
            about.TargetApplication = "";   //  the name of a Plugin Storage device or panel header for a dockable panel
            about.Type = PluginType.General;
            about.VersionMajor = 1;  // your plugin version
            about.VersionMinor = 0;
            about.Revision = 1;
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            about.ConfigurationPanelHeight = 0;   // height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function

            mbApiInterface.MB_AddMenuItem( Menu0, null, null );
            mbApiInterface.MB_AddMenuItem( Menu0_0, null, Zenkaku2Hankaku );
            mbApiInterface.MB_AddMenuItem( Menu0_1, null, ImportTextFile );
            mbApiInterface.MB_AddMenuItem( Menu0_2, null, ImportTextFile );

            Mylog = Logger.GetInstance( mbApiInterface.Setting_GetPersistentStoragePath(), "TrackRename" );
            return about;
        }
        private bool IsPlay()
        {
            // 生成中または一時停止中は処理しない
            switch ( mbApiInterface.Player_GetPlayState() )
            {
                case PlayState.Playing:
                case PlayState.Paused:
                    // 再生・一時停止中は処理しない
                    return true;
                default:
                    break;
            }
            return false;
        }

        private void Zenkaku2Hankaku( object sender, EventArgs args )
        {
            if ( IsPlay() )
            {
                return;
            }

            // 選択中のものを取得
            if ( !mbApiInterface.Library_QueryFilesEx( "domain=SelectedFiles", out string[] files ) )
                files = new string[0];

            // 未選択の場合
            if ( files.Length == 0 )
            {
                return;
            }
            string at;
            string tt1;
            string tt2;
            bool refresh_f = false;
            foreach ( string file in files )
            {
                at = mbApiInterface.Library_GetFileTag( file, MetaDataType.Album );
                tt1 = mbApiInterface.Library_GetFileTag( file, MetaDataType.TrackTitle );

                tt2 = StrUtility.Zen2Han( tt1 );

                if ( tt2 != tt1 )
                {
                    mbApiInterface.Library_SetFileTag( file, MetaDataType.TrackTitle, tt2 );
                    mbApiInterface.Library_CommitTagsToFile( file );
                    // Log
                    string log_str = string.Format( "[{0}][{1}]=>[{2}]", at, tt1, tt2 );
                    Mylog.LogOutput( log_str );
                    refresh_f = true;

                }
                if ( refresh_f == true )
                {
                    mbApiInterface.MB_RefreshPanels();
                }

            }
        }
        private void ImportTextFile( object sender, EventArgs args )
        {
            if ( IsPlay() )
            {
                return;
            }

            string a = args.ToString();
            char delimiterChars;
            if ( a == Menu0_0 )
            {
                delimiterChars = ',';
            }
            else
            {
                delimiterChars = '\t';
            }

            string fname = SelectFile();
            if ( fname.Length != 0 )
            {
                StreamReader sr = new StreamReader( @fname, Encoding.GetEncoding("shift_jis") );
                List<string> keys = new List<string>();
                List<string> values = new List<string>();

                while ( !sr.EndOfStream )
                {
                    string line = sr.ReadLine();
                    string[] wk = line.Split( delimiterChars );
                    keys.Add( wk[0].Trim() );
                    values.Add( wk[1].Trim() );
                }
                // 選択中のものを取得
                if ( !mbApiInterface.Library_QueryFilesEx( "domain=SelectedFiles", out string[] files ) )
                    files = new string[0];

                // 未選択の場合
                if ( files.Length == 0 )
                {
                    return;
                }
                // 選択分の処理
                string at;
                string tt;
                bool refresh_f = false;
                foreach ( string file in files )
                {
                    at = mbApiInterface.Library_GetFileTag( file, MetaDataType.Album );
                    tt = mbApiInterface.Library_GetFileTag( file, MetaDataType.TrackTitle );
                    // キーで検索
                    for ( int i = 0; i < keys.Count; i++ )
                    {
                        if ( tt == keys[i] && values[i].Length != 0 )
                        {
                            mbApiInterface.Library_SetFileTag( file, MetaDataType.TrackTitle, values[i] );
                            mbApiInterface.Library_CommitTagsToFile( file );
                            // Log
                            string log_str = string.Format( "[{0}][{1}]=>[{2}]", at, tt, values[i] );
                            Mylog.LogOutput( log_str );
                            refresh_f = true;
                            break;
                        }
                    }

                }
                if ( refresh_f == true )
                {
                    mbApiInterface.MB_RefreshPanels();
                }
            }
        }
        private string SelectFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "*.*";
            ofd.Filter = Properties.Resources.Filter + "(*.*)|*.*";
            ofd.FilterIndex = 2;
            ofd.Title = Properties.Resources.OpenFileTitle;
            ofd.RestoreDirectory = true;

            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                return ( ofd.FileName );
            }
            return ("");
        }
        public bool Configure(IntPtr panelHandle)
        {
            // save any persistent settings in a sub-folder of this path
            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
            // panelHandle will only be set if you set about.ConfigurationPanelHeight to a non-zero value
            // keep in mind the panel width is scaled according to the font the user has selected
            // if about.ConfigurationPanelHeight is set to 0, you can display your own popup window
            if (panelHandle != IntPtr.Zero)
            {
                Panel configPanel = (Panel)Panel.FromHandle(panelHandle);
                Label prompt = new Label();
                prompt.AutoSize = true;
                prompt.Location = new Point(0, 0);
                prompt.Text = "prompt:";
                TextBox textBox = new TextBox();
                textBox.Bounds = new Rectangle(60, 0, 100, textBox.Height);
                configPanel.Controls.AddRange(new Control[] { prompt, textBox });
            }
            return false;
        }
       
        // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        // its up to you to figure out whether anything has changed and needs updating
        public void SaveSettings()
        {
            // save any persistent settings in a sub-folder of this path
            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
        }

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        public void Close(PluginCloseReason reason)
        {
        }

        // uninstall this plugin - clean up any persisted files
        public void Uninstall()
        {
        }

        // receive event notifications from MusicBee
        // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            // perform some action depending on the notification type
            switch (type)
            {
                case NotificationType.PluginStartup:
                    // perform startup initialisation
                    switch (mbApiInterface.Player_GetPlayState())
                    {
                        case PlayState.Playing:
                        case PlayState.Paused:
                            // ...
                            break;
                    }
                    break;
                case NotificationType.TrackChanged:
                    string artist = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist);
                    // ...
                    break;
            }
        }

        // return an array of lyric or artwork provider names this plugin supports
        // the providers will be iterated through one by one and passed to the RetrieveLyrics/ RetrieveArtwork function in order set by the user in the MusicBee Tags(2) preferences screen until a match is found
        //public string[] GetProviders()
        //{
        //    return null;
        //}

        // return lyrics for the requested artist/title from the requested provider
        // only required if PluginType = LyricsRetrieval
        // return null if no lyrics are found
        //public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred, string provider)
        //{
        //    return null;
        //}

        // return Base64 string representation of the artwork binary data from the requested provider
        // only required if PluginType = ArtworkRetrieval
        // return null if no artwork is found
        //public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
        //{
        //    //Return Convert.ToBase64String(artworkBinaryData)
        //    return null;
        //}

        //  presence of this function indicates to MusicBee that this plugin has a dockable panel. MusicBee will create the control and pass it as the panel parameter
        //  you can add your own controls to the panel if needed
        //  you can control the scrollable area of the panel using the mbApiInterface.MB_SetPanelScrollableArea function
        //  to set a MusicBee header for the panel, set about.TargetApplication in the Initialise function above to the panel header text
        //public int OnDockablePanelCreated(Control panel)
        //{
        //  //    return the height of the panel and perform any initialisation here
        //  //    MusicBee will call panel.Dispose() when the user removes this panel from the layout configuration
        //  //    < 0 indicates to MusicBee this control is resizable and should be sized to fill the panel it is docked to in MusicBee
        //  //    = 0 indicates to MusicBee this control resizeable
        //  //    > 0 indicates to MusicBee the fixed height for the control.Note it is recommended you scale the height for high DPI screens(create a graphics object and get the DpiY value)
        //    float dpiScaling = 0;
        //    using (Graphics g = panel.CreateGraphics())
        //    {
        //        dpiScaling = g.DpiY / 96f;
        //    }
        //    panel.Paint += panel_Paint;
        //    return Convert.ToInt32(100 * dpiScaling);
        //}

        // presence of this function indicates to MusicBee that the dockable panel created above will show menu items when the panel header is clicked
        // return the list of ToolStripMenuItems that will be displayed
        //public List<ToolStripItem> GetHeaderMenuItems()
        //{
        //    List<ToolStripItem> list = new List<ToolStripItem>();
        //    list.Add(new ToolStripMenuItem("A menu item"));
        //    return list;
        //}

        //private void panel_Paint(object sender, PaintEventArgs e)
        //{
        //    e.Graphics.Clear(Color.Red);
        //    TextRenderer.DrawText(e.Graphics, "hello", SystemFonts.CaptionFont, new Point(10, 10), Color.Blue);
        //}

    }
}