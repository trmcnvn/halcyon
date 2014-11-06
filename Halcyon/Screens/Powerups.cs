//-----------------------------------------------------------------------------
// Powerups.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    /// <summary>
    /// Powerup help
    /// </summary>
    class Powerups : MenuScreen
    {
        // shield power up
        Animation shieldTexture;
        // bomb power up
        Animation bombTexture;
        // +1 life power up
        Animation lifeTexture;

        // Font
        SpriteFont font;
        SpriteFont descFont;

        /// <summary>
        /// Construct a pause menu
        /// </summary>
        public Powerups()
            : base( "Powerups" )
        {
            
        }


        /// <summary>
        /// Load screen content here
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            if ( content == null )
                content = new ContentManager( ScreenManager.Game.Services, "Content" );

            // font
            font = content.Load<SpriteFont>( "Font/powerup" );
            descFont = content.Load<SpriteFont>( "Font/powerupdesc" );

            // Load textures
            shieldTexture = new Animation();
            bombTexture = new Animation();
            lifeTexture = new Animation();

            shieldTexture.Initialize(
                content.Load<Texture2D>( "Animations/shield_powerup" ), new Vector2( 85, 350 ), 128, 128, 16, 32,
                Color.White, 1f, true );

            bombTexture.Initialize(
                content.Load<Texture2D>( "Animations/bomb_powerup" ), new Vector2( 85, 530 ), 128, 128, 16, 32,
                Color.White, 1f, true );

            lifeTexture.Initialize(
                content.Load<Texture2D>( "Animations/life_powerup" ), new Vector2( 85, 700 ), 128, 128, 16, 32,
                Color.White, .65f, true );
        }

        /// <summary>
        /// Exit Game has been selected
        /// </summary>
        protected override void OnCancel()
        {
            Dummy.Create( ScreenManager, new Background(), new MainMenu() );
        }

        /// <summary>
        /// Update function
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update( GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen )
        {
            base.Update( gameTime, otherScreenHasFocus, coveredByOtherScreen );

            // Transition
            float transitionOffset = ( float )Math.Pow( TransitionPosition, 2 );

            shieldTexture.Position = new Vector2( 165, 290 );
            lifeTexture.Position = new Vector2( 165, 665 );
            bombTexture.Position = new Vector2( 165, 470 );

            if ( ScreenState == ScreenState.TransitionOn ) {
                shieldTexture.Position = new Vector2( shieldTexture.Position.X - transitionOffset * 256.0f, 
                    shieldTexture.Position.Y );
                lifeTexture.Position = new Vector2( lifeTexture.Position.X - transitionOffset * 256.0f,
                    lifeTexture.Position.Y );
                bombTexture.Position = new Vector2( bombTexture.Position.X - transitionOffset * 256.5f,
                    bombTexture.Position.Y );
            } else {
                shieldTexture.Position = new Vector2( shieldTexture.Position.X + transitionOffset * 512.0f, 
                    shieldTexture.Position.Y );
                lifeTexture.Position = new Vector2( lifeTexture.Position.X + transitionOffset * 512.0f,
                    lifeTexture.Position.Y );
                bombTexture.Position = new Vector2( bombTexture.Position.X + transitionOffset * 512.0f,
                    bombTexture.Position.Y );
            }

            shieldTexture.Update( gameTime );
            lifeTexture.Update( gameTime );
            bombTexture.Update( gameTime );
        }

        /// <summary>
        /// Draw function, Draw to screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw( GameTime gameTime )
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Begin rendering
            spriteBatch.Begin();

            // Draw animations
            shieldTexture.Draw( spriteBatch );
            bombTexture.Draw( spriteBatch );
            lifeTexture.Draw( spriteBatch );

            // Draw text
            Vector2 shieldPosition = new Vector2(10, 320 );
            Vector2 bombPosition = new Vector2( 10, 505 );
            Vector2 lifePosition = new Vector2( 10, 700 );

            Vector2 shieldTitle = new Vector2( ScreenManager.GraphicsDevice.Viewport.Width / 2.0f - this.font.LineSpacing, 275 );
            Vector2 bombTitle = new Vector2( ScreenManager.GraphicsDevice.Viewport.Width / 2.0f - this.font.LineSpacing, 450 );
            Vector2 lifeTitle = new Vector2( ScreenManager.GraphicsDevice.Viewport.Width / 2.0f - this.font.LineSpacing, 655 );

            // Transition
            float transitionOffset = ( float )Math.Pow( TransitionPosition, 2 );

            if ( ScreenState == ScreenState.TransitionOn ) {
                shieldPosition = new Vector2( shieldPosition.X - transitionOffset * 256.0f, shieldPosition.Y );
                bombPosition = new Vector2( bombPosition.X - transitionOffset * 256.0f, bombPosition.Y );
                lifePosition = new Vector2( lifePosition.X - transitionOffset * 256.0f, lifePosition.Y );

                shieldTitle = new Vector2( shieldTitle.X - transitionOffset * 256.0f, shieldTitle.Y );
                bombTitle = new Vector2( bombTitle.X - transitionOffset * 256.0f, bombTitle.Y );
                lifeTitle = new Vector2( lifeTitle.X - transitionOffset * 256.0f, lifeTitle.Y );
            } else {
                shieldPosition = new Vector2( shieldPosition.X + transitionOffset * 512.0f, shieldPosition.Y );
                bombPosition = new Vector2( bombPosition.X + transitionOffset * 512.0f, bombPosition.Y );
                lifePosition = new Vector2( lifePosition.X + transitionOffset * 512.0f, lifePosition.Y );

                shieldTitle = new Vector2( shieldTitle.X + transitionOffset * 512.0f, shieldTitle.Y );
                bombTitle = new Vector2( bombTitle.X + transitionOffset * 512.0f, bombTitle.Y );
                lifeTitle = new Vector2( lifeTitle.X + transitionOffset * 512.0f, lifeTitle.Y );
            }

            spriteBatch.DrawString( this.font, "Shield", shieldTitle, Color.LightBlue );
            spriteBatch.DrawString( this.descFont,
                "You will be granted a shield of immunity.\nWhile this shield is active you cannot be\nkilled! Take advantage of this.",
                shieldPosition, Color.LightBlue );

            spriteBatch.DrawString( this.font, "Bomb", bombTitle, Color.LightPink );
            spriteBatch.DrawString( this.descFont,
                "Activating a bomb will kill all enemies\nwithin its range, You can double-tap your\nscreen to activate it. Your ship can only\nhold two bombs at one time",
                bombPosition, Color.LightPink );

            spriteBatch.DrawString( this.font, " Life", lifeTitle, Color.LightGreen );
            spriteBatch.DrawString( this.descFont,
                "You will be granted an extra life for a\nmaximum of 5 lives. When you are out\nof lives it is game over!",
                lifePosition, Color.LightGreen );

            // End rendering
            spriteBatch.End();

            base.Draw( gameTime );

            if ( TransitionPosition > 0 )
                ScreenManager.FadeBackBufferToBlack( 1.0f - ( 1.0f - TransitionPosition ) );
        }
    }
}
