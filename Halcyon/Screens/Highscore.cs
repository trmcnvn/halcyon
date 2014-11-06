//-----------------------------------------------------------------------------
// Highscore.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    class Highscore : HScoreScreen
    {
        // Highscores class object
        private Highscores hs;

        private HScoreEntry refreshEntry;
        private HScoreEntry nameEntry;

        float fade = 0.0f;

        /// <summary>
        /// Construct a high score page
        /// </summary>
        public Highscore(Highscores hs)
            : base("High Scores", 1.3f)
        {
            this.hs = hs;

            refreshEntry = new HScoreEntry("Refresh", "Font/menufont", .65f);
            nameEntry = new HScoreEntry("Change Name", "Font/menufont", .65f);

            refreshEntry.Selected += refreshEntry_Selected;
            nameEntry.Selected += nameEntry_Selected;
        }

        /// <summary>
        /// When Refresh is clicked!
        /// </summary>
        private void refreshEntry_Selected(object sender, EventArgs e)
        {
            // Re-download the data from our server
            hs = new Highscores();
            hs.DownloadTop10();
            hs.CheckuID();
            hs.Fetch();
            hs.Refreshing = true;
        }

        /// <summary>
        /// User wants to change their name
        /// </summary>
        private void nameEntry_Selected(object sender, EventArgs e)
        {
            if (hs.UIDExists)
            {
                Microsoft.Xna.Framework.GamerServices.Guide.BeginShowKeyboardInput(
                    PlayerIndex.One,
                    "Change your name",
                    "This will allow you to change the name of your highscore entry. (8 Characters Max)",
                    null,
                    changeNameCallback,
                    null);
            }
            else
                System.Windows.MessageBox.Show("We couldn't find a DB entry for you, Have you submitted a score yet?");
        }

        /// <summary>
        /// When a user has finished changing their name
        /// </summary>
        private void changeNameCallback(IAsyncResult res)
        {
            // Get users input
            string newName = Microsoft.Xna.Framework.GamerServices.Guide.EndShowKeyboardInput(res);
            // Check to see if the name is > 8 characters, If so cut the string to the first 8 chars
            if (!String.IsNullOrEmpty(newName))
            {
                if (newName.Length > 8)
                    newName = newName.Substring(0, 8);

                // Update the db entry
                hs.UpdateName(newName);

                // Re-download the data from our server
                hs.DownloadTop10();
                hs.Fetch();

                // Maybe a delay in applying the new name, check to see if we need to re-download again
                if ( hs.UIDExists && hs.DownloadFetchComplete )
                {
                    if ( hs.UserData[ 0 ].Name.CompareTo( newName ) != 0 )
                    {
                        hs.DownloadTop10();
                        hs.Fetch();
                    }
                }
            }
        }

        /// <summary>
        /// Override Update function
        /// We do this so we can check if the download of the top 10 has completed
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            Get();

            fade += ( float )gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Exit Game has been selected
        /// </summary>
        protected override void OnCancel()
        {
            Dummy.Create(ScreenManager, new Background(), new MainMenu());
        }

        /// <summary>
        /// Fill the entries container with the top 10
        /// </summary>
        private void Get()
        {
            if (hs.DownloadTop10Complete)
            {
                // Clear the entries list
                Entries.Clear();
                // Loop thru the top 10 list and add them to the container
                float yPos = 200f;

                if ( hs.Data == null )
                    return;

                for (int i = 0; i < hs.Data.Count; i++)
                {
                    Highscores.highScoreData data = hs.Data[i];

                    int pad = 16 - data.Name.Length;
                    if (i == 9)
                        pad -= 1;

                    HScoreEntry x = new HScoreEntry(
                        ( i + 1 ).ToString() + ". " + data.Name );

                    HScoreEntry y = new HScoreEntry( data.Score );
                    y.ManualPosition = true;

                    y.Position = new Vector2( 320.0f, ( yPos == 200f ) ? 200f : yPos += x.GetHeight( this ) );
                    yPos += 5;

                    if (hs.Data[i].uID == "1")
                    {
                        x.Color = Color.YellowGreen;
                        y.Color = Color.YellowGreen;
                    }

                    Entries.Add( x );
                    Entries.Add( y );

                }

                // Set this to false
                hs.DownloadTop10Complete = false;

                // Since the list is cleared, Add these here
                Entries.Add(refreshEntry);
                Entries.Add(nameEntry);
            }
        }

        /// <summary>
        /// override Draw function so we can add a transition effect
        /// </summary>

        float alpha = 0.0f;
        public override void Draw(GameTime gameTime)
        {
            UpdateEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Begin Rendering
            spriteBatch.Begin();

            alpha = 1.0f;

            Vector4 color = Color.YellowGreen.ToVector4();

            // Transitions

            // Refresh Button
            refreshEntry.Position = new Vector2( 40, 730 );
            nameEntry.Position = new Vector2( 230, 730 );
            Vector2 position = new Vector2( graphics.Viewport.Width / 2, 590 );
            Vector2 scorePosition = new Vector2( graphics.Viewport.Width / 2, 620 );
            Vector2 datePosition = new Vector2( graphics.Viewport.Width / 2, 655 );

            float transitionOffset = ( float )Math.Pow( TransitionPosition, 2 );

            if ( ScreenState == ScreenState.TransitionOn ) {
                refreshEntry.Position = new Vector2( refreshEntry.Position.X - transitionOffset * 256.0f, refreshEntry.Position.Y );
                nameEntry.Position = new Vector2( nameEntry.Position.X - transitionOffset * 256.0f, nameEntry.Position.Y );
                position = new Vector2( position.X - transitionOffset * 256.0f, position.Y );
                scorePosition = new Vector2( scorePosition.X - transitionOffset * 256.0f, scorePosition.Y );
                datePosition = new Vector2( datePosition.X - transitionOffset * 256.0f, datePosition.Y );
            } else {
                refreshEntry.Position = new Vector2( refreshEntry.Position.X + transitionOffset * 512.0f, refreshEntry.Position.Y );
                nameEntry.Position = new Vector2( nameEntry.Position.X + transitionOffset * 512.0f, nameEntry.Position.Y );
                position = new Vector2( position.X + transitionOffset * 512.0f, position.Y );
                scorePosition = new Vector2( scorePosition.X + transitionOffset * 512.0f, scorePosition.Y );
                datePosition = new Vector2( datePosition.X + transitionOffset * 512.0f, datePosition.Y );
            }

            // Draw "Your Position"
            Vector2 origin = this.Font.MeasureString( "Your High Score" ) / 2;
            spriteBatch.DrawString( this.Font, "Your High Score", position, new Color( color * alpha ),
                0f, origin, 1f, SpriteEffects.None, 0f );

            // Draw users personal score
            if ( hs.UIDExists || hs.DownloadFetchComplete )
            {
                if ( hs.DownloadFetchComplete ) {
                    string text = hs.UserData[ 0 ].Name + " - " + hs.UserData[ 0 ].Score;
                    string date = hs.UserData[ 0 ].Timestamp;

                    spriteBatch.DrawString( this.Font, text, scorePosition,
                        new Color( color * alpha ),
                        0f, this.Font.MeasureString( text ) / 2, 1f, SpriteEffects.None, 0f );

                    spriteBatch.DrawString( this.Font, date, datePosition,
                        new Color( color * alpha ), 0f,
                        this.Font.MeasureString( date ) / 2, 1f, SpriteEffects.None, 0f );
                } else {
                    hs.Refreshing = true;
                }
            }
            else if ( !hs.Refreshing )
            {
                string text = "No Highscore Found";

                spriteBatch.DrawString( this.Font, text, scorePosition,
                    new Color( color * alpha ),
                    0f, this.Font.MeasureString( text ) / 2, 1f, SpriteEffects.None, 0f );
            }
            if ( hs.Refreshing )
            {
                string text = "Refreshing...";

                spriteBatch.DrawString( this.Font, text, scorePosition,
                    new Color( color * alpha ),
                    0f, this.Font.MeasureString( text ) / 2, 1f, SpriteEffects.None, 0f );
            }

            // End Rendering
            spriteBatch.End();

            base.Draw(gameTime);

            // Transition the game screen off
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1.0f - (1.0f - TransitionPosition));
        }
    }
}
