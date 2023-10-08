using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MusicBeePlugin
{
    class Logger
    {
        private static Logger singleton = null;
        private static int LOG_RETENTION_PERIOD = 30;
        private string LogFilePath = null;
        private string LogFileFileBase = null;
        private string NowLogFileName = null;

        private static string LOG_FILE_EXT = ".log";

        private object LockObj = new object();
        private StreamWriter stream = null;

        public static Logger GetInstance(string log_file_path, string log_file_base )
        {
            if ( singleton == null )
            {
                singleton = new Logger( log_file_path, log_file_base );
            }
            return singleton;
        }
        private Logger( string log_file_path, string log_file_base )
        {
            // ログファイルの保存パス
            this.LogFilePath = log_file_path;
            this.LogFileFileBase = log_file_base;

            // ログファイル名生成
            string fname = string.Format( "{0}_{1}{2}", this.LogFileFileBase, DateTime.Now.ToString( "yyyy-MM-dd" ), LOG_FILE_EXT );
            this.NowLogFileName = System.IO.Path.Combine( this.LogFilePath, fname );

            // ログファイル作成
            CreateLogfile( new FileInfo( this.NowLogFileName ) );
        }
        private void CreateLogfile( FileInfo logFile )
        {
            if ( !Directory.Exists( logFile.DirectoryName ) )
            {
                Directory.CreateDirectory( logFile.DirectoryName );
            }

            this.stream = new StreamWriter( logFile.FullName, true, Encoding.GetEncoding( "shift_jis" ) )
            {
                AutoFlush = true
            };
        }
        public void LogOutput( string msg )
        {
            // ログファイル名生成
            string fname = string.Format( "{0}_{1}{2}", this.LogFileFileBase, DateTime.Now.ToString( "yyyy-MM-dd" ), LOG_FILE_EXT );
            string cmp_fname = System.IO.Path.Combine( this.LogFilePath, fname );
            if ( this.NowLogFileName != cmp_fname )
            {
                this.stream.Close();
                DeleteOldLogFile();
                this.NowLogFileName = cmp_fname;
                CreateLogfile( new FileInfo( this.NowLogFileName ) );
            }
            // ログ出力
            string fullMsg = string.Format( "{0} {1}", DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fff" ), msg );
            lock ( this.LockObj )
            {
                this.stream.WriteLine( fullMsg );
            }
        }
        private void DeleteOldLogFile()
        {
            string fname_pattern = string.Format( "{0}_*{1}", this.LogFileFileBase, LOG_FILE_EXT );

            // LOG_RETENTION_PERIOD日前の日付
            DateTime retentionDate = DateTime.Today.AddDays( -LOG_RETENTION_PERIOD );

            string[] filePathList = Directory.GetFiles( this.LogFilePath, fname_pattern, SearchOption.TopDirectoryOnly );
            foreach ( string filePath in filePathList )
            {
                string work;
                work = filePath.Replace( this.LogFilePath, "" );
                work = work.Replace( this.LogFileFileBase + "_", "" );
                work = work.Replace(LOG_FILE_EXT, "" );
                DateTime logCreatedDate = DateTime.ParseExact( work, "yyyyMMdd", null );
                if ( logCreatedDate < retentionDate )
                {
                    File.Delete( filePath );
                }
            }
        }
    }
}
