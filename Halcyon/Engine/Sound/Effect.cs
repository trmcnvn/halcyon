//-----------------------------------------------------------------------------
// Effect.cs
//
// Manager of all sound effects, Play/Pause/Stop functionality
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using System.IO.IsolatedStorage;
using System.Text;

namespace Halcyon {
    class Effect {
    #region Public Variables
        //-----------------------------------------------------------------------------
        // Volume
        //
        // Changes the volume of the music
        //-----------------------------------------------------------------------------
    #endregion
    #region Public Methods
        //-----------------------------------------------------------------------------
        // Sound
        //
        // Constructor, Constructs a new instance of the class Sound
        //-----------------------------------------------------------------------------
        public Effect( SoundEffect songLoaded ) {
            currentSong = songLoaded;
        }
        //-----------------------------------------------------------------------------
        // Play
        //
        // Plays the song that we loaded when constructing the class
        //-----------------------------------------------------------------------------
        public void Play() {
            if ( currentSong == null ) 
                return;
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if ( bool.Parse( settings[ "SoundFX" ].ToString() ) == true ) {
                // Can't use currentSongInstance, Otherwise we can't play multiple sounds at once ;]??
                currentSong.Play();
            }
        }
    #endregion // Public Methods

    #region Private Variables
        private SoundEffect currentSong;
    #endregion // Private Variables
    }
}
