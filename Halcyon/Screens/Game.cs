//-----------------------------------------------------------------------------
// Game.cs
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices;
using System.IO.IsolatedStorage;

namespace Halcyon
{
    /// <summary>
    /// Main Gameplay screen
    /// </summary>
    class Game : Screen
    {
        // Content Manager
        public ContentManager content;
        // Player Class
        public Player player;
        // Enemy Class
        public Enemy enemy; 
        // Explosion Animation
        Texture2D explosionTexture;
        public List<Animation> explosions;

        Texture2D impactTexture;
        public List<Animation> impacts;

        // Power up - Shield
        public Shield powerShield;
        // Power up - Life
        ExtraLife powerLife;
        // Power up - Bomb
        Bomb powerBomb;

        // Current player Score
        public float playerScore;
        // Static access to the Game Class
        public static Game current { get; private set; }
        // Gets/Sets the state of pause
        public bool isPaused = false;
        // Check to see if the screen needs to be shaken
        public bool shake = false;
        // Texture for shaded paused state
        private Texture2D backgroundOverlayTexture;
        // Highscore data
        public Highscores playerHS;

        // bomb count
        public int bombCount = 1;
        private Animation animBombActivated;

        // hud font
        private SpriteFont hudFont;

        private bool hasLeveled = false;
        private float fadeDelay = 0f;

        private Sprite heartLife;
        private Sprite nukeBomb;

        private float timeEnemyIncrease = 0.0f;

        /// <summary>
        /// Construct a game screen
        /// </summary>
        public Game(Highscores hs)
        {
            TransitionTimeOff = TimeSpan.FromSeconds(0.5);
            TransitionTimeOn = TimeSpan.FromSeconds(1.5);

            // Reset static objects
            current = this;
            playerHS = hs;

            isBlurred = true;

            explosions = new List<Animation>();
            impacts = new List<Animation>();

            player = new Player();
            enemy = new Enemy();
        }

        /// <summary>
        /// Load graphics content for the game
        /// </summary>
        public override void LoadContent()
        {
            // Setup Content
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            EnabledGestures = GestureType.DoubleTap;

            // Create Overlay, Used when game state is paused
            backgroundOverlayTexture = content.Load<Texture2D>("Background/blank");

            // Load all the assets for the particle system, so we don't lag during gameplay
            content.Load<Texture2D>("Player/laser");
            content.Load<Texture2D>("Player/laser-green");
            content.Load<Texture2D>("Player/laser-red");
            content.Load<Texture2D>("Player/plasma");
            content.Load<Texture2D>("Player/shield");
            content.Load<Texture2D>("Player/life");
            content.Load<Texture2D>("Player/bomb");
            heartLife = new Sprite( content.Load<Texture2D>( "Player/heart" ) );
            nukeBomb = new Sprite( content.Load<Texture2D>( "Player/nuke" ) );

            content.Load<Texture2D>("Player/bomb_activated");
            animBombActivated = new Animation();

            content.Load<Texture2D>("Animations/explosion-1");
            content.Load<Texture2D>("Animations/explosion-2");
            content.Load<Texture2D>("Animations/explosion-3");
            content.Load<Texture2D>("Animations/explosion-4");
            content.Load<Texture2D>("Animations/player-explosion");
            content.Load<Texture2D>( "Animations/booster" );

            powerShield = new Shield(content.Load<Texture2D>("Animations/shield_powerup"));
            powerLife = new ExtraLife(content.Load<Texture2D>("Animations/life_powerup"));
            powerBomb = new Bomb(content.Load<Texture2D>("Animations/bomb_powerup"));
            impactTexture = content.Load<Texture2D>("Animations/impact");

            // Hud Font
            hudFont = content.Load<SpriteFont>("Font/hudfont");

            // Reset Elapsed time, Starting the game now :)
            ScreenManager.Game.ResetElapsedTime();

            // Load Player/Enemy Content
            player.LoadContent(content);
            enemy.LoadContent(content);
        }

        public void AddPlayerExplosion(Vector2 position)
        {
            Animation explosion = new Animation();

            explosionTexture = content.Load<Texture2D>("Animations/player-explosion");

            explosion.Initialize(explosionTexture, position, 128, 128, 16, 45, Color.White, 1.5f, false);
            explosions.Add(explosion);
        }

        public void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();

            int numExplosion = RandomMath.Random.Next(1, 4);
            explosionTexture = content.Load<Texture2D>("Animations/explosion-" + numExplosion);

