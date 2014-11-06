//-----------------------------------------------------------------------------
// Life.cs
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
    class ExtraLife : Powerup
    {
        // Animation
        public Animation animLife;

        // Velocity
        private Vector2 lifeVelocity;

        // Collection Animation
        private Animation playerExtraLife;

        // Constructor
        public ExtraLife(Texture2D lifeTexture)
            : base(lifeTexture)
        {
            animLife = new Animation();
            playerExtraLife = new Animation();

            powerSound = "audio/lol";

            lifeVelocity = new Vector2(0f, 5f);
        }

        public override void Update(GameTime gameTime)
        {
            animLife.Position += lifeVelocity;

            if (animLife.Position.Y > (Game.current.ScreenManager.GraphicsDevice.Viewport.Height +
                animLife.FrameHeight))
                animLife.Active = false;

            if (animLife.Active)
            {
                if (animLife.CollisionCheck(Game.current.player.playerSprite))
                {
                    // Player has picked up the power-up
                    // Activate shield
                    playerExtraLife.Initialize(Game.current.content.Load<Texture2D>("Player/life"),
                        Game.current.player.playerSprite.Position,
                        128, 128, 16, 45, Color.White, 1f, false);

                    // Give player an extra life
                    Game.current.player.PlayerLives++;

                    animLife.Active = false;

                    // Sound
                    Effect lifeEffect = new Effect( Game.current.content.Load<SoundEffect>( "Sound/life" ) );
                    lifeEffect.Play();
                }
            }

            playerExtraLife.Position = Game.current.player.playerSprite.Position;
            playerExtraLife.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            playerExtraLife.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }

        // Create and initialize the power up
        public override void CreatePower(Vector2 position)
        {
            animLife.Initialize(powerTexture, position, 128, 128, 16, 32, Color.White, 0.7f, true);
            powerList.Add(animLife);
        }
    }
}