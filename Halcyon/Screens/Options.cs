//-----------------------------------------------------------------------------
// GameOver.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO.IsolatedStorage;

namespace Halcyon {
    /// <summary>
    /// Game has ended!
    /// </summary>
    class Options : MenuScreen {
        MenuEntry musicEntry;
        MenuEntry fxEntry;
        MenuEntry vibrateEntry;

        Texture2D sliderStrip;
        Button musicVolume;
        Button soundFXVolume;

        /// <summary>
        /// Construct a options screen
        /// </summary>
        public Options()
            : base( "Options" ) {

            entryPosition = 300.0f;
            menuEntryPadding = 8;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            // Create Entries
            musicEntry = new MenuEntry( "Music:", 0.65f );
            musicEntry.Text += ( bool.Parse( settings[ "Music" ].ToString() ) == true ) ? " On" : " Off";
            musicEntry.menuColor = ( bool.Parse( settings[ "Music" ].ToString() ) == true ) ? Color.LightGreen : Color.Red;

            fxEntry = new MenuEntry( "Sound Fx:", 0.65f );
            fxEntry.Text += ( bool.Parse( settings[ "SoundFX" ].ToString() ) == true ) ? " On" : " Off";
            fxEntry.menuColor = ( bool.Parse( settings[ "SoundFX" ].ToString() ) == true ) ? Color.LightGreen : Color.Red;

            vibrateEntry = new MenuEntry( "Vibrate: ", 0.65f );
            vibrateEntry.Text += ( bool.Parse( settings[ "Vibrate" ].ToString() ) == true ) ? " On" : " Off";
            vibrateEntry.menuColor = ( bool.Parse( settings[ "Vibrate" ].ToString() ) == true ) ? Color.LightGreen : Color.Red;

            musicEntry.Selected += MusicSelected;
            fxEntry.Selected += SoundFXSelected;
            vibrateEntry.Selected += VibrateSelected;

            // Add Entries to list!
            MenuEntries.Add( musicEntry );
            MenuEntries.Add( fxEntry );
            MenuEntries.Add( vibrateEntry );
        }

        public override void LoadContent() {
            base.LoadContent();

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            sliderStrip = content.Load<Texture2D>( "Menu/sliderStrip" );

            musicVolume = new Button( @"Menu/sliderHandle" );
            musicVolume.LoadContent( content, ScreenManager.GraphicsDevice );

            musicVolume.PositionOrigin = musicVolume.TextureCenter;
            musicVolume.PositionOfOrigin = new Vector2( 
                ( float.Parse(settings["MusicVolume"].ToString()) * 300 ) + 99, 601 );
            musicVolume.DragRestrictions = new Rectangle( 99,
                ( int )musicVolume.PositionOfOrigin.Y, 300, 0 );
            musicVolume.PositionChanged += musicVolumeChanged;
            musicVolume.AllowDrag = true;


            soundFXVolume = new Button( @"Menu/sliderHandle" );
            soundFXVolume.LoadContent( content, ScreenManager.GraphicsDevice );

            soundFXVolume.PositionOrigin = soundFXVolume.TextureCenter;
            soundFXVolume.PositionOfOrigin = new Vector2( 
                ( float.Parse(settings["SoundFXVolume"].ToString()) * 300 ) + 99, 751 );
            soundFXVolume.DragRestrictions = new Rectangle( 99,
                ( int )soundFXVolume.PositionOfOrigin.Y, 300, 0 );
            soundFXVolume.PositionChanged += soundFXVolumeChanged;
            soundFXVolume.AllowDrag = true;
        }

        void musicVolumeChanged( object sender, EventArgs a ) {
            Button handle = sender as Button;
            float scaledValue = ( handle.PositionOfOrigin.X - ( float )handle.DragRestrictions.Left ) /
                ( float )handle.DragRestrictions.Width;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if ( Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Device ) {
                Sound.Volume = scaledValue;
                settings[ "MusicVolume" ] = scaledValue;
                settings.Save();
            } else {
                Sound.Volume = MathHelper.Clamp( scaledValue, 0.000001f, 1.0f );
                settings[ "MusicVolume" ] = MathHelper.Clamp( scaledValue, 0.000001f, 1.0f );
                settings.Save();
            }
        }

        void soundFXVolumeChanged( object sender, EventArgs a ) {
            Button handle = sender as Button;
            float scaledValue = ( handle.PositionOfOrigin.X - ( float )handle.DragRestrictions.Left ) /
                ( float )handle.DragRestrictions.Width;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            SoundEffect.MasterVolume = scaledValue;

            settings[ "SoundFXVolume" ] = scaledValue;
            settings.Save();
        }

        void MusicSelected( object sender, EventArgs a ) {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            // Stop Music!
            if ( bool.Parse( settings[ "Music" ].ToString() ) == true ) {
                // Stop current music
                if ( Sound.IsPlaying() )
                    Sound.Stop();

                musicEntry.menuColor = Color.Red;
                musicEntry.Text = "Music: Off";

                // Music was selected, Change settings
                settings[ "Music" ] = !bool.Parse( settings[ "Music" ].ToString() );
                settings.Save();

            } else if ( bool.Parse( settings[ "Music" ].ToString() ) == false ) {
                if ( MediaPlayer.GameHasControl == false ) {
                    System.Windows.MessageBoxResult res = System.Windows.MessageBox.Show(
                        "You are currently playing music, Would you like to stop it?", "Music",
                        System.Windows.MessageBoxButton.OKCancel );

                    if ( res == System.Windows.MessageBoxResult.OK ) {
                        // Stop Music
                        MediaPlayer.Stop();

                        Sound.Play();
                        Sound.Volume = float.Parse( settings[ "MusicVolume" ].ToString() );
                        Sound.SetLooping( true );

                        musicEntry.menuColor = Color.LightGreen;
                        musicEntry.Text = "Music: On";


                        // Music was selected, Change settings
                        settings[ "Music" ] = true;
                        settings.Save();
                    }
                } else {
                    Sound.Play();
                    Sound.Volume = float.Parse( settings[ "MusicVolume" ].ToString() );
                    Sound.SetLooping( true );

                    musicEntry.menuColor = Color.LightGreen;
                    musicEntry.Text = "Music: On";

                    // Music was selected, Change settings
                    settings[ "Music" ] = !bool.Parse( settings[ "Music" ].ToString() );
                    settings.Save();
                }
            }
        }