            explosion.Initialize(explosionTexture, position, 128, 128, 16, 32, Color.White, 1f, false);
            explosions.Add( explosion );
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                    explosions.RemoveAt(i);
            }
        }

        public void AddImpact(Vector2 position, Color color, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Animation impact = new Animation();
                impact.Initialize(impactTexture, position, 64, 64, 16, 20, color, 1f, false);
                impacts.Add(impact);
            }
        }

        private void UpdateImpacts(GameTime gameTime)
        {
            for (int i = impacts.Count - 1; i >= 0; i--)
            {
                impacts[i].Update(gameTime);
                if (impacts[i].Active == false)
                    impacts.RemoveAt(i);
            }
        }
        public void runWeaponCheck()
        {
            if (playerScore >= 4200 && playerScore < 12000)
            {
                Game.current.player.PlayerWeapon = new DoubleLaserWeapon(Game.current.player);
            }
            else if (playerScore >= 12000 && playerScore < 25000)
            {
                Game.current.player.PlayerWeapon = new TripleLaserWeapon(Game.current.player);
            }
            else if (playerScore >= 25000)
            {
                Game.current.player.PlayerWeapon = new PlasmaWeapon(Game.current.player);
            }
        }

        private float timeSinceLastPower = 0f;
        private float timeSinceLastUpdateShake = 0f;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive && isPaused == false)
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Increase Player score
                playerScore += 3;

                // Power ups
                // Spawn between 20-30s (this needs tweaking)
                timeSinceLastPower += elapsedTime;
                if (timeSinceLastPower > RandomMath.RandomBetween(10, 20))
                {
                    int valRan = RandomMath.Random.Next(1, 400);
                    if (valRan > 50 && valRan < 130) // Shield
                    {
                        powerShield.CreatePower(new Vector2(RandomMath.RandomBetween(10 + powerShield.animShield.FrameWidth,
                            Game.current.ScreenManager.GraphicsDevice.Viewport.Width -
                            (powerShield.animShield.FrameWidth + 10)), 0f - powerShield.animShield.FrameHeight));
                    }
                    if ((valRan > 130 && valRan < 180) && player.PlayerLives < 5) // Life
                    {
                        powerLife.CreatePower(new Vector2(RandomMath.RandomBetween(10 + powerLife.animLife.FrameWidth,
                           Game.current.ScreenManager.GraphicsDevice.Viewport.Width -
                           (powerLife.animLife.FrameWidth + 10)), 0f - powerLife.animLife.FrameHeight));
                    }
                    else if ((valRan > 200 && valRan < 350) && bombCount < 2) // bomb
                    {
                        powerBomb.CreatePower(new Vector2(RandomMath.RandomBetween(10 + powerBomb.animBomb.FrameWidth,
                           Game.current.ScreenManager.GraphicsDevice.Viewport.Width -
                           (powerBomb.animBomb.FrameWidth + 10)), 0f - powerBomb.animBomb.FrameHeight));
                    }

                    timeSinceLastPower = 0f;
                }

                powerShield.Update(gameTime);
                powerLife.Update(gameTime);
                powerBomb.Update(gameTime);

                // Update Enemy Spawns
                timeEnemyIncrease += elapsedTime;
                if ( timeEnemyIncrease > 10 && playerScore < 30000 ) {

                    enemy.enemySpeed += 0.1f;
                    enemy.spawnTime -= 25;

                    timeEnemyIncrease = 0.0f;
                }

                // Update player level
                if (playerScore >= 4200 && playerScore < 4500)
                {
                    hasLeveled = true;
                    playerScore += 300;
                    player.PlayerWeapon = new DoubleLaserWeapon(player);
                }
                else if (playerScore >= 12000 && playerScore < 12600)
                {
                    hasLeveled = true;
                    playerScore += 600;
                    player.PlayerWeapon = new TripleLaserWeapon(player);
                }
                else if (playerScore >= 25000 && playerScore <= 25900)
                {
                    hasLeveled = true;
                    playerScore += 900;
                    player.PlayerWeapon = new PlasmaWeapon(player);
                }

                // Run an update to turn off the level up text
                if (hasLeveled)
                {
                    // Set fade delay
                    fadeDelay += elapsedTime;
                    // Run a timer of 1s
                    timeSinceLastUpdateShake += elapsedTime;
                    if (timeSinceLastUpdateShake > 1.5f)
                    {
                        hasLeveled = false;
                        fadeDelay = 0f;
                        timeSinceLastUpdateShake = 0f;
                    }
                }

                // Update player/enemies
                player.Update(gameTime);
                enemy.Update(gameTime);
                UpdateExplosions(gameTime);
                UpdateImpacts(gameTime);
                animBombActivated.Update(gameTime);

                // Run an update to turn shake off if it has been enabled
                if (shake)
                {
                    // Run the shake for the same amount of time we are vibrating
                    timeSinceLastUpdateShake += elapsedTime;
                    if (timeSinceLastUpdateShake > 1.5f)
                    {
                        shake = false;
                        VibrateController vc = VibrateController.Default;
                        vc.Stop();
                        timeSinceLastUpdateShake = 0f;
                    }
                }

                if ( player.IsAlive ) {
                    // Loop thru enemies for Collision detection
                    foreach ( Enemy.Enemies tmpEnemy in enemy.EnemiesList ) {
                        // Collided with the player!
                        if ( tmpEnemy.enemyData.enemySprite.CollisionCheck( Game.current.player.playerSprite ) ) {
                            // Does he have a shield up?
                            if ( powerShield.shieldActive == false && player.deadShieldActive == false ) {
                                if ( player.IsAlive == false )
                                    break;

                                // Player has died!
                                if ( Game.current.player.PlayerLives >= 0 )
                                    Game.current.player.PlayerLives--;

                                // Call OnDead()
                                player.OnDead();

                                bombCount = 0;

                                // Shake the screen to visualize a vibrate effect
                                shake = true;

                                // Player is dead so we kill all enemies that are currently active
                                foreach ( Enemy.Enemies e in enemy.EnemiesList ) {
                                    e.OnDead();

                                    // no fct, you don't gain exp for death

                                    // Remove them from the list
                                    enemy.EnemiesList.QueuePendingRemoval( e );
                                    enemy.curActive--;
                                }
                            } else {

                                playerScore += tmpEnemy.enemyData.Experience;

                                tmpEnemy.OnDead();

                                Enemy.fct_data a = new Enemy.fct_data();

                                a.fct_position = tmpEnemy.enemyData.enemySprite.Position;
                                a.fct_flag = true;
                                a.fct_text = tmpEnemy.enemyData.Experience.ToString();

                                enemy.fct_list.Add( a );

                                enemy.EnemiesList.QueuePendingRemoval( tmpEnemy );
                                enemy.curActive--;
                            }
                        }
                    }
                }

                foreach ( Enemy.Enemies tmpEnemy in enemy.EnemiesList ) {
                    // Projectile Collision
                    foreach ( Projectile projectile in player.Projectiles ) {
                        if ( projectile.ProjectileSprite.CollisionCheck( tmpEnemy.enemyData.enemySprite ) ) {
                            player.Projectiles.QueuePendingRemoval( projectile );

                            // Do damage to the enemy
                            tmpEnemy.enemyData.Health -= projectile.Damage;

                            projectile.OnHit();

                            // Did the enemy die?
                            if ( tmpEnemy.enemyData.Health <= 0 ) {
                                // Increase score by XP
                                playerScore += tmpEnemy.enemyData.Experience;

                                tmpEnemy.OnDead();

                                Enemy.fct_data a = new Enemy.fct_data();

                                a.fct_position = tmpEnemy.enemyData.enemySprite.Position;
                                a.fct_flag = true;
                                a.fct_text = tmpEnemy.enemyData.Experience.ToString();

                                enemy.fct_list.Add( a );

                                enemy.EnemiesList.QueuePendingRemoval( tmpEnemy );
                                enemy.curActive--;
                            }
                        }
                    }
                }

                enemy.EnemiesList.ApplyPendingRemovals();
                player.Projectiles.ApplyPendingRemovals();
            }
        }

        /// <summary>
        /// Lets the game respond to player input
        /// </summary>
        public override void HandleInput(InputState input, GameTime gameTime)
        {
            if (input == null)
                throw new ArgumentNullException("No Input");

            if (IsActive && isPaused == false)
            {
                // User hit Back button, Close/Remove Game screen, This will go back to the main menu
                if (input.IsNewButtonPress(Buttons.Back))
                {
                    // Set Paused state to true, Pausing the game play
                    isPaused = true;
                    ScreenManager.Add( new Paused() );

                    Effect backEffect = new Effect( content.Load<SoundEffect>( "Sound/back" ) );
                    backEffect.Play();
                }

                if (bombCount > 0 && player.IsAlive )
                {
                    foreach (GestureSample gesture in input.gestures)
                    {
                        if (gesture.GestureType == GestureType.DoubleTap)
                        {
                            animBombActivated.Initialize(content.Load<Texture2D>("Player/bomb_activated"),
                                player.playerSprite.Position, 128, 128, 16, 30, Color.White, 1f, false);

                            bombCount--;

                            shake = true;

                            // Play sound
                            Effect bombEffect = new Effect( content.Load<SoundEffect>( "Sound/bomb_activ" ) );
                            bombEffect.Play();

                            // Kill all enemies
                            foreach (Enemy.Enemies e in enemy.EnemiesList)
                            {
                                e.OnDead();

                                Enemy.fct_data a = new Enemy.fct_data();

                                a.fct_position = e.enemyData.enemySprite.Position;
                                a.fct_flag = true;
                                a.fct_text = e.enemyData.Experience.ToString();

                                enemy.fct_list.Add( a );

                                // Remove them from the list
                                enemy.EnemiesList.QueuePendingRemoval(e);
                                enemy.curActive--;
                            }
                        }
                    }
                }

                player.HandleInput(Accelerometer.GetState(), TouchPanel.GetState(), input);
            }
        }

        /// <summary>
        /// Draw the interface
        /// </summary>
        private void DrawHUD(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw hearts
            Vector2 heartPosition = new Vector2( 0, ( ScreenManager.GraphicsDevice.Viewport.Height - heartLife.Height ) + 15 );
            heartLife.Color = new Color( 153, 153, 153, 153 );
            for ( int i = 0; i < player.PlayerLives; i++ ) {
                heartLife.Draw( gameTime, spriteBatch, SpriteEffects.None );
                heartPosition.X += heartLife.Width;
                heartLife.Position = heartPosition;
            }

            // Bombs
            Vector2 bombPositon = new Vector2( ( ScreenManager.GraphicsDevice.Viewport.Width / 2 ) - 35,
                ( ScreenManager.GraphicsDevice.Viewport.Height - nukeBomb.Height ) + 15 );
            nukeBomb.Color = new Color( 153, 153, 153, 153 );
            if ( bombCount > 0 ) {
                for ( int i = 0; i < bombCount; i++ ) {
                    nukeBomb.Draw( gameTime, spriteBatch, SpriteEffects.None );
                    bombPositon.X += nukeBomb.Width;
                    nukeBomb.Position = bombPositon;
                }
            }

            // Score
            Vector2 scorePosition = new Vector2( 
                ScreenManager.GraphicsDevice.Viewport.Width - this.hudFont.MeasureString(playerScore.ToString()).X - 10, 
                ScreenManager.GraphicsDevice.Viewport.Height - this.hudFont.LineSpacing );
            Color scoreColor = new Color( 153, 153, 153, 153 );
            spriteBatch.DrawString( this.hudFont, playerScore.ToString(), scorePosition, scoreColor );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        public override void Draw( GameTime gameTime )
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            Matrix transformMatrix = Matrix.CreateTranslation(new Vector3(  
                (float)((RandomMath.Random.NextDouble() * 10) - 1),  
                (float)((RandomMath.Random.NextDouble() * 10) - 1), 0));

            // Begin rendering
            if (shake && (isPaused == false))
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null, transformMatrix);

                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                if ( bool.Parse( settings[ "Vibrate" ].ToString() ) == true ) {
                    VibrateController vc = VibrateController.Default;
                    vc.Start( new TimeSpan( 00, 00, 01 ) );
                }
            }
            else
                spriteBatch.Begin();

            // Power Ups
            powerShield.Draw(spriteBatch);
            powerLife.Draw(spriteBatch);
            powerBomb.Draw(spriteBatch);

            animBombActivated.Draw(spriteBatch);

            // Draw Player / Enemies
            if ( player.IsAlive )
                player.Draw( gameTime, spriteBatch );

            enemy.Draw(gameTime, spriteBatch);

            // Interface
            DrawHUD( gameTime, spriteBatch );

            if (hasLeveled)
            {
                // player gained a new level, display something pretty
            }

            for (int i = 0; i < impacts.Count; i++)
            {
                impacts[i].Draw(spriteBatch);
            }

            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);
            }

            foreach ( Enemy.fct_data info in enemy.fct_list )
            {
                if ( info.fct_flag )
                {
                    Vector2 fct_pos = info.fct_position;
                    fct_pos.Y -= 60;
                    fct_pos.X -= 15;

                    // Fade text in/out
                    float time = info.fct_time / 1.0f;
                    float alpha = 4 * time * ( 1 - time );

                    Vector4 color = new Color( 153, 153, 153, 153 ).ToVector4();

                    spriteBatch.DrawString( enemy.fct_font, info.fct_text,
                        fct_pos, new Color( color * alpha ) );
                }
            }

            if ( !player.IsAlive ) {
                spriteBatch.DrawString( hudFont, player.deadText,
                    new Vector2(
                        ( ScreenManager.GraphicsDevice.Viewport.Width / 2 ) -
                        ( hudFont.MeasureString( player.deadText ).X ),
                        ScreenManager.GraphicsDevice.Viewport.Height / 2 ), Color.Red, 0.0f, Vector2.Zero, 2f,
                        SpriteEffects.None, 0.0f );
            }

            // Dark Overlay (ONLY DRAWN WHEN PAUSED)
            if (isPaused)
            {
                // Draw Overlay
                spriteBatch.Draw(backgroundOverlayTexture,
                    new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width,
                        ScreenManager.GraphicsDevice.Viewport.Height), Color.Black * 0.65f);
            }

            spriteBatch.End();

            // Transition the game screen off
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1.0f - (1.0f - TransitionPosition));
        }
    }
}
