//-----------------------------------------------------------------------------
// Powerups.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Phone.Tasks;

namespace Halcyon {
    /// <summary>
    /// Powerup help
    /// </summary>
    class About : MenuScreen {
        /// <summary>
        /// Font for this page
        /// </summary>
        private SpriteFont font;
        private SpriteFont titleFont;

        /// <summary>
        /// Web task
        /// </summary>
        private WebBrowserTask webTask;

        /// <summary>
        /// Player ship model
        /// </summary>
        private Texture2D ship;

        /// <summary>
        /// as logo
        /// </summary>
        private Texture2D aslogo;

        /// <summary>
        /// Construct a pause menu
        /// </summary>
        public About()
            : base( "About" ) {
                
                entryPosition = 240.0f;
        }


        /// <summary>
        /// Load screen content here
        /// </summary>
        #pragma warning disable 0618
        public override void LoadContent() {
            base.LoadContent();

            if ( content == null )
                content = new ContentManager( ScreenManager.Game.Services, "Content" );

            font = content.Load<SpriteFont>( "Font/powerupdesc" );
            titleFont = content.Load<SpriteFont>( "Font/menufont" );
            ship = content.Load<Texture2D>( "Player/player" );
            aslogo = content.Load<Texture2D>( "Menu/aslogo-trans-white" );

            webTask = new WebBrowserTask();
            webTask.URL = "http://www.addictionsoftware.com"; // Mango update
        }

        /// <summary>
        /// Exit Game has been selected
        /// </summary>
        protected override void OnCancel() {
            Dummy.Create( ScreenManager, new Background(), new MainMenu() );
        }

        /// <summary>
        /// Returns bounds of the as logo
        /// </summary>
        /// <returns></returns>
        protected virtual Rectangle GetLogoBounds() {
            return new Rectangle( ScreenManager.GraphicsDevice.Viewport.Width / 2 - ( aslogo.Width / 2 ), ScreenManager.GraphicsDevice.Viewport.Height - aslogo.Height,
                aslogo.Width, aslogo.Height );
        }

        /// <summary>
        /// Handles this screens input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="gameTime"></param>
        public override void HandleInput( InputState input, GameTime gameTime ) {
            base.HandleInput( input, gameTime );

            foreach ( GestureSample gesture in input.gestures ) {
                if ( gesture.GestureType == GestureType.Tap ) {
                    Point loc = new Point( ( int )gesture.Position.X, ( int )gesture.Position.Y );

                    if ( GetLogoBounds().Contains( loc ) )
                        webTask.Show();
                }
            }
        }

        /// <summary>
        /// Update function
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update( GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen ) {
            base.Update( gameTime, otherScreenHasFocus, coveredByOtherScreen );
        }

        /// <summary>
        /// Draw function, Draw to screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw( GameTime gameTime ) {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Begin rendering
            spriteBatch.Begin();

            string description = "You are in a fight to the death with the\nenemy forces of Halcyon! Take out as\nmany enemies as you can before you die.";
            Vector2 descPosition = new Vector2( 10.0f, entryPosition );

            Vector2 shipPosition = new Vector2( ScreenManager.GraphicsDevice.Viewport.Width / 2.0f, entryPosition + 260.0f );

            string helpTitle = "How to play";
            Vector2 titlePosition = new Vector2( ScreenManager.GraphicsDevice.Viewport.Width / 2.0f, entryPosition + 180.0f );

            string help = "Tap or drag to move your ship, You can\nalso move left and right by tilting your\nphone. Check out the powerup page for\nmore information!";
            Vector2 helpPosition = new Vector2( 10.0f, entryPosition + 330.0f );

            Vector2 asPosition = new Vector2( ScreenManager.GraphicsDevice.Viewport.Width / 2.0f,
                ScreenManager.GraphicsDevice.Viewport.Height - aslogo.Height );

            float offset = ( float )Math.Pow( TransitionPosition, 2.0 );

            if ( ScreenState == ScreenState.TransitionOn ) {
                descPosition = new Vector2( descPosition.X - offset * 256.0f, descPosition.Y );
                helpPosition = new Vector2( helpPosition.X - offset * 256.0f, helpPosition.Y );
                asPosition = new Vector2( asPosition.X - offset * 256.0f, asPosition.Y );
                shipPosition = new Vector2( shipPosition.X - offset * 256.0f, shipPosition.Y );
                titlePosition = new Vector2( titlePosition.X - offset * 256.0f, titlePosition.Y );
            } else {
                descPosition = new Vector2( descPosition.X + offset * 512.0f, descPosition.Y );
                helpPosition = new Vector2( helpPosition.X + offset * 512.0f, helpPosition.Y );
                asPosition = new Vector2( asPosition.X + offset * 512.0f, asPosition.Y );
                shipPosition = new Vector2( shipPosition.X + offset * 512.0f, shipPosition.Y );
                titlePosition = new Vector2( titlePosition.X + offset * 512.0f, titlePosition.Y );
            }

            // Description
            spriteBatch.DrawString( font, description,
                descPosition, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f );

            spriteBatch.Draw( ship, shipPosition, null, Color.White, 0.0f, new Vector2( ship.Width / 2.0f, ship.Height / 2.0f ), 0.8f, 
                SpriteEffects.None, 0.0f );

            // Help
            spriteBatch.DrawString( font, help, helpPosition, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f );

            spriteBatch.Draw( aslogo, asPosition, null, Color.White, 0.0f, new Vector2( aslogo.Width / 2.0f, aslogo.Height / 2.0f ), 1.0f,
                SpriteEffects.None, 0.0f );

            spriteBatch.DrawString( titleFont, helpTitle, titlePosition, Color.Lerp( Color.Aquamarine, Color.BurlyWood, .5f ) * ( 1.0f - TransitionPosition ), 
                0.0f, new Vector2( titleFont.MeasureString( helpTitle ).X / 2.0f, titleFont.MeasureString( helpTitle ).Y / 2.0f ),
                1.0f, SpriteEffects.None, 0.0f );

            // End rendering
            spriteBatch.End();

            base.Draw( gameTime );

            if ( TransitionPosition > 0 )
                ScreenManager.FadeBackBufferToBlack( 1.0f - ( 1.0f - TransitionPosition ) );
        }
    }
}
