//-----------------------------------------------------------------------------
// LaserWeapon.cs
//
// AddictionSoftware
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Halcyon
{
    class LaserWeapon : Weapon
    {
        /// <summary>
        /// Constructs a new laser weapon
        /// </summary>
        public LaserWeapon(Player weaponOwner)
            : base( weaponOwner )
        {
            // Set firing delay
            fireDelay = 0.12f;
        }

        /// <summary>
        /// Create the projectile
        /// </summary>
        protected override void CreateProjectiles(Vector2 velocity)
        {
            LaserProjectile projectile = new LaserProjectile(weaponOwner, "Player/laser", 
                new Vector4(0.6f, 1f, 1f, .5f), velocity);
            weaponOwner.Projectiles.Add(projectile);
        }
    }
}
