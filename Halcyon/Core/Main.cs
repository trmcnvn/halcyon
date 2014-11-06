//-----------------------------------------------------------------------------
// Main.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Globalization;

namespace Halcyon
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Halcyon_WP7 : Microsoft.Xna.Framework.Game
    {
        // Used for Graphics
        GraphicsDeviceManager graphics;

        // Screen manager
        ScreenManager screenManager;

        // Screen Factory
        ScreenFactory screenFactory;

        // Game Ptr
        public static Microsoft.Xna.Framework.Game gamePtr { get; protected set; }

        public bool isPreserved = false;

        public Halcyon_WP7() {
            graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";

            // Ptr
            gamePtr = this;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks( 333333 );
            // Set game to full screen
            graphics.IsFullScreen = true;
            // Set Portrait mode
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;

            // Initialize Accelerometer
            Accelerometer.Initialize();

            // Create the screen factory and add it to the Services
            screenFactory = new ScreenFactory();
            Services.AddService( typeof( IScreenFactory ), screenFactory );

            // Set up Screen Manager
            screenManager = new ScreenManager( this );

            Components.Add( screenManager );

            // Hook event handlers to watch for game state 
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Launching +=
                new EventHandler<Microsoft.Phone.Shell.LaunchingEventArgs>( GameIsLaunching );
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Activated +=
                new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>( GameHasActivated );
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Deactivated +=
                new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>( GameHasDeactivated );

            // Catch Exceptions
            System.Windows.Application.Current.UnhandledException += ( s, e ) => {
                com.addictionsoftware.ExceptionHandler exception = new com.addictionsoftware.ExceptionHandler( e.ExceptionObject as Exception );
                exception.SaveInformation();
                exception.HandleForClient( this );
            };
        }

        protected override void EndRun() {
            // Return original volume
            if ( Sound.IsPlaying() ) {
                Sound.Stop();
            }
            Sound.Volume = Sound.OriginalVolume;

            base.EndRun();
        }

        protected override void LoadContent()
        {
            Sound.SetSong( Content.Load<Song>( "Sound/background" ) );

            if ( com.addictionsoftware.ExceptionFile.FileCheck() ) {
                com.addictionsoftware.ExceptionFile.SendFile();
                com.addictionsoftware.ExceptionFile.DeleteFile();
            }

            using ( IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication() ) {
                if ( store.FileExists( "Halcyon.dat" ) == false ) {
                    store.CreateFile( "Halcyon.dat" );
                }
            }

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if ( settings.Contains( "Music" ) == false )
                settings.Add( "Music", true );

            if ( settings.Contains( "SoundFX" ) == false )
                settings.Add( "SoundFX", true );

            if ( settings.Contains( "MusicVolume" ) == false )
                settings.Add( "MusicVolume", 1.0f );

            if ( settings.Contains( "SoundFXVolume" ) == false )
                settings.Add( "SoundFXVolume", 0.65f );

            if ( settings.Contains( "Vibrate" ) == false )
                settings.Add( "Vibrate", true );

            settings.Save();

            // Save original volume
            Sound.OriginalVolume = Sound.Volume;

            if ( MediaPlayer.GameHasControl == false ) {
                settings[ "Music" ] = false;
                settings.Save();

                System.Windows.MessageBoxResult res = System.Windows.MessageBox.Show( 
                    "You are currently playing music, Would you like to stop it?", "Music",
                    System.Windows.MessageBoxButton.OKCancel );

                if ( res == System.Windows.MessageBoxResult.OK ) {
                    // Stop Music
                    MediaPlayer.Stop();

                    settings[ "Music" ] = true;
                    settings.Save();
                }
            }

            if ( bool.Parse( settings[ "Music" ].ToString() ) ) {
                // Play our background music
                Sound.Play();
                Sound.Volume = float.Parse( settings[ "MusicVolume" ].ToString() );
                Sound.SetLooping( true );
            }

            // Tracking
            com.addictionsoftware.Tracking.TrackInstall();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        // Create and activate background and main screen
        private void AddInitialScreens()
        {
            screenManager.Add(new Background());
            screenManager.Add(new MainMenu());
        }

        // Event hook when game is launched for the first time
        void GameIsLaunching(object sender, Microsoft.Phone.Shell.LaunchingEventArgs e)
        {
            AddInitialScreens();
        }

        // Event hook when game is activated (eg after moving to another app and back to it used for tombstoning)
        void GameHasActivated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            Type type = typeof(Microsoft.Phone.Shell.ActivatedEventArgs);
            System.Reflection.PropertyInfo property = type.GetProperty("IsApplicationInstancePreserved");
            if (property != null)
                isPreserved = (bool)property.GetValue(e, null);
            // Try to deserialize the screen manager
            if (!screenManager.Activate(isPreserved))
            {
                // If the screen manager fails to deserialize, add the initial screens
                AddInitialScreens();
            }

            if ( Game.current != null ) {
                // If there is a game active show pause menu
                if ( Game.current.IsActive && Game.current.isPaused != true) {
                    Game.current.isPaused = true;
                    screenManager.Add( new Paused() );
                }
            }
        }
        // Event hook when game is deactivated and no longer in the foreground
        void GameHasDeactivated(object sender, Microsoft.Phone.Shell.DeactivatedEventArgs e)
        {
            // Serialize the screen manager when the game deactivated
            screenManager.Deactivate();
        }
    }
}
