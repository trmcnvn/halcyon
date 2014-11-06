//-----------------------------------------------------------------------------
// ScreenManager.cs
//-----------------------------------------------------------------------------

// Namespace Usage
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using com.addictionsoftware;

namespace Halcyon
{
    /// <summary>
    /// ScreenManager manages all Game screen instances
    /// Calls their update/draw methods at appropriate times
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        // Lists to store all Screen information
        public Screen[] GetScreens
        {
            get { return screens.ToArray(); }
        }
        private List<Screen> screens = new List<Screen>();
        private List<Screen> screensToUpdate = new List<Screen>();

        public struct gameStatusData
        {
            public int lives { get; set; }
            public float score { get; set; }
            public int bombs { get; set; }
            public int weapon { get; set; }
            public int spawnTime { get; set; }
            public float spawnSpeed { get; set; }
            public Vector2 playerPosition { get; set; }
        }
        gameStatusData gameStatus = new gameStatusData();

        Enemy.EnemyData enemySaveData = new Enemy.EnemyData();
       

        // Gets current input
        private InputState input = new InputState();

        // File to store data in
        private const string StateFilename = "ScreenManagerState.xml";
        // Check to see if ScreenManager has been initialized
        private bool isInitialized = false;

        // Texture used for fading transitions
        private Texture2D blankTexture;

