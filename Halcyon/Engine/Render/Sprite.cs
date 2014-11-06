//-----------------------------------------------------------------------------
// Sprite.cs
//-----------------------------------------------------------------------------

// Namespace Usage
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    /// <summary>
    /// Represents a Texture2D
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// Returns the current texture
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        private Texture2D texture;

        /// <summary>
        /// Returns/Sets the current position of the texture
        /// Default value: (0, 0)
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        private Vector2 position = Vector2.Zero;

        public Vector2 PrevPosition;

        /// <summary>
        /// Returns/Sets the current velocity of the texture
        /// Default value: (0, 0)
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        private Vector2 velocity = Vector2.Zero;

        /// <summary>
        /// Returns the origin
        /// </summary>
        public Vector2 Origin
        {
            get { return new Vector2(Width / 2, Height / 2); }
        }

        /// <summary>
        /// Returns/Sets the current color of the texture
        /// Default value: White
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        private Color color = Color.White;

        /// <summary>
        /// Returns/Sets the current rotation of the texture
        /// Default value: 0.0f
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        private float rotation = 0.0f;

        /// <summary>
        /// Returns/Sets the scale of the texture
        /// Default value: 1.0f
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        private float scale = 1.0f;

        /// <summary>
        /// Returns the width of the texture
        /// </summary>
        public int Width
        {
            get { return Texture.Width; }
        }

        /// <summary>
        /// Returns the height of the texture
        /// </summary>
        public int Height
        {
            get { return Texture.Height; }
        }

        /// <summary>
        /// Creates a boundary rectangle around the texture
        /// </summary>
        public Rectangle CollisionRect
        {
            get
            {
                return new Rectangle((int)(Position.X - ((Width / 2) * Scale)), 
                    (int)(Position.Y - ((Height / 2) * Scale)), (int)(Width * Scale), (int)(Height * Scale));
            }
        }

        /// <summary>
        /// Constructs a new Texture
        /// </summary>
        /// <param name="texture">Texture for this class to hold</param>
        public Sprite(Texture2D texture)
        {
            this.texture = texture;
        }

        /// <summary>
        /// Create a clone of this sprite
        /// </summary>
        public Sprite Clone()
        {
            Sprite cloneSprite = new Sprite(this.texture);

            cloneSprite.position = this.position;
            cloneSprite.velocity = this.velocity;
            cloneSprite.rotation = this.rotation;
            cloneSprite.color = this.color;
            cloneSprite.scale = this.scale;

            return cloneSprite;
        }

        /// <summary>
        /// Checks whether or not the sprite collides with this one
        /// </summary>
        public bool CollisionCheck(Sprite sprite)
        {
            return CollisionRect.Intersects(sprite.CollisionRect);
        }

        public void Draw( GameTime gameTime, SpriteBatch spriteBatch, SpriteEffects spriteEffects, Vector2 position )
        {
            // Create source Rect
            Rectangle sourceRect = new Rectangle( 0, 0, Width, Height );

            // Draw the sprite
            spriteBatch.Draw( Texture, position, sourceRect,
                Color, MathHelper.ToRadians( Rotation ), Origin, Scale, spriteEffects, 0f );
        }

        /// <summary>
        /// Draws the texture onto the screen
        /// </summary>
        /// <param name="spriteBatch">Enables us to draw textures</param>
        /// <param name="spriteEffects">Defines sprite mirroring options</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            // Create source Rect
            Rectangle sourceRect = new Rectangle(0, 0, Width, Height);

            // Draw the sprite
            spriteBatch.Draw(Texture, Position, sourceRect,
                Color, MathHelper.ToRadians(Rotation), Origin, Scale, spriteEffects, 0f);
        }

    }
}
