//-----------------------------------------------------------------------------
// HScoreEntry.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Halcyon
{
    /// <summary>
    /// Represents a entry in the top 10 high score table
    /// </summary>
    class HScoreEntry
    {
        // Text to render to the screen
        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        // Position to render the text
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool ManualPosition = false;

        // Scale of the text
        private float scale;
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        private Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        // Fading effect
        private float selectionFade;

        private string font;
        private SpriteFont spritefont;

        // Event raised when the entry is selected
        // No real need for our own custom EventArgs, maybe in the future :)?
        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Called when the Selected event is fired
        /// </summary>
        protected internal virtual void OnSelected()
        {
            if ( Selected != null )
            {
                Selected( this, EventArgs.Empty );

                // Play Sound
                Effect selectEffect = new Effect( content.Load<SoundEffect>( "Sound/select" ) );
                selectEffect.Play();
            }
        }

        /// <summary>
        /// Construct a new High score entry
        /// </summary>
        public HScoreEntry(string text, string font, float scale = 1f)
        {
            this.text = text;
            this.scale = scale;
            this.font = font;
            this.color = Color.White;
        }

        public HScoreEntry( string text, float scale = 1f )
        {
            this.text = text;
            this.scale = scale;
            this.font = "Font/hscorefont";
            this.color = Color.White;
        }

        /// <summary>
        /// Update the high score entry
        /// </summary>
        public virtual void Update(HScoreScreen screen, GameTime gameTime)
        {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4f;
            selectionFade = Math.Max(selectionFade - fadeSpeed, 0f);
        }

        /// <summary>
        /// Draws the menu entry
        /// </summary>
        ContentManager content;
        public virtual void Draw(HScoreScreen screen, GameTime gameTime)
        {
            // Color of the menu entry text
            Color menuColor = color;

            float pulsate = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 6f) + 1f;
            float scale = this.scale + pulsate * .05f * selectionFade;

            // modify alpha value
            menuColor *= 1f - screen.TransitionPosition;

            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            if ( content == null )
                content = new ContentManager( screenManager.Game.Services, "Content" );

            spritefont = content.Load<SpriteFont>( font );

            Vector2 origin = new Vector2(0, 0);

            spriteBatch.DrawString(spritefont, text, position, menuColor, 0f, origin, scale,
                SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Queries how much space this menu entry requires
        /// </summary>
        public virtual int GetHeight(HScoreScreen screen)
        {
            if ( spritefont != null )
                return spritefont.LineSpacing;
            else
                return screen.Font.LineSpacing;;
        }

        /// <summary>
        /// Queries how much space this menu entry requires
        /// </summary>
        public virtual int GetWidth(HScoreScreen screen)
        {
            if ( spritefont != null )
                return ( int )spritefont.MeasureString( text ).X;
            else
                return ( int )screen.Font.MeasureString( text ).X;
        }
    }
}
