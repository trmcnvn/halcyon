//-----------------------------------------------------------------------------
// Highscores.cs
//-----------------------------------------------------------------------------
using System;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.Phone.Info;
using Newtonsoft.Json;
using com.addictionsoftware;

namespace Halcyon
{
    public class Highscores
    {
        /// <summary>
        /// Storage structure
        /// </summary>
        public class highScoreData
        {
            public string uID { get; set; }
            public string Name { get; set; }
            public string Score { get; set; }
            public string Timestamp { get; set; }
        }

        /// <summary>
        /// List of results
        /// </summary>
        private List<highScoreData> data;
        public IList<highScoreData> Data
        {
            get { return data; }
        }

        /// <summary>
        /// Used to store data that we get using Fetch(), Async FUN!
        /// </summary>
        private List<highScoreData> userData =
            new List<highScoreData>();
        public IList<highScoreData> UserData
        {
            get { return userData; }
        }

        /// <summary>
        /// Is the download of the top 10 complete, Async calls are fun :D
        /// </summary>
        private bool downloadTop10Complete = false;
        public bool DownloadTop10Complete
        {
            get { return downloadTop10Complete; }
            set { downloadTop10Complete = value; }
        }

        /// <summary>
        /// Have we stored the data that we got from Fetch()? Used by third party classes, ASYNC FUN
        /// </summary>
        private bool downloadFetchComplete = false;
        public bool DownloadFetchComplete
        {
            get { return downloadFetchComplete; }
            set { downloadFetchComplete = value; }
        }

        public bool Refreshing = false;

        /// <summary>
        /// ASYNC :):):):):):)
        /// </summary>
        private bool uIDExists = false;
        public bool UIDExists
        {
            get { return uIDExists; }
        }

        public WebClient connect;

        string HashFunc(string x)
        {
            SHA256 shaFunc = new SHA256Managed();
            byte[] d = Encoding.UTF8.GetBytes(x);
            d = shaFunc.ComputeHash(d);

            string output = "";
            for (int i = 0; i < d.Length; i++)
                output += d[i].ToString("x2").ToLower();

            return output;
        }

        /// <summary>
        /// Initiate the download of the top 10 high scores
        /// </summary>
        public void DownloadTop10()
        {
            // Connect to server and request top 10
            Network.GetRequest( "hs.php?game=" + Network.GetGame() + "&req=get&format=top10&uID=" + Network.GetWindowsLive() + "&ignore=" + DateTime.Now, ( s, e ) => {
                // Set returned data to our list
                data = new List<highScoreData>();
                data = JsonConvert.DeserializeObject<List<highScoreData>>( e.Result );
                // Set download to true
                downloadTop10Complete = true;
            } );
        }

        /// <summary>
        /// Check to see if this uID exists in the DB
        /// </summary>
        public void CheckuID()
        {
            Network.GetRequest( "hs.php?game=" + Network.GetGame() + "&req=get&format=check&uID=" + Network.GetWindowsLive() + "&Token=" + GenerateToken() + "&ignore=" + DateTime.Now, ( s, e ) =>
            {
                
                if ( e.Result.Substring( 0, 4 ).CompareTo( "true" ) == 0 )
                    uIDExists = true;
                else
                    uIDExists = false;
            } );
        }

        /// <summary>
        /// Update a users name in the db
        /// </summary>
        public void UpdateName(string name)
        {
            Network.PostRequest( "hs.php?game=" + Network.GetGame() + "&req=updatename&uID=" + Network.GetWindowsLive() + "&Name=" + name + "&Token=" + GenerateToken() + "&ignore=" + DateTime.Now, 
                ( s, e ) =>
                    {
                        // Error?
                        if ( e.Result.Substring( 0, 3 ).CompareTo( "err" ) == 0 ) {
                            string err = e.Result.Substring( 6 );
                            // Email this off to us? err =  mysql_errno()
                        }
                    } );
        }

        /// <summary>
        /// Fetch the user's personal data
        /// </summary>
        public void Fetch()
        {
            // Connect to server and fetch user info
            Network.GetRequest( "hs.php?game=" + Network.GetGame() + "&req=get&format=fetch&uID=" + Network.GetWindowsLive() + "&Token=" + GenerateToken() + "&ignore=" + DateTime.Now, ( s, e ) =>
            {
                if ( e.Result.Contains( "uID was not found in the database" ) )
                    return;

                // Store data, Using a list here -_-;
                userData.Clear();
                userData = JsonConvert.DeserializeObject<List<highScoreData>>( e.Result );
                // Set download to true
                downloadFetchComplete = true;
                Refreshing = false;
            } );
        }

        /// <summary>
        /// Generate a token
        /// TODO: better generation of token
        /// </summary>
        public string GenerateToken()
        {
            return HashFunc( Network.GetWindowsLive() );
        }

        /// <summary>
        /// Submit a new high score!
        /// </summary>
        public void SubmitScore(string name, string score)
        {
            // Hash the data together
            string hashString = HashFunc( name + Network.GetWindowsLive() + score + GenerateToken() + "lolinternets" );
            Network.PostRequest( "hs.php?game=" + Network.GetGame() + "&req=add&uID=" + Network.GetWindowsLive() + "&Name=" + name + "&Score=" + score + "&Token=" + GenerateToken() + "&Hash=" + hashString + "&ignore=" + DateTime.Now, null );
        }
    }
}
