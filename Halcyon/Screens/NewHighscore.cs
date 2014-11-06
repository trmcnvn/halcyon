//-----------------------------------------------------------------------------
// NewHighscore.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    /// <summary>
    /// Player has achieved a new high score!
    /// </summary>
    class NewHighscore : MenuScreen
    {
        /// <summary>
        /// Construct a pause menu
        /// </summary>
        public NewHighscore()
            : base("New High Score", 1.1f)
        {
            // Set this to be a pop up screen, so screens below it don't transition off
            IsPopup = true;

            // Create Entries
            MenuEntry submitEntry = new MenuEntry("Submit Your Score");
            MenuEntry restartEntry = new MenuEntry("Play Again");
            MenuEntry exitEntry = new MenuEntry("Main Menu");

            // Create Selected functions
            submitEntry.Selected += SubmitSelected;
            restartEntry.Selected += RestartSelected;
            exitEntry.Selected += ExitSelected;

            // Add Entries to list!
            MenuEntries.Add(submitEntry);
            MenuEntries.Add(restartEntry);
            MenuEntries.Add(exitEntry);
        }

        /// <summary>
        /// Submit the latest score
        /// </summary>
        void SubmitSelected(object sender, EventArgs e)
        {
            // Update highscore for uID
            if ( !com.addictionsoftware.Network.HasConnection() ) {
                System.Windows.MessageBox.Show( "It doesn't seem like you have an active internet connection.\n\nBecause of this the highscores page has been disabled.", "Alert!", System.Windows.MessageBoxButton.OK );
                return;
            } else if ( !com.addictionsoftware.Network.HasCommunication() ) {
                System.Windows.MessageBox.Show( "We are having trouble contacting the addictionsoftware servers.\n"
                + "Until this issue is resolved certain features will be disabled.\n\nWe are sorry for any inconvenience." );
                return;
            }

            string playerName;
            if ( Game.current.playerHS.UIDExists )
            {
                playerName = Game.current.playerHS.UserData[ 0 ].Name;

                Game.current.playerHS.SubmitScore( playerName, Game.current.playerScore.ToString() );

                System.Windows.MessageBox.Show( "Your score has been updated! \nCheck the global scoreboard to see if you made the top 10!" );

                Dummy.Create( ScreenManager, new Background(), new MainMenu() );
            }
            else
            {
                // Invoke name selection
                Microsoft.Xna.Framework.GamerServices.Guide.BeginShowKeyboardInput(
                   PlayerIndex.One,
                   "Choose your name",
                   "We didn't find a highscore entry for you, Since this is your first entry please select a name! (8 chars max)",
                   null,
                   changeNameCallback,
                   null );
            }
        }

        /// <summary>
        /// When a user has finished changing their name
        /// </summary>
        private void changeNameCallback( IAsyncResult res )
        {
            string newPlayerName = Microsoft.Xna.Framework.GamerServices.Guide.EndShowKeyboardInput( res );
            if ( !String.IsNullOrEmpty( newPlayerName ) )
            {
                if ( newPlayerName.Length > 8 )
                    newPlayerName = newPlayerName.Substring( 0, 8 );

                Game.current.playerHS.SubmitScore( newPlayerName, Game.current.playerScore.ToString() );

                Dummy.Create( ScreenManager, new Background(), new MainMenu() );
            }
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
                    string text = "Your previous high score";
                    Vector2 position = new Vector2( ScreenManager.GraphicsDevice.Viewport.Width / 2,
                        630 );

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
                        630 );

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
