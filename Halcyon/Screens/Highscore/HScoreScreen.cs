//-----------------------------------------------------------------------------
// HScoreScreen.cs
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Halcyon
{
    abstract class HScoreScreen : Screen    
    {
        // Title to draw
        private string screenTitle;

        // the number of pixels to pad above and below menu entries for touch input
        private const int entryPadding = 15;

        private List<HScoreEntry> entries = 
            new List<HScoreEntry>();
        protected List<HScoreEntry> Entries
        {
            get { return entries; }
            set { entries = value; }
        }

        // Content Manager
        private ContentManager content;

        // Font
        private SpriteFont font;
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        private float titleScale = 1.5f;

        /// <summary>
        /// Constructs a new high score screen
        /// </summary>
        public HScoreScreen(string screenTitle, float titleScale = 1.5f)
        {
            // Enable tap on the touch screen
            EnabledGestures = GestureType.Tap;

            // Set title
            this.screenTitle = screenTitle;
            this.titleScale = titleScale;

            // Transition Time
            TransitionTimeOff = TimeSpan.FromSeconds(0.5);
            TransitionTimeOn = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            font = content.Load<SpriteFont>("Font/hscorefont");
        }

        /// <summary>
        /// Allows the screen to create the hit bounds for a particular menu entry
        /// </summary>
        protected virtual Rectangle GetEntryHitBounds(HScoreEntry entry)
        {
            // Hit bounds are the width of the entire screen, and the height of the entry
            return new Rectangle(
                ( int )entry.Position.X - ( ( entryPadding * 2 ) + 5 ),
                ( int )entry.Position.Y - ( ( entryPadding * 2 ) + 10 ),
                entry.GetWidth( this ) + entryPadding, entry.GetHeight( this ) + entryPadding );
        }

        /// <summary>
        /// Responds to user input
        /// </summary>
        public override void HandleInput(InputState input, GameTime gameTime)
        {
            // Back button pushed, Cancel the current menu screen
            if ( input.IsNewButtonPress( Buttons.Back ) ) {
                OnCancel();

                Effect backEffect = new Effect( content.Load<SoundEffect>( "Sound/back" ) );
                backEffect.Play();
            }

            // Look for any touch screen taps that occurred
            foreach (GestureSample gesture in input.gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    // Convert the tap position to a point
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                    // Check to see if any buttons were clicked
                    for (int i = 0; i < entries.Count; i++)
                    {
                        HScoreEntry x = entries[i];
                        if (GetEntryHitBounds(x).Contains(tapLocation))
                            x.OnSelected();
                    }
                }
            }
        }

        /// <summary>
        /// Allows the screen to position the menu entries
        /// </summary>
        protected virtual void UpdateEntryLocations()
        {
            // Make the menu slide into place during transition
            float transitionOffet = (float)Math.Pow(TransitionPosition, 2);

            // Update each menu entries location
            Vector2 position = new Vector2( 0f, 200f );

            for (int i = 0; i < entries.Count; i++)
            {
                HScoreEntry Entry = entries[i];

                if ( Entry.ManualPosition )
                {
                    Vector2 tmpPos = Entry.Position;

                    if ( ScreenState == ScreenState.TransitionOn )
                        Entry.Position = new Vector2( 320f - transitionOffet * 512f, Entry.Position.Y );
                    else
                        Entry.Position = new Vector2( 320f + transitionOffet * 512f, Entry.Position.Y );
                }
                else
                {
                    // Each entry is to positioned in the center
                    position = new Vector2(
                        ( ScreenManager.GraphicsDevice.Viewport.Width / 2 ) -
                        ( font.MeasureString( screenTitle ).X / 2 ) - 100,
                        position.Y );

                    if ( ScreenState == ScreenState.TransitionOn )
                        position = new Vector2( position.X - transitionOffet * 256.0f, position.Y );
                    else
                        position = new Vector2( position.X + transitionOffet * 512.0f, position.Y );

                    // set the position
                    Entry.Position = position;

                    // Move down the position for next entry
                    position = new Vector2( position.X,
                        position.Y + Entry.GetHeight( this ) + 5 );
                }
            }
        }

        /// <summary>
        /// Updates the menu screen
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].Update(this, gameTime);
            }
        }

        /// <summary>
        /// Draws the menu screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Begin rendering
            spriteBatch.Begin();

            for (int i = 0; i < entries.Count; i++)
            {
                HScoreEntry entry = entries[i];
                entry.Draw(this, gameTime);
            }

            // slide into place
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 150);
            Vector2 titleOrigin = font.MeasureString(screenTitle) / 2;
            Color titleColor = Color.Lerp( Color.Aquamarine, Color.BurlyWood, .5f ) * ( 1.0f - TransitionPosition );

            titlePosition = new Vector2(titlePosition.X, titlePosition.Y - transitionOffset * 220);

            // Draw menu title
            spriteBatch.DrawString(font, screenTitle, titlePosition, titleColor, 0.0f,
                titleOrigin, titleScale, SpriteEffects.None, 0.0f);

            // end rendering
            spriteBatch.End();
        }

        /// <summary>
        /// Handler for when the user has canceled the menu
        /// </summary>
        protected virtual void OnCancel()
        {
            Exit();
        }
    }
}
