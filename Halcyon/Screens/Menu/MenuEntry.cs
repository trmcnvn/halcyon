//-----------------------------------------------------------------------------
// MenuEntry.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    /// <summary>
    /// Represents a single entry on a menu screen
    /// </summary>
    class MenuEntry
    {
        // The text rendered to screen
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        private string text;

        // Position to render the text
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        private Vector2 position;

        // Scale
        public float Scale;
        
        // Color
        public Color menuColor = Color.White;

        // fading effect on the menu entry
        private float selectionFade;

        private string font;
        private SpriteFont spritefont;

        // Event raised when the menu entry is selected
        // No real need for our own custom EventArgs, maybe in the future :)?
        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Called when the Selected event is fired
        /// </summary>
        protected internal virtual void OnSelected()
        {
            if (Selected != null)
            {
                //menuColor = Color.Aquamarine;
                Selected(this, EventArgs.Empty);

                // Play Sound
                Effect selectEffect = new Effect( content.Load<SoundEffect>( "Sound/select" ) );
                selectEffect.Play();
            }
        }

        /// <summary>
        /// Constructs a new menu entry
        /// </summary>
        /// <param name="text">The text to render</param>
        public MenuEntry(string text, string font, float scale = .85f)
        {
            this.text = text;
            this.Scale = scale;
            this.font = font;
        }

        public MenuEntry( string text, float scale = .85f )
        {
            this.text = text;
            this.Scale = scale;
            this.font = "Font/menufont";
        }

        /// <summary>
        /// Updates the menu entry
        /// </summary>
        public virtual void Update(MenuScreen screen, GameTime gameTime)
        {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4.0f;
            selectionFade = Math.Max(selectionFade - fadeSpeed, 0.0f);
        }

        /// <summary>
        /// Draws the menu entry
        /// </summary>
        ContentManager content;
        public virtual void Draw(MenuScreen screen, GameTime gameTime)
        {
            // Color of the menu entry text
            Color tmpColor = menuColor;

            float pulsate = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 6.0f) + 1.0f;
            float scale = Scale + pulsate * 0.05f * selectionFade;
            Scale = scale;

            // modify alpha value
            tmpColor *= 1.0f - screen.TransitionPosition;

            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            if ( content == null )
                content = new ContentManager(screenManager.Game.Services, "Content");
            this.spritefont = content.Load<SpriteFont>( this.font );

            Vector2 origin = new Vector2(0.0f, spritefont.LineSpacing / 2.0f);

            spriteBatch.DrawString(spritefont, text, position, tmpColor, 0.0f, origin, scale,
                SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// Queries how much space this menu entry requires
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            if ( spritefont != null )
                return spritefont.LineSpacing;
            else
                return screen.ScreenManager.Font.LineSpacing;
        }

        /// <summary>
        /// Queries how much space this menu entry requires
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            if ( spritefont != null )
                return ( int )spritefont.MeasureString( text ).X;
            else
                return ( int )screen.ScreenManager.Font.MeasureString( Text ).X;
        }
    }
}