//-----------------------------------------------------------------------------
// MenuScreen.cs
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Halcyon
{
    abstract class MenuScreen : Screen
    {
        // the number of pixels to pad above and below menu entries for touch input
        public int menuEntryPadding = 15;

        // List of menu entries
        List<MenuEntry> menuEntries = new List<MenuEntry>();
        // Menu Title
        private string menuTitle;

        // Returns the list of menu entries, so classes can add/remove entries
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        // Content Manager
        protected ContentManager content;

        public float entryPosition = 300.0f;
        float scale;

        // Font
        private SpriteFont font;
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        /// <summary>
        /// Constructs a menu screen
        /// </summary>
        public MenuScreen(string menuTitle, float scale = 1.5f)
        {
            // Enable tap on touch screen for menu selection :)
            EnabledGestures = GestureType.Tap;

            this.menuTitle = menuTitle;
            this.scale = scale;

            // Set Transition time
            TransitionTimeOff = TimeSpan.FromSeconds(0.5);
            TransitionTimeOn = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            font = content.Load<SpriteFont>("Font/menufont");
        }

        /// <summary>
        /// Allows the screen to create the hit bounds for a particular menu entry
        /// </summary>
        protected virtual Rectangle GetMenuEntryHitBounds(MenuEntry entry)
        {
            // Hit bounds are the width of the entire screen, and the height of the entry
            return new Rectangle((int)entry.Position.X - ((menuEntryPadding * 2) + 5), 
                (int)entry.Position.Y - ((menuEntryPadding * 2) + 10),
                entry.GetWidth(this) + (menuEntryPadding),
                entry.GetHeight(this) + (menuEntryPadding));
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

                    // Loop thru menu entries to see if any were tapped
                    for (int i = 0; i < menuEntries.Count; i++)
                    {
                        MenuEntry menuEntry = menuEntries[i];

                        if ( GetMenuEntryHitBounds( menuEntry ).Contains( tapLocation ) ) {
                            menuEntry.OnSelected();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handler for when the user has canceled the menu
        /// </summary>
        protected virtual void OnCancel()
        {
            Exit();
        }

        /// <summary>
        /// Allows the screen to position the menu entries
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transition
            float transitionOffet = (float)Math.Pow(TransitionPosition, 2);

            // Start at Y = 300
            Vector2 position = new Vector2(0.0f, entryPosition);

            // Update each menu entries location
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                // Each entry is to positioned in the center
                position = new Vector2(
                    ScreenManager.GraphicsDevice.Viewport.Width / 2 - ((menuEntry.GetWidth(this) / 2) * menuEntry.Scale), 
                    position.Y);

                if (ScreenState == ScreenState.TransitionOn)
                    position = new Vector2(position.X - transitionOffet * 256.0f, position.Y);
                else
                    position = new Vector2(position.X + transitionOffet * 512.0f, position.Y);

                // set the position
                menuEntry.Position = position;

                // Move down the position for next entry
                position = new Vector2(position.X, 
                    position.Y + menuEntry.GetHeight(this) + (menuEntryPadding * 2) + 15);
            }
        }

        /// <summary>
        /// Updates the menu screen
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each menu entry
            for (int i = 0; i < menuEntries.Count; i++)
            {
                menuEntries[i].Update(this, gameTime);
            }
        }

        /// <summary>
        /// Draws the menu screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Update our menu entries positions
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Begin rendering
            spriteBatch.Begin();

            // draw each menu entry
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];
                menuEntry.Draw(this, gameTime);

            }

            // slide into place
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 200);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = Color.Lerp(Color.Aquamarine, Color.BurlyWood, .5f) * (1.0f - TransitionPosition);
            float titleScale = scale;

            titlePosition = new Vector2(titlePosition.X, titlePosition.Y - transitionOffset * 220);

            // Draw menu title
            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0.0f,
                titleOrigin, titleScale, SpriteEffects.None, 0.0f);

            // end rendering
            spriteBatch.End();
        }
    }
}
