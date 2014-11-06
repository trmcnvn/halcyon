//-----------------------------------------------------------------------------
// Dummy.cs
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    /// <summary>
    /// This class is a dummy, middle man type screen - used for nice looking transitions
    /// </summary>
    class Dummy : Screen
    {
        // Check to see if all other screens besides the dummy have transitioned off
        private bool allScreensOff;
        // Array of all the screens to load after transitions are finished
        private Screen[] screensToLoad;

        /// <summary>
        /// Constructs a dummy screen
        /// </summary>
        private Dummy(Screen[] screensToLoad)
        {
            this.screensToLoad = screensToLoad;
            TransitionTimeOn = TimeSpan.FromSeconds(0.5);
            TransitionTimeOff = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Creates a dummy screen, Used for transitioning effects
        /// </summary>
        public static void Create(ScreenManager screenManager, params Screen[] screensToLoad)
        {
            // Set all screens to transition off
            foreach (Screen screen in screenManager.GetScreens)
                screen.Exit();

            // Create dummy screen
            Dummy dummyScreen = new Dummy(screensToLoad);

            // Add Dummy screen to screen manager
            screenManager.Add(dummyScreen);
        }

        /// <summary>
        /// Allows the dummy screen to update
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (allScreensOff)
            {
                // Remove this dummy screen
                ScreenManager.Remove(this);

                // Add all screens we want to load, now that the dummy page is done
                foreach (Screen screen in screensToLoad)
                    ScreenManager.Add(screen);

                // Reset Elapsed Time
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        /// <summary>
        /// Allows the dummy screen to draw
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // We do this in draw rather than update due to the fact we want it to draw a frame before hand.
            if ((ScreenState == ScreenState.Active) && (ScreenManager.GetScreens.Length == 1))
                allScreensOff = true;

            // Transition the game screen off
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(0.0f + (1.0f - TransitionPosition));
        }
    }
}
