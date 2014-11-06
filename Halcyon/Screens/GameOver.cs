//-----------------------------------------------------------------------------
// GameOver.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Halcyon
{
    /// <summary>
    /// Game has ended!
    /// </summary>
    class GameOver : MenuScreen
    {
        float val = 0f;

        /// <summary>
        /// Construct a pause menu
        /// </summary>
        public GameOver()
            : base("Game Over")
        {
            // Set this to be a pop up screen, so screens below it don't transition off
            IsPopup = true;

            entryPosition = 320.0f;

            // Current score
            val = Game.current.playerScore;

            // Create Entries
            MenuEntry restartEntry = new MenuEntry("Play Again");
            MenuEntry exitEntry = new MenuEntry("Main Menu");

            // Create Selected functions
            restartEntry.Selected += RestartSelected;
            exitEntry.Selected += ExitSelected;

            // Add Entries to list!
            MenuEntries.Add(restartEntry);
            MenuEntries.Add(exitEntry);
        }

        /// <summary>
        /// Restart the game
        /// </summary>
        void RestartSelected(object sender, EventArgs e)
        {
            Dummy.Create(ScreenManager, new Background(), new Game(Game.current.playerHS));
        }

        /// <summary>
        /// Main Menu was selected 
        /// </summary>
        void ExitSelected(object sender, EventArgs e)
        {
            Dummy.Create(ScreenManager, new Background(), new MainMenu());
        }

        /// <summary>
        /// When Button.Back is pushed
        /// Same effect as selecting Restart
        /// </summary>
        protected override void OnCancel()
        {
            Dummy.Create(ScreenManager, new Background(), new MainMenu());
        }

        /// <summary>
        /// Draws the menu screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            float transitionOffset = ( float )Math.Pow( TransitionPosition, 2 );

            if ( com.addictionsoftware.Network.HasCommunication() && com.addictionsoftware.Network.HasConnection() ) {
                if ( Game.current.playerHS.UIDExists ) {
                    // Player has a previous record
                    string text = "Your high score";
                    Vector2 position = new Vector2( ScreenManager.GraphicsDevice.Viewport.Width / 2,
                        560 );

                    string score = Game.current.playerHS.UserData[ 0 ].Name + " - " +
                        Game.current.playerHS.UserData[ 0 ].Score.ToString();
                    Vector2 scorePosition = new Vector2( position.X, position.Y + 50 );

                    if ( ScreenState == ScreenState.TransitionOn ) {
                        position = new Vector2( position.X - transitionOffset * 256.0f, position.Y );
                        scorePosition = new Vector2( scorePosition.X - transitionOffset * 256.0f, scorePosition.Y );
                    } else {
                        position = new Vector2( position.X + transitionOffset * 512.0f, position.Y );
                        scorePosition = new Vector2( scorePosition.X + transitionOffset * 512.0f, scorePosition.Y );
                    }

                    spriteBatch.DrawString( this.Font, text, position, Color.YellowGreen, 0.0f,
                        this.Font.MeasureString( text ) / 2, 0.65f, SpriteEffects.None, 0.0f );
                    spriteBatch.DrawString( this.Font, score, scorePosition, Color.YellowGreen, 0.0f,
                        this.Font.MeasureString( score ) / 2, 0.65f, SpriteEffects.None, 0.0f );
                } else {
                    string text = "No Highscore Found";
                    Vector2 position = new Vector2( ScreenManager.GraphicsDevice.Viewport.Width / 2,
                        560 );

                    if ( ScreenState == ScreenState.TransitionOn ) {
                        position = new Vector2( position.X - transitionOffset * 256.0f, position.Y );
                    } else {
                        position = new Vector2( position.X + transitionOffset * 512.0f, position.Y );
                    }

                    spriteBatch.DrawString( this.Font, text, position, Color.YellowGreen, 0.0f,
                        this.Font.MeasureString( text ) / 2, 0.65f, SpriteEffects.None, 0.0f );
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
