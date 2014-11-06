//-----------------------------------------------------------------------------
// Animation.cs
//-----------------------------------------------------------------------------

// Namespace Usage
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    public class Animation
    {
        // Image representing the collection of images
        Texture2D spriteStrip;

        // Scale used to display the strip
        float scale;

        // Time since we last updated
        int elapsedTime;

        // Time we display a frame until the next one
        int frameTime;

        // Number of frames that the animation contains
        int frameCount;

        // Index of the current frame we are displaying
        int currentFrame;

        // Area of the image strip we want to display
        Rectangle sourceRect = new Rectangle();

        // Area where we want to display the image strip in game
        Rectangle destRect = new Rectangle();

        // Color of the frame we are displaying
        public Color color;

        // Width of a frame
        public int FrameWidth;

        // Height of a frame
        public int FrameHeight;

        // State of the animation
        public bool Active;

        // Determines if the animation should loop
        public bool Looping;

        // Position
        public Vector2 Position = Vector2.Zero;

        // Creates a boundary rectangle around the texture
        public Rectangle CollisionRect
        {
            get
            {
                return new Rectangle((int)(Position.X - ((FrameWidth / 2) * scale)),
                    (int)(Position.Y - ((FrameHeight / 2) * scale)), (int)(FrameWidth * scale), (int)(FrameHeight * scale));
            }
        }

        // Checks whether or not the sprite collides with this one
        public bool CollisionCheck(Sprite sprite)
        {
            return CollisionRect.Intersects(sprite.CollisionRect);
        }

        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount,
            int frametime, Color color, float scale, bool looping)
        {
            // Local copy of the variables
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;

            Looping = looping;
            Position = position;
            spriteStrip = texture;

            // Set time to zero
            elapsedTime = 0;
            currentFrame = 0;

            // Set animation state to active
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            // Do not update if we aren't active
            if (Active == false)
                return;

            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (elapsedTime > frameTime)
            {
                // Move to the next frame
                currentFrame++;

                // if the currentFrame is equal to frameCount reset currentFrame to 0
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (Looping == false)
                        Active = false;
                }

                // Reset elapsed time
                elapsedTime = 0;
            }

            // Grab the correct frame in the image strip
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            // Grab the correct frame in the image strip
            destRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
                (int)Position.Y - (int)(FrameHeight * scale) / 2,
                (int)(FrameWidth * scale),
                (int)(FrameHeight * scale));
        }

        // Draw the animation strip
        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation if we are active
            if (Active)
                spriteBatch.Draw(spriteStrip, destRect, sourceRect, color);
        }
    }
}