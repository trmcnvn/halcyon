//-----------------------------------------------------------------------------
// DoubleLaserWeapon.cs
//
// AddictionSoftware
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    class DoubleLaserWeapon : LaserWeapon
    {
        /// <summary>
        /// Constructs a new laser weapon
        /// </summary>
        public DoubleLaserWeapon(Player weaponOwner)
            : base(weaponOwner )
        {

        }

        /// <summary>
        /// Create the projectile
        /// </summary>
        protected override void CreateProjectiles(Vector2 velocity)
        {
            // Spread of the 2 lasers
            Vector2 cross = Vector2.Multiply(new Vector2(-velocity.Y, velocity.X), 8f);

            // Create the first laser
            LaserProjectile projectile = new LaserProjectile(weaponOwner, "Player/laser-green", 
                new Vector4(0.6f, 1f, 0.6f, .5f), velocity);
            weaponOwner.Projectiles.Add(projectile);
            projectile.ProjectileSprite.Position += cross;

            // Create the second laser
            projectile = new LaserProjectile(weaponOwner, "Player/laser-green",
                new Vector4(0.6f, 1f, 0.6f, .5f), velocity);
            weaponOwner.Projectiles.Add(projectile);
            projectile.ProjectileSprite.Position -= cross;
        }
    }
}
