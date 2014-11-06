//-----------------------------------------------------------------------------
// MainMenu.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace Halcyon
{
    /// <summary>
    /// Main menu screen
    /// </summary>
    class MainMenu : MenuScreen
    {
        // Download top 10 high scores
        Highscores hs;

        // Twitter
        private Texture2D twitterTexture;
        private WebBrowserTask twitterWeb;
        private SpriteFont font;

        public MainMenu()
            : base("Halcyon", 1.5f)
        {

            entryPosition = 300.0f;
            //menuEntryPadding = 10;

            MenuEntry playGameEntry = new MenuEntry("Play");
            MenuEntry highScoreEntry = new MenuEntry("Scores");
            MenuEntry powerupsEntry = new MenuEntry( "Powerups" );
            MenuEntry optionsEntry = new MenuEntry("Options");
            MenuEntry helpEntry = new MenuEntry( "About" );

            playGameEntry.Selected += PlayGameEntrySelected;
            highScoreEntry.Selected += HighScoreEntrySelected;
            optionsEntry.Selected += OptionsEntrySelected;
            powerupsEntry.Selected += PowerupsEntrySelected;
            helpEntry.Selected += HelpSelected;

            MenuEntries.Add(playGameEntry);
            MenuEntries.Add(highScoreEntry);
            MenuEntries.Add( powerupsEntry );
            MenuEntries.Add(optionsEntry);
            MenuEntries.Add( helpEntry );
        }
        #pragma warning disable 0618
        public override void LoadContent()
        {
            base.LoadContent();

            // Download top 10 high scores
            // Do it here so if user connection is slow
            // we aren't drawing no text at all when highscore screen is loaded
            hs = new Highscores();
            hs.DownloadTop10();
            hs.Fetch();
            hs.CheckuID();
            
            // Font
            font = content.Load<SpriteFont>( "Font/default" );

            twitterTexture = content.Load<Texture2D>("Menu/twitter-banner");

            twitterWeb = new WebBrowserTask();
            twitterWeb.Uri = new Uri("http://www.twitter.com/addictionsoft", UriKind.Absolute);
        }

        /// <summary>
        /// Allows the screen to create the hit bounds for a particular menu entry
        /// </summary>
        protected virtual Rectangle GetTwitterHitBounds()
        {
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;

            return new Rectangle( graphics.Viewport.Width - ( int )( twitterTexture.Width * 0.75 ), graphics.Viewport.Height - ( int )( 50.0f * 0.75f ),
                ( int )( twitterTexture.Width * 0.75f ), ( int )( twitterTexture.Height * 0.75f ) );
        }

        public override void HandleInput(InputState input, GameTime gameTime)
        {
            base.HandleInput(input, gameTime);

            // Look for any touch screen taps that occurred
            foreach (GestureSample gesture in input.gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    // Convert the tap position to a point
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                    // Did they tap the Twitter banner?
                    if (GetTwitterHitBounds().Contains(tapLocation))
                    {
                        twitterWeb.Show();
                    }
                }
            }
        }

        /// <summary>
        /// Play Game has been selected
        /// </summary>
        void PlayGameEntrySelected(object sender, EventArgs e)
        {
            Dummy.Create(ScreenManager, new Background(), new Game(hs));
        }

        /// <summary>
        /// High Score has been selected
        /// </summary>
        void HighScoreEntrySelected(object sender, EventArgs e)
        {
            if ( !com.addictionsoftware.Network.HasConnection() ) {
                System.Windows.MessageBox.Show( "It doesn't seem like you have an active internet connection.\n\nBecause of this the highscores page has been disabled.", "Alert!", System.Windows.MessageBoxButton.OK );
                return;
            } else if ( !com.addictionsoftware.Network.HasCommunication() ) {
                System.Windows.MessageBox.Show( "We are having trouble contacting the addictionsoftware servers.\n"
                + "Until this issue is resolved certain features will be disabled.\n\nWe are sorry for any inconvenience." );
                return;
            }

            Dummy.Create( ScreenManager, new Background(), new Highscore( hs ) );
        }

        /// <summary>
        /// Options has been selected
        /// </summary>
        void OptionsEntrySelected(object sender, EventArgs e)
        {
            Dummy.Create( ScreenManager, new Background(), new Options() );
        }

        /// <summary>
        /// Exit Game has been selected
        /// </summary>
        void PowerupsEntrySelected(object sender, EventArgs e)
        {
            Dummy.Create( ScreenManager, new Background(), new Powerups() );
        }

        /// <summary>
        /// About page was selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HelpSelected( object sender, EventArgs e ) {
            Dummy.Create( ScreenManager, new Background(), new About() );
        }

        /// <summary>
        /// Buttons.Back button has been pressed
        /// </summary>
        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
        }

        /// <summary>
        /// override Draw function so we can add a transition effect
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Begin rendering
            spriteBatch.Begin();

            Vector2 twitterPosition = 
                new Vector2( graphics.Viewport.Width - ( twitterTexture.Width * 0.75f ) - 5.0f, graphics.Viewport.Height - ( 50f * 0.75f ) );

            float transitionOffset = ( float )Math.Pow( TransitionPosition, 2 );

            if ( ScreenState == ScreenState.TransitionOn ) {
                twitterPosition = new Vector2( twitterPosition.X - transitionOffset * 256.0f, twitterPosition.Y );
            } else {
                twitterPosition = new Vector2( twitterPosition.X + transitionOffset * 512.0f, twitterPosition.Y );
            }

            spriteBatch.Draw( twitterTexture, twitterPosition, null, Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f );

            // Version information
            string name = System.Reflection.Assembly.GetExecutingAssembly().FullName;
            System.Reflection.AssemblyName assName = new System.Reflection.AssemblyName( name );

            Vector2 verPos = new Vector2( 5.0f, ScreenManager.GraphicsDevice.Viewport.Height - ( font.LineSpacing * 0.65f ) );

            if ( ScreenState == ScreenState.TransitionOn )
                verPos = new Vector2( verPos.X - transitionOffset * 256.0f, verPos.Y );
            else
                verPos = new Vector2( verPos.X + transitionOffset * 512.0f, verPos.Y );

            spriteBatch.DrawString( font, "Version: " + assName.Version, verPos, Color.Gray, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.0f );


            // end rendering
            spriteBatch.End();

            // Transition the game screen off
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1.0f - (1.0f - TransitionPosition));
        }
    }
}