        void SoundFXSelected( object sender, EventArgs a ) {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if ( bool.Parse( settings[ "SoundFX" ].ToString() ) == true ) {
                fxEntry.menuColor = Color.Red;
                fxEntry.Text = "Sound Fx: Off";
            } else if ( bool.Parse( settings[ "SoundFX" ].ToString() ) == false ) {
                fxEntry.menuColor = Color.LightGreen;
                fxEntry.Text = "Sound Fx: On";
            }

            settings[ "SoundFX" ] = !bool.Parse( settings[ "SoundFX" ].ToString() );
            settings.Save();
        }

        void VibrateSelected( object sender, EventArgs e ) {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if ( bool.Parse( settings[ "Vibrate" ].ToString() ) == true ) {
                vibrateEntry.menuColor = Color.Red;
                vibrateEntry.Text = "Vibrate: Off";
            } else if ( bool.Parse( settings[ "Vibrate" ].ToString() ) == false ) {
                vibrateEntry.menuColor = Color.LightGreen;
                vibrateEntry.Text = "Vibrate: On";
            }

            settings[ "Vibrate" ] = !bool.Parse( settings[ "Vibrate" ].ToString() );
            settings.Save();
        }

        /// <summary>
        /// When Button.Back is pushed
        /// Same effect as selecting Restart
        /// </summary>
        protected override void OnCancel() {
            Dummy.Create( ScreenManager, new Background(), new MainMenu() );
        }

        public override void Update( GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen ) {
            musicVolume.Update( gameTime );
            soundFXVolume.Update( gameTime );

            base.Update( gameTime, otherScreenHasFocus, coveredByOtherScreen );
        }

        /// <summary>
        /// Draws the menu screen
        /// </summary>
        public override void Draw( GameTime gameTime ) {
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            float transitionOffset = ( float )Math.Pow( TransitionPosition, 2 );

            string musicText = "Music Volume";
            string soundText = "Sound FX Volume";

            Vector2 musicVolumePosition = new Vector2( 75, 600 );
            Vector2 soundFXVolumePosition = new Vector2( 75, 750 );
            Vector2 musicTextPosition = new Vector2( ( 150 + 95 ) -
                ( this.Font.MeasureString( musicText ).X / 2 ) * 0.5f, 540 );
            Vector2 soundFXTextPosition = new Vector2( ( 150 + 95 ) -
                ( this.Font.MeasureString( soundText ).X / 2 ) * 0.5f, 690 );

            if ( ScreenState == ScreenState.TransitionOn ) {
                musicVolumePosition = new Vector2( musicVolumePosition.X - transitionOffset * 256.0f, musicVolumePosition.Y );
                soundFXVolumePosition = new Vector2( soundFXVolumePosition.X - transitionOffset * 256.0f, soundFXVolumePosition.Y );
                musicTextPosition = new Vector2( musicTextPosition.X - transitionOffset * 256.0f, musicTextPosition.Y );
                soundFXTextPosition = new Vector2( soundFXTextPosition.X - transitionOffset * 256.0f, soundFXTextPosition.Y );
            } else {
                musicVolumePosition = new Vector2( musicVolumePosition.X + transitionOffset * 512.0f, musicVolumePosition.Y );
                soundFXVolumePosition = new Vector2( soundFXVolumePosition.X + transitionOffset * 512.0f, soundFXVolumePosition.Y );
                musicTextPosition = new Vector2( musicTextPosition.X + transitionOffset * 512.0f, musicTextPosition.Y );
                soundFXTextPosition = new Vector2( soundFXTextPosition.X + transitionOffset * 512.0f, soundFXTextPosition.Y );
            }

            spriteBatch.Draw( sliderStrip, musicVolumePosition, Color.White );
            spriteBatch.Draw( sliderStrip, soundFXVolumePosition, Color.White );

            Vector2 musicButtonPos = new Vector2( musicVolume.PositionForDraw.X, musicVolume.PositionForDraw.Y );
            Vector2 soundButtonPos = new Vector2( soundFXVolume.PositionForDraw.X, soundFXVolume.PositionForDraw.Y );

            if ( ScreenState == ScreenState.TransitionOn ) {
                musicButtonPos = new Vector2( musicButtonPos.X - transitionOffset * 256.0f, musicButtonPos.Y );
                soundButtonPos = new Vector2( soundButtonPos.X - transitionOffset * 256.0f, soundButtonPos.Y );
            } else {
                musicButtonPos = new Vector2( musicButtonPos.X + transitionOffset * 512.0f, musicButtonPos.Y );
                soundButtonPos = new Vector2( soundButtonPos.X + transitionOffset * 512.0f, soundButtonPos.Y );
            }

            musicVolume.Draw( gameTime, spriteBatch, musicButtonPos );
            soundFXVolume.Draw( gameTime, spriteBatch, soundButtonPos );

            spriteBatch.DrawString( this.Font, musicText, musicTextPosition, Color.White, 0.0f,
                Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString( this.Font, soundText, soundFXTextPosition, Color.White, 0.0f,
                Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f );

            spriteBatch.End();

            base.Draw( gameTime );
        }
    }
}
