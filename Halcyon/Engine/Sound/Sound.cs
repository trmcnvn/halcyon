//-----------------------------------------------------------------------------
// Sound.cs
//
// Manager of all sounds, Play/Pause/Stop functionality
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Text;

namespace Halcyon {
    public static class Sound {
    #region Public Variables
        //-----------------------------------------------------------------------------
        // Volume
        //
        // Changes the volume of the music
        //-----------------------------------------------------------------------------
        public static float Volume {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = value; }
        }
        public static float OriginalVolume { get; set; }
    #endregion
    #region Public Methods
        //-----------------------------------------------------------------------------
        // Play
        //
        // Plays the song that we loaded when constructing the class
        //-----------------------------------------------------------------------------
        public static void Play() {
            if ( currentSong == null ) 
                return;

            MediaPlayer.Play( currentSong );
        }
        //-----------------------------------------------------------------------------
        // Stop
        //
        // Stops the song that we loaded when constructing the class
        //-----------------------------------------------------------------------------
        public static void Stop() {
            if ( currentSong == null )
                return;

            MediaPlayer.Stop();
        }
        //-----------------------------------------------------------------------------
        // Pause
        //
        // Pauses the song that is currently playing
        //-----------------------------------------------------------------------------
        public static void Pause() {
            if ( currentSong == null )
                return;

            MediaPlayer.Pause();
        }
        //-----------------------------------------------------------------------------
        // Resume
        //
        // Resumes the song that is currently playing
        //-----------------------------------------------------------------------------
        public static void Resume() {
            if ( currentSong == null )
                return;

            MediaPlayer.Resume();
        }
        //-----------------------------------------------------------------------------
        // SetLooping
        //
        // Sets the looping flag for the current song
        //-----------------------------------------------------------------------------
        public static void SetLooping( bool state ) {
            if ( currentSong == null )
                return;

            MediaPlayer.IsRepeating = state;
        }
        //-----------------------------------------------------------------------------
        // SetSong
        //
        // Sets the current song to play
        //-----------------------------------------------------------------------------
        public static void SetSong( Song newSong ) {
            currentSong = newSong;
        }
        //-----------------------------------------------------------------------------
        // IsPlaying
        //
        // Checks to see if the current song is in play
        //-----------------------------------------------------------------------------
        public static bool IsPlaying() {
            if ( currentSong == null )
                return false;

            if ( MediaPlayer.State == MediaState.Playing )
                return true;
            else
                return false;
        }
        //-----------------------------------------------------------------------------
        // IsStopped
        //
        // Checks to see if the current song is stopped
        //-----------------------------------------------------------------------------
        public static bool IsStopped() {
            if ( currentSong == null )
                return false;

            if ( MediaPlayer.State == MediaState.Stopped )
                return true;
            else
                return false;
        }
        //-----------------------------------------------------------------------------
        // IsPaused
        //
        // Checks to see if the current song is paused
        //-----------------------------------------------------------------------------
        public static bool IsPaused() {
            if ( currentSong == null )
                return false;

            if ( MediaPlayer.State == MediaState.Paused )
                return true;
            else
                return false;
        }
        //-----------------------------------------------------------------------------
        // IsLooped
        //
        // Checks to see if the current song is set for looping
        //-----------------------------------------------------------------------------
        public static bool IsLooped() {
            if ( currentSong == null )
                return false;

            return MediaPlayer.IsRepeating;
        }
            
    #endregion // Public Methods

    #region Private Variables
        private static Song currentSong;
    #endregion // Private Variables
    }
}