        /// <summary>
        /// Gets/Sets the current font to use for all screens
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }
        private SpriteFont font;

        /// <summary>
        /// Default sprite batch to be shared across all screens
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }
        private SpriteBatch spriteBatch;

        public RenderTarget2D current_render;
        public RenderTarget2D previous_render;
        
        Microsoft.Xna.Framework.Game game;

        /// <summary>
        /// Constructs a new screen manager component
        /// </summary>
        public ScreenManager(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
            this.game = game;
            TouchPanel.EnabledGestures = GestureType.None;
        }

        /// <summary>
        /// Initializes the screen manager component
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // Make sure we have not initialized before
            if (isInitialized)
                throw new InvalidOperationException("Initialize can only be called once");

            // Set isInitialized to true
            isInitialized = true;
        }

        /// <summary>
        /// Load your graphics content
        /// </summary>
        protected override void LoadContent()
        {
            // Set sprite batch
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Set default font
            font = Game.Content.Load<SpriteFont>("Font/menufont");
            blankTexture = Game.Content.Load<Texture2D>("Background/blank");

            // Tell each screen to load it's content
            foreach (Screen screen in screens)
            {
                screen.LoadContent();
                screen.Activate(false);
            }
        }

        /// <summary>
        /// Unload your graphics content
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each screen to unload it's content
            foreach (Screen screen in screens)
                screen.UnloadContent();
        }

        /// <summary>
        /// Allows each screen to run logic
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Update input
            input.Update();

            // Create a copy of the screens list to avoid confusion
            screensToUpdate.Clear();

            foreach (Screen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as their are screens to be updated
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the queue
                Screen screen = screensToUpdate[screensToUpdate.Count - 1];
                // Remove it from the queue
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input, gameTime);
                        otherScreenHasFocus = true;
                    }

                    // If this screen isn't a pop up, Then inform other active screens they are covered
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
        }

        /// <summary>
        /// Tells each screen to draw itself
        /// </summary>
        public override void Draw( GameTime gameTime )
        {
            SpriteBatch spriteBatch = new SpriteBatch( GraphicsDevice );
            spriteBatch.Begin();

            for ( int i = 0; i < screens.Count; ++i ) {
                if ( screens[ i ].ScreenState == ScreenState.Hidden )
                    continue;

                screens[ i ].Draw( gameTime );
            }

            spriteBatch.End();
        }

        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;
            // Begin rendering
            spriteBatch.Begin();
            // Draw
            spriteBatch.Draw(blankTexture,
                new Rectangle(0, 0, viewport.Width, viewport.Height), Color.Black * alpha);
            // End rendering
            spriteBatch.End();
        }

        /// <summary>
        /// Adds a new screen to the screen manager
        /// </summary>
        public void Add(Screen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If ScreenManager has been initialized, call LoadContent of our Screen
            if (isInitialized)
            {
                screen.LoadContent();
                screen.Activate(false);
            }

            // Add screen to List
            screens.Add(screen);

            // Update TouchPanel gestures
            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }

        /// <summary>
        /// Removes a screen from the screen manager
        /// </summary>
        public void Remove(Screen screen)
        {
            // If screen manager has been initialized, call UnloadContent
            if (isInitialized)
                screen.UnloadContent();

            // Remove screen from our list
            screens.Remove(screen);
            screensToUpdate.Remove(screen);

            // Update TouchPanel gestures
            if (screens.Count > 0)
                TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
        }

        public void Deactivate()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Create an XML document to hold the list of screen types currently in the stack
                XDocument doc = new XDocument();
                XElement root = new XElement("ScreenManager");
                doc.Add(root);

                // Make a copy of the master screen list, to avoid confusion if
                // the process of deactivating one screen adds or removes others.
                screensToUpdate.Clear();
                foreach (Screen screen in screens)
                    screensToUpdate.Add(screen);

                // Iterate the screens to store in our XML file and deactivate them
                foreach (Screen screen in screensToUpdate)
                {
                    // Only add the screen to our XML if it is serializable
                    if (screen.IsSerializable)
                    {
                        root.Add(new XElement(
                            "Screen",
                            new XAttribute("Type", screen.GetType().AssemblyQualifiedName)
                            ));
                    }

                    // Deactivate the screen regardless of whether we serialized it
                    screen.Deactivate();
                }
                if (Halcyon.Game.current != null)
                {
                    root.Add(new XElement("Saved", new XAttribute("bombs", Halcyon.Game.current.bombCount)
                                                 , new XAttribute("lives", Halcyon.Game.current.player.PlayerLives)
                                                 , new XAttribute("score", Halcyon.Game.current.playerScore)
                                                 , new XAttribute("spawn", Halcyon.Game.current.enemy.spawnTime)
                                                 , new XAttribute("speed", Halcyon.Game.current.enemy.enemySpeed)
                                                 , new XAttribute("playerY", Halcyon.Game.current.player.playerSprite.Position.Y)
                                                 , new XAttribute("playerX", Halcyon.Game.current.player.playerSprite.Position.X)
                                                 ));


                    for (int x = 0; x < Halcyon.Game.current.enemy.EnemiesList.Count; x++)
                    {
                        root.Add(new XElement("Enemy",
                            new XAttribute("Health", Halcyon.Game.current.enemy.EnemiesList[x].enemyData.Health),
                            new XAttribute("Texture", Halcyon.Game.current.enemy.EnemiesList[x].enemyData.enemyTexture),
                            new XAttribute("PositionX", Halcyon.Game.current.enemy.EnemiesList[x].enemyData.enemySprite.Position.X),
                            new XAttribute("PositionY", Halcyon.Game.current.enemy.EnemiesList[x].enemyData.enemySprite.Position.Y),
                            new XAttribute("Velocity", Halcyon.Game.current.enemy.EnemiesList[x].enemyData.enemySprite.Velocity.X),
                            new XAttribute("Sprite", Halcyon.Game.current.enemy.EnemiesList[x].enemyData.enemySprite)
                        ));

                    }
                }


                // Save the document
                using (IsolatedStorageFileStream stream = storage.CreateFile(StateFilename))
                {
                    doc.Save(stream);
                }
            }

        }

        public bool Activate(bool instancePreserved)
        {            
            if (instancePreserved)
            {
                // Make a copy of the master screen list, to avoid confusion if
                // the process of activating one screen adds or removes others.
                screensToUpdate.Clear();

                foreach (Screen screen in screens)
                    screensToUpdate.Add(screen);

                foreach (Screen screen in screensToUpdate)
                    screen.Activate(true);
            }

             // Otherwise we need to refer to our saved file and reconstruct the screens that were present
            // when the game was deactivated.
            else
            {
                // Try to get the screen factory from the services, which is required to recreate the screens
                IScreenFactory screenFactory = Game.Services.GetService(typeof(IScreenFactory)) as IScreenFactory;
                if (screenFactory == null)
                {
                    throw new InvalidOperationException(
                        "Game.Services must contain an IScreenFactory in order to activate the ScreenManager.");
                }

                // Open up isolated storage
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    // Check for the file; if it doesn't exist we can't restore state
                    if (!storage.FileExists(StateFilename))
                        return false;

                    // Read the state file so we can build up our screens
                    using (IsolatedStorageFileStream stream = storage.OpenFile(StateFilename, FileMode.Open))
                    {
                        XDocument doc = XDocument.Load(stream);

                        foreach (XElement scoreElem in doc.Root.Elements("Saved"))
                        {
                            gameStatus.score = float.Parse(scoreElem.Attribute("score").Value);
                            gameStatus.bombs = int.Parse(scoreElem.Attribute("bombs").Value);
                            gameStatus.lives = int.Parse(scoreElem.Attribute("lives").Value);
                            gameStatus.spawnSpeed = float.Parse(scoreElem.Attribute("speed").Value);
                            gameStatus.spawnTime = int.Parse(scoreElem.Attribute("spawn").Value);
                            gameStatus.playerPosition = new Vector2(float.Parse(scoreElem.Attribute("playerX").Value), float.Parse(scoreElem.Attribute("playerY").Value));
                        }

                        // Iterate the document to recreate the screen stack
                        foreach (XElement screenElem in doc.Root.Elements("Screen"))
                        {
                            // Use the factory to create the screen
                            Type screenType = Type.GetType(screenElem.Attribute("Type").Value);
                            Screen screen = screenFactory.CreateScreen(screenType, gameStatus);

                            // Add the screen to the screens list and activate the screen
                            screen.ScreenManager = this;
                            screens.Add(screen);
                            screen.LoadContent();
                            screen.Activate(false);

                            // update the TouchPanel to respond to gestures this screen is interested in
                            TouchPanel.EnabledGestures = screen.EnabledGestures;
                        }

                        foreach ( XElement scoreElem in doc.Root.Elements( "Enemy" ) ) {
                            float x = float.Parse( scoreElem.Attribute( "PositionX" ).Value );
                            float y = float.Parse( scoreElem.Attribute( "PositionY" ).Value );
                            enemySaveData.enemyTexture = scoreElem.Attribute("Texture").Value;
                            enemySaveData.enemySprite = new Sprite(Game.Content.Load<Texture2D>("Enemy/" + enemySaveData.enemyTexture));
                            enemySaveData.enemySprite.Position = new Vector2(x, y);

                            enemySaveData.enemySprite.Velocity = new Vector2(float.Parse(scoreElem.Attribute("Velocity").Value), gameStatus.spawnSpeed);
                            enemySaveData.Health = float.Parse(scoreElem.Attribute("Health").Value);
                            enemySaveData.Experience = 25;
                            enemySaveData.enemySprite.Scale = 0.9f;

                            Halcyon.Game.current.enemy.EnemiesList.Add(new Enemy.Enemies(enemySaveData));
                        }
                        Halcyon.Game.current.player.playerSprite.Position = gameStatus.playerPosition;
                    }
                }
            }

            return true;

        }
    }
}
