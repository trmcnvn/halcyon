/*
 * Network.cs
 * 
 * General networking functions
 */
using System;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Phone.Info;

namespace com.addictionsoftware {
    static class Network {
        /// <summary>
        /// Information for Windows LiveID
        /// </summary>
        private static readonly int liveLength = 32;
        private static readonly int liveOffset = 2;
        /// <summary>
        /// Name of the game
        /// </summary>
        private static readonly string gameName = "Halcyon";
        /// <summary>
        /// Status of connection with the servers
        /// </summary>
        private static bool serverContact = true;

        /// <summary>
        /// Checks to see if we have internet connectivity
        /// </summary>
        /// <returns></returns>
        public static bool HasConnection() {
            return NetworkInterface.GetIsNetworkAvailable();
        }

        /// <summary>
        /// Returns the state of the communication between the application and the servers
        /// </summary>
        /// <returns></returns>
        public static bool HasCommunication() {
            return serverContact;
        }

        /// <summary>
        /// Returns the name of this specific game
        /// </summary>
        /// <returns></returns>
        public static string GetGame() {
            return gameName;
        }

        /// <summary>
        /// Returns the name of the windows device
        /// </summary>
        /// <returns>XDeviceEmulator</returns>
        public static string GetDevice() {
            object deviceName;
            if ( DeviceExtendedProperties.TryGetValue( "DeviceName", out deviceName ) )
                return deviceName.ToString();

            return "";
        }

        /// <summary>
        /// Returns the manufacturer of the device
        /// </summary>
        /// <returns>Microsoft</returns>
        public static string GetManufacturer() {
            object manufacturerName;
            if ( DeviceExtendedProperties.TryGetValue( "DeviceManufacturer", out manufacturerName ) )
                return manufacturerName.ToString();

            return "";
        }

        /// <summary>
        /// Returns the anonymous windows live ID
        /// </summary>
        /// <returns></returns>
        public static string GetWindowsLive() {
            object liveID;
            if ( Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Device ) {
                if ( UserExtendedProperties.TryGetValue( "ANID", out liveID ) )
                    if ( liveID != null && liveID.ToString().Length >= ( liveLength + liveOffset ) )
                        return liveID.ToString().Substring( liveOffset, liveLength );
            } else
                return "31337";        

            return "";
        }

        /// <summary>
        /// Callback delegate for WebClient functions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void NetworkCallback( object sender, DownloadStringCompletedEventArgs e );

        /// <summary>
        /// Send a POST request to the webserver
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="callbackFunction"></param>
        public static void PostRequest( string request, NetworkCallback callbackFunction ) {
            if ( !HasConnection() )
                return;

            WebClient server = new WebClient();
            server.DownloadStringAsync( new Uri( "http://www.addictionsoftware.com/pages/" + request ) );
            server.DownloadStringCompleted += (s, e) => {
                if ( e.Error == null ) {
                    serverContact = true;
                    // MySQL server available?
                    if ( e.Result.Contains( "Connection Unavailable" ) )
                    {
                        serverContact = false;
                        return;
                    } else if ( callbackFunction != null )
                        callbackFunction( s, e );
                } else {
                    serverContact = false;
                }
            };
        }

        /// <summary>
        /// Send a GET Request to the webserver
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callbackFunction"></param>
        public static void GetRequest( string request, NetworkCallback callbackFunction ) {
            PostRequest( request, callbackFunction );
        }
    }
}
