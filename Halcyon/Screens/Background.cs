//-----------------------------------------------------------------------------
// Background.cs
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AdDuplex.Xna;

namespace Halcyon
{
    class Background : Screen
    {
        // Content Manager
        private ContentManager content;

        // Generic content
        private Texture2D starTexture;
        private Texture2D asteroidTexture;
        private Texture2D backgroundTexture;
        private Texture2D backgroundOverlayTexture;

        private List<Sprite> starList = new List<Sprite>();
        private List<Sprite> asteroidList = new List<Sprite>();

        // Number of stars and asteroids to generate at any giving time
        private const int starCount = 300;
        private const int asteroidCount = 6;

        /// <summary>
        /// Background Constructor
        /// </summary>
        public Background()
        {
            TransitionTimeOff = TimeSpan.FromSeconds(0.5);
            TransitionTimeOn = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            // Setup Content
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Load Textures
            backgroundTexture = content.Load<Texture2D>("Background/game-background");
            backgroundOverlayTexture = content.Load<Texture2D>("Background/blank");
            starTexture = content.Load<Texture2D>("Background/star");
            asteroidTexture = content.Load<Texture2D>("Background/Asteroid");

            // Loop thru the constant amount of stars to create
            for (int i = 0; i < starCount; i++)
            {
                // Initialize Sprite
                Sprite starSprite = new Sprite(starTexture);
                // Set Sprite Position
                starSprite.Position = new Vector2(
                    (float)RandomMath.Random.Next(ScreenManager.GraphicsDevice.Viewport.Width),
                    (float)RandomMath.Random.Next(ScreenManager.GraphicsDevice.Viewport.Height));
                // Set Sprite Velocity
                starSprite.Velocity = new Vector2(0.0f, 6.0f);

                // Add Sprite to List
                starList.Add(starSprite);
            }
            // Loop thru the constant amount of asteroids to create
            for (int i = 0; i < asteroidCount; i++)
            {
                // Initialize Sprite
                Sprite asteroidSprite = new Sprite(asteroidTexture);
                // Set Sprite Position
                asteroidSprite.Position = new Vector2(
                    (float)RandomMath.Random.Next(ScreenManager.GraphicsDevice.Viewport.Width),
                    (float)RandomMath.Random.Next(ScreenManager.GraphicsDevice.Viewport.Height));
                // Set Sprite Velocity
                asteroidSprite.Velocity = new Vector2(4.0f, 6.0f);
                // Set Sprite Scale
                asteroidSprite.Scale = (float)RandomMath.Random.NextDouble();

                // Add Sprite to List
                asteroidList.Add(asteroidSprite);
            }
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Loop thru the constant amount of stars
            for (int i = 0; i < starCount; i++)
            {
                // Get current Sprite
                Sprite starSprite = starList[i];
                // Update Sprite Position
                starSprite.Position = new Vector2(starSprite.Position.X, 
                    starSprite.Position.Y + starSprite.Velocity.Y);
                // Check if Y axis is out of bounds
                if (starSprite.Position.Y > ScreenManager.GraphicsDevice.Viewport.Height)
                    // Sprite has hit the end of the viewport, Reset it's position
                    starSprite.Position = new Vector2(
                        (float)RandomMath.Random.Next(ScreenManager.GraphicsDevice.Viewport.Width), 0.0f);
            }
            // Loop thru the constant amount of asteroids
            for (int i = 0; i < asteroidCount; i++)
            {
                // Get current sprite
                Sprite asteroidSprite = asteroidList[i];
                // Update sprite position
                asteroidSprite.Position = new Vector2(asteroidSprite.Position.X + asteroidSprite.Velocity.X,
                    asteroidSprite.Position.Y + asteroidSprite.Velocity.Y);
                // Check if X axis is out of bounds
                if (asteroidSprite.Position.X > ScreenManager.GraphicsDevice.Viewport.Width + asteroidSprite.Width)
                    // Sprite has hit the boundaries, reset it's position
                    asteroidSprite.Position = new Vector2(0.0f - asteroidSprite.Width, asteroidSprite.Position.Y);
                if (asteroidSprite.Position.Y > ScreenManager.GraphicsDevice.Viewport.Height + asteroidSprite.Height)
                    // Sprite has hit the boundaries, reset it's position
                    asteroidSprite.Position = new Vector2(asteroidSprite.Position.X, 0.0f - asteroidSprite.Height);

                // Set sprites rotation
                asteroidSprite.Rotation += 2.0f;
                if (asteroidSprite.Rotation >= 360.0f)
                    asteroidSprite.Rotation = 0.0f;
            }

            // This screen doesn't get covered by others :)
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Begin rendering
            spriteBatch.Begin();

            // Draw Background
            spriteBatch.Draw(backgroundTexture,
                new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, 
                   ScreenManager.GraphicsDevice.Viewport.Height), Color.White);

            // Draw star field
            foreach (Sprite starSprite in starList)
                starSprite.Draw(gameTime, spriteBatch, SpriteEffects.None);
            // Draw asteroid field
            foreach (Sprite asteroidSprite in asteroidList)
                asteroidSprite.Draw(gameTime, spriteBatch, SpriteEffects.None);

            // Draw Overlay (This is for a darker look, Note: Possible without a texture?)
            spriteBatch.Draw(backgroundOverlayTexture, 
                new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, 
                    ScreenManager.GraphicsDevice.Viewport.Height), Color.Black * 0.5f);

            // End Rendering
            spriteBatch.End();
        }
    }
}
