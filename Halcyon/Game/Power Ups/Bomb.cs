//-----------------------------------------------------------------------------
// Bomb.cs
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
    class Bomb : Powerup
    {
        // Animation
        public Animation animBomb;

        // Velocity
        private Vector2 bombVelocity;

        // Collection Animation
        private Animation playerBomb;

        // Constructor
        public Bomb(Texture2D bombTexture)
            : base(bombTexture)
        {
            animBomb = new Animation();
            playerBomb = new Animation();

            powerSound = "audio/lol";

            bombVelocity = new Vector2(0f, 5f);
        }

        public override void Update(GameTime gameTime)
        {
            animBomb.Position += bombVelocity;

            if (animBomb.Position.Y > (Game.current.ScreenManager.GraphicsDevice.Viewport.Height +
                animBomb.FrameHeight))
                animBomb.Active = false;

            if (animBomb.Active)
            {
                if (animBomb.CollisionCheck(Game.current.player.playerSprite))
                {
                    // Player has picked up the power-up
                    // Activate shield
                    playerBomb.Initialize(Game.current.content.Load<Texture2D>("Player/bomb"),
                        Game.current.player.playerSprite.Position,
                        128, 128, 16, 45, Color.White, 1.5f, false);

                    animBomb.Active = false;
                    Game.current.bombCount++;

                    // Sound
                    Effect bombEffect = new Effect( Game.current.content.Load<SoundEffect>( "Sound/bomb" ) );
                    bombEffect.Play();
                }
            }

            playerBomb.Position = Game.current.player.playerSprite.Position;
            playerBomb.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            playerBomb.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }

        // Create and initialize the power up
        public override void CreatePower(Vector2 position)
        {
            animBomb.Initialize(powerTexture, position, 128, 128, 16, 32, Color.White, 1f, true);
            powerList.Add(animBomb);
        }
    }
}