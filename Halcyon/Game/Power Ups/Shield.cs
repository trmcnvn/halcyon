//-----------------------------------------------------------------------------
// Shield.cs
//
// AddictionSoftware
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Halcyon
{
    class Shield : Powerup
    {
        // Animation
        public Animation animShield;

        // Velocity
        private Vector2 shieldVelocity;

        // Shield
        private Animation playerShield;
        
        // Shield up time
        private const float playerShieldTime = 5f;

        // Shield fade out delay
        private float playerShieldFade = 0f;

        // Time since power up was activated
        private float playerShieldLast = 0f;

        // is shield active
        public bool shieldActive = false;

        // Constructor
        public Shield(Texture2D shieldTexture)
            : base(shieldTexture)
        {
            animShield = new Animation();
            playerShield = new Animation();
            
            powerSound = "audio/lol";

            shieldVelocity = new Vector2(0f, 5f);
        }

        public override void Update(GameTime gameTime)
        {
            animShield.Position += shieldVelocity;

            if (animShield.Position.Y > (Game.current.ScreenManager.GraphicsDevice.Viewport.Height +
                animShield.FrameHeight))
                animShield.Active = false;

            if (animShield.Active)
            {
                if (animShield.CollisionCheck(Game.current.player.playerSprite))
                {
                    // Player has picked up the power-up
                    // Activate shield
                    playerShield.Initialize(Game.current.content.Load<Texture2D>("Player/shield"),
                        Game.current.player.playerSprite.Position,
                        128, 128, 16, 32, Color.White, 1f, true);

                    shieldActive = true;
                    animShield.Active = false;

                    Effect shieldEffect = new Effect( Game.current.content.Load<SoundEffect>( "Sound/shield" ) );
                    shieldEffect.Play();
                }
            }

            if (playerShield.Active)
            {
                playerShieldFade += (float)gameTime.ElapsedGameTime.TotalSeconds;
                playerShieldLast += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (playerShieldLast > playerShieldTime)
                {
                    playerShield.Active = false;
                    shieldActive = false;
                    playerShieldLast = 0f;
                    playerShieldFade = 0f;
                }
            }

            playerShield.Position = Game.current.player.playerSprite.Position;
            playerShield.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float time = playerShieldFade / 5f;
            float alpha = 4 * time * (1 - time);

            Vector4 color = Color.White.ToVector4();
            playerShield.color = new Color(color * alpha);

            if ( Game.current.player.IsAlive )
                playerShield.Draw( spriteBatch );

            base.Draw(spriteBatch);
        }

        // Create and initialize the power up
        public override void CreatePower(Vector2 position)
        {
            animShield.Initialize(powerTexture, position, 128, 128, 16, 32, Color.White, 1f, true);
            powerList.Add(animShield);
        }
    }
}