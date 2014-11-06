//-----------------------------------------------------------------------------
// LaserProjectile.cs
//
// AddictionSoftware
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Halcyon
{
    class LaserProjectile : Projectile
    {
        /// <summary>
        /// Speed of the projectile
        /// </summary>
        const float initialSpeed = 15f;

        /// <summary>
        /// Color of the impact effect
        /// </summary>
        Vector4 impactColor = Vector4.Zero;

        /// <summary>
        /// Constructs a new laser projectile
        /// </summary>
        public LaserProjectile(Player projectileOwner, String texturePath, Vector4 impactColor, Vector2 velocity)
            : base(projectileOwner, new Sprite(Game.current.content.Load<Texture2D>(texturePath)),  velocity)
        {
            this.projectileSprite.Velocity = initialSpeed * velocity;
            this.impactColor = impactColor;
            this.damageAmount = 35.0f;
        }

        /// <summary>
        /// When the projectile hits an object
        /// </summary>
        public override void OnHit()
        {
            Game.current.AddImpact(ProjectileSprite.Position, new Color(impactColor), 2);
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
