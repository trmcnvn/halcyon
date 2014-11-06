/*
 * ExceptionHandler.cs
 * 
 * Displays a message to the client and sends exception information to a web server.
 * 
 */
using System;
using System.Windows;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;

namespace com.addictionsoftware {
    static class ExceptionFile {
        /// <summary>
        /// Checks if the exception file exists
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool FileCheck() {
            using ( IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication() ) {
                if ( file.FileExists( Network.GetGame() + "Error.dat" ) == true ) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sends the information in the file off to the web server
        /// </summary>
        public static void SendFile() {
            // Remember to delete file after use.
            string fileContents;
            string exceptionMessage;
            string exceptionStack;

            // Open file and get content
            using ( IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication() ) {
                using ( IsolatedStorageFileStream stream = file.OpenFile( Network.GetGame() + "Error.dat", FileMode.Open ) ) {
                    StreamReader read = new StreamReader( stream );
                    fileContents = read.ReadToEnd();
                    read.Close();

                    // Parse the contents
                    exceptionMessage = fileContents.Substring( 0, fileContents.IndexOf( '~' ) );
                    exceptionStack = fileContents.Substring( fileContents.IndexOf( '~' ), fileContents.Length - fileContents.IndexOf( '~' ) );
                    exceptionStack = exceptionStack.Replace( "   ", Uri.EscapeDataString( "\n" ) );

                    // Send Information
                    string request = "ex.php?game=" + Network.GetGame() + "&dev=" + Network.GetDevice() + "&msg=" + exceptionMessage + "&stack=" + exceptionStack + "&ignore=" + DateTime.Now;
                    Network.PostRequest( request, null );
                }
            }
        }

        /// <summary>
        /// Deletes the exception file
        /// </summary>
        public static void DeleteFile() {
            // Delete file
            using ( IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication() ) {
                if ( file.FileExists( Network.GetGame() + "Error.dat" ) )
                    file.DeleteFile( Network.GetGame() + "Error.dat" );
            }
        }
    }

    class ExceptionHandler {
        /// <summary>
        /// The exception for this class to handle
        /// </summary>
        private Exception exception;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exception"></param>
        public ExceptionHandler( Exception exception ) {
            this.exception = exception;
        }

        /// <summary>
        /// Displays a message to the client that we must close.
        /// </summary>
        public void HandleForClient( Game game ) {
            // Create message box for the user
            MessageBox.Show( "The game had an unexpected error and has to be shut down.\n\nInformation about this crash has been sent to the developers so we can fix it!\n\n" +
                "We're sorry for the inconveience.", "Ooops! Unexpected Error", MessageBoxButton.OK );

            // Exit the game
            game.Exit();
        }

        /// <summary>
        /// Save the exception/crash information to the disk so we can send it to the developers at next launch
        /// </summary>
        public void SaveInformation() {
            // Create a file on the disk
            using ( IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication() ) {
                if ( file.FileExists( Network.GetGame() + "Error.dat" ) == false ) {
                    using ( IsolatedStorageFileStream stream = file.OpenFile( Network.GetGame() + "Error.dat", FileMode.OpenOrCreate ) ) {
                        // Write the exception information to the file
                        StreamWriter write = new StreamWriter( stream );
                        write.Write( string.Format( "{0}~{1}", exception.Message, exception.StackTrace ) );
                        write.Close();
                    }
                }
            }
        }
    }
}
