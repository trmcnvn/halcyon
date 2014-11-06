/*
 * Tracking.cs
 * 
 * Tracking function for data!
 * 
 * addictionsoftware 2011
 */
using System;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;

namespace com.addictionsoftware {
    class TrackingData : Dictionary<string, object> {
        /// <summary>
        /// Token for MixPanel Project
        /// </summary>
        private const string token = "";
        /// <summary>
        /// Name of the tracking data
        /// </summary>
        private readonly string trackingName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="trackingName"></param>
        public TrackingData( string trackingName ) {
            this.trackingName = trackingName;
        }

        /// <summary>
        /// Adds the object to the key for tracking
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public string Get() {
            // Is the token added?
            if ( !this.ContainsKey( "token" ) )
                this.Add( "token", token );
            // Is the distinct id added?
            if ( !this.ContainsKey( "distinct_id" ) )
                this.Add( "distinct_id", Network.GetWindowsLive() );

            // Base64 encode
            return Convert.ToBase64String( Encoding.UTF8.GetBytes( new JObject( new object[ 2 ] {
                new JProperty("event", trackingName),
                new JProperty("properties", new JObject(Enumerable.ToArray<JProperty>(Enumerable.Select<KeyValuePair<string, object>, JProperty>((
                    IEnumerable<KeyValuePair<string, object>>) this, (Func<KeyValuePair<string, object>, JProperty>)(KeyValuePair => new JProperty(
                        KeyValuePair.Key, KeyValuePair.Value.ToString()))))))
            } ).ToString() ) );
        }
    }

    static class Tracking {
        /// <summary>
        /// Start tracking the data
        /// </summary>
        /// <param name="data"></param>
        private static void Send( TrackingData data ) {
            // Talk tot he web sever
            WebRequest.Create( new UriBuilder( "http://api.mixpanel.com/track" ) {
                Query = string.Format( ( IFormatProvider )CultureInfo.InvariantCulture, "data={0}", new object[ 1 ] {
                    data.Get()
                } )
            }.Uri ).BeginGetResponse( new AsyncCallback( ( a ) => { } ), null );
            
        }

        /// <summary>
        /// Track the users who launch the game
        /// </summary>
        public static void TrackInstall() {
            TrackingData data = new TrackingData( "Install" );
            data.Add( "Locale", Thread.CurrentThread.CurrentCulture.Name );
            data.Add( "Game", Network.GetGame() );
            data.Add( "DeviceName", Network.GetDevice() );
            data.Add( "DeviceManufacturer", Network.GetManufacturer() );
            Send( data );
        }
    }
}
