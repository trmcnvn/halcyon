//-----------------------------------------------------------------------------
// PlasmaProjectile.cs
//
// AddictionSoftware
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Halcyon
{
    class PlasmaProjectile : Projectile
    {
        /// <summary>
        /// Speed of the projectile
        /// </summary>
        const float initialSpeed = 15f;

        /// <summary>
        /// Construct a new plasma projectile
        /// </summary>
        public PlasmaProjectile(Player projectileOwner, Vector2 velocity)
            : base(projectileOwner, new Sprite(Game.current.content.Load<Texture2D>("Player/plasma")), velocity)
        {
            this.projectileSprite.Velocity = initialSpeed * velocity;
            this.damageAmount = 50.0f;
        }

        /// <summary>
        /// When the projectile hits an object
        /// </summary>
        public override void OnHit()
        {
            Game.current.AddImpact(ProjectileSprite.Position, new Color(255, 156, 255, 128), 2);
        }

        /// <summary>
        /// Update the projectile
        /// </summary>
        public override void Update(float elapsedTime)
        {
            //this.projectileSprite.Rotation += 10;
            base.Update(elapsedTime);
        }

        /// <summary>
        /// Draws the projectile
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.projectileSprite.Draw(gameTime, spriteBatch, SpriteEffects.None);
        }
    }
}
