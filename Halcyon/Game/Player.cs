//-----------------------------------------------------------------------------
// Player.cs
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;

namespace Halcyon
{
    class Player
    {
        // Viewport
        private Viewport viewport;
        // Sprite for the player's ship
        public Sprite playerSprite;
        // Elapsed Time
        private float elapsedTime;

        // Player Weapon
        public Weapon PlayerWeapon
        {
            get { return playerWeapon; }
            set { playerWeapon = value; }
        }
        private Weapon playerWeapon;

        // Position for player sprite
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        private Vector2 position;

        // Velocity for player sprite
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        private Vector2 velocity;

        // Alive check for player
        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }
        private bool isAlive;

        // Current player lives left
        public int PlayerLives
        {
            get { return playerLives; }
            set { playerLives = value; }
        }
        private int playerLives = 3;

        /// <summary>
        /// All of the projectiles fired by this ship.
        /// </summary>
        private ListRemovalCollection<Projectile> projectiles =
            new ListRemovalCollection<Projectile>();
        public ListRemovalCollection<Projectile> Projectiles
        {
            get { return projectiles; }
        }

        Animation playerBooster;

        private float timeDead = 0.0f;
        public string deadText = "You Died!";
        public Animation deadShield;
        public bool deadShieldActive = false;
        private float deadShieldTime = 0.0f;
        private Texture2D shieldTexture;

        // Movement configuration
        private const float accelerometerScale = 1.5f;
        private const float playerAcceleration = 25000.0f;
        private const float playerMaxSpeed = 1750.0f;
        private float playerMovement;
            
        /// <summary>
        /// LoadContent will be called once per game
        /// Load all of your content here
        /// </summary>
        private ContentManager content;
        public void LoadContent(ContentManager content)
        {
            this.content = content;
            // Apply Variables
            viewport = Game.current.ScreenManager.GraphicsDevice.Viewport;
            // Initialize the sprite
            playerSprite = new Sprite(content.Load<Texture2D>("Player/player"));
            playerSprite.Scale = 0.7f;
            Position = new Vector2(viewport.Width / 2, 
                viewport.Height - 120);

            //Game.current.BlurredTextures.Add( playerSprite );
            deadShield = new Animation();
            shieldTexture = content.Load<Texture2D>( "Player/shield" );

            // Create default weapon
            playerWeapon = new LaserWeapon(this);
            Game.current.runWeaponCheck();
            projectiles.Clear();

            playerBooster = new Animation();
            playerBooster.Initialize( content.Load<Texture2D>( "Animations/booster" ), 
                new Vector2( Position.X, Position.Y + playerSprite.Height - 60 ), 128, 128, 16, 32, Color.White, .45f, true );

            // Player is alive
            isAlive = true;
            //playerLives = 3;
        }

        /// <summary>
        /// Called when the player dies
        /// </summary>
        public void OnDead()
        {
            if (PlayerLives >= 0) {
                Game.current.AddPlayerExplosion(Position);

                if ( PlayerLives == 0 )
                    deadText = "Last Life!";

                // Set active to false
                IsAlive = false;
            } else {
                Game.current.AddPlayerExplosion( Position );
                deadText = "Game Over!";
                IsAlive = false;
            }

            // Sound
            Effect playerEffect = new Effect( content.Load<SoundEffect>( "Sound/playerdeath" ) );
            playerEffect.Play();
        }

        /// <summary>
        /// Handle user input
        /// </summary>
        public void HandleInput(AccelerometerState accelState, TouchCollection touchState, InputState input)
        {
            // Move player by accelerometer, > 0.10f so the slighest movement doesn't mess you up
            if (Math.Abs(accelState.Acceleration.X) > 0.10f)
                playerMovement = MathHelper.Clamp(accelState.Acceleration.X * accelerometerScale, -1.0f, 1.0f);

            // Move player by Touch screen
            foreach (TouchLocation touch in touchState)
                if ( touch.State == TouchLocationState.Moved || touch.State == TouchLocationState.Pressed ) {
                    if ( touch.Position.Y > 80.0f )
                        Position = new Vector2( touch.Position.X, touch.Position.Y );
                }
        }

        /// <summary>
        /// Game logic
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Update elapsed time (Used in HandleInput)
            elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 oldVelocity = velocity;

            // Update player velocity
            velocity.X += playerMovement * playerAcceleration * elapsedTime;
            velocity.X *= 0.48f;
            velocity.X = MathHelper.Clamp(velocity.X, -playerMaxSpeed, playerMaxSpeed);

            // Update player position
            Position += Velocity * elapsedTime;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // Viewport boundary check
            position.X = MathHelper.Clamp(
                Position.X, (playerSprite.Width / 2) * playerSprite.Scale,
                    viewport.Width - ((playerSprite.Width / 2) * playerSprite.Scale));
            position.Y = MathHelper.Clamp(
                Position.Y, 
                 80f + ( ( playerSprite.Height / 2 ) * playerSprite.Scale ),
                    viewport.Height - 100);

            // Apply values to the sprite
            playerSprite.Velocity = Velocity;
            playerSprite.Position = Position;

            playerBooster.Position = new Vector2( Position.X, Position.Y + playerSprite.Height - 60 );
            playerBooster.Update( gameTime );

            // Fire and Update the Weapon!
            if ( isAlive )
                playerWeapon.Fire( new Vector2( 0, 1f ) ); 

            playerWeapon.Update( elapsedTime );

            // Update the Projectile
            foreach ( Projectile projectile in projectiles )
            {
                if ( projectile.Active && isAlive )
                    projectile.Update( elapsedTime );
                else
                    projectiles.QueuePendingRemoval( projectile );
            }
            projectiles.ApplyPendingRemovals();

            if ( deadShieldActive ) {
                deadShieldTime += elapsedTime;
                if ( deadShieldTime > 1.5f ) {
                    deadShieldActive = false;
                    deadShield.Active = false;
                    deadShieldTime = 0.0f;
                }
            }

            if ( !isAlive ) {
                timeDead += elapsedTime;
                if ( timeDead > 1.5f ) {
                    timeDead = 0.0f;

                    if ( playerLives < 0 ) {
                        Game.current.isPaused = true;
                        Game.current.playerHS.CheckuID();
                        // Check if highscore screen should be shown instead..
                        if ( Game.current.playerHS.UIDExists && Game.current.playerScore >
                            Convert.ToDouble(Game.current.playerHS.UserData[0].Score) && com.addictionsoftware.Network.HasConnection() 
                            || !Game.current.playerHS.UIDExists && com.addictionsoftware.Network.HasConnection())
                        {
                            // :)!! Show new high score screen here
                            Game.current.ScreenManager.Add( new NewHighscore() );
                            // Play sound
                            Effect gameEffect = new Effect( content.Load<SoundEffect>( "Sound/gameover" ) );
                            gameEffect.Play();
                        } else {
                            Game.current.ScreenManager.Add( new GameOver() );
                            // Play sound
                            Effect gameEffect = new Effect( content.Load<SoundEffect>( "Sound/gameover" ) );
                            gameEffect.Play();
                        }
                    } else {
                        IsAlive = true;
                        deadShield.Initialize(
                            shieldTexture, playerSprite.Position, 128, 128, 16, 32, Color.White, 1.0f, true );
                        deadShieldActive = true;
                    }
                }
            }

            deadShield.Position = playerSprite.Position;
            deadShield.Update( gameTime );

            // Clear input
            playerMovement = 0.0f;
        }

        /// <summary>
        /// This is called when the game should draw itself
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw Projectile
            foreach (Projectile projectile in projectiles)
                projectile.Draw(gameTime, spriteBatch);

            float time = deadShieldTime / 1.5f;
            float alpha = 4 * time * ( 1 - time );

            Vector4 color = Color.White.ToVector4();
            deadShield.color = new Color( color * alpha );
            deadShield.Draw( spriteBatch );

            playerBooster.Draw( spriteBatch );
            playerSprite.Draw( gameTime, spriteBatch, SpriteEffects.None );
        }
    }
}
