//-----------------------------------------------------------------------------
// PlasmaWeapon.cs
//
// AddictionSoftware
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Halcyon
{
    class PlasmaWeapon : Weapon
    {
        /// <summary>
        /// Constructs a new plasma weapon
        /// </summary>
        public PlasmaWeapon(Player weaponOwner)
            : base( weaponOwner )
        {
            // Set the firing delay
            fireDelay = 0.3f;
        }

        /// <summary>
        /// Create the projectile
        /// </summary>
        protected override void CreateProjectiles(Vector2 velocity)
        {
            float rotation = (float)Math.Acos(Vector2.Dot(new Vector2(0f, -1f), velocity));
            rotation *= (Vector2.Dot(new Vector2(0f, -1f),
                new Vector2(velocity.Y, -velocity.X)) > 0f) ? 1f : -1f;

            Vector2 direction2 = new Vector2(
                (float)Math.Sin(rotation - MathHelper.ToRadians(3.5f)),
                -(float)Math.Cos(rotation - MathHelper.ToRadians(3.5f)));

            Vector2 direction3 = new Vector2(
                (float)Math.Sin(rotation + MathHelper.ToRadians(3.5f)),
                -(float)Math.Cos(rotation + MathHelper.ToRadians(3.5f)));

            PlasmaProjectile projectile = new PlasmaProjectile(weaponOwner, velocity);
            weaponOwner.Projectiles.Add(projectile);

            projectile = new PlasmaProjectile(weaponOwner, direction2);
            weaponOwner.Projectiles.Add(projectile);

            projectile = new PlasmaProjectile(weaponOwner, direction3);
            weaponOwner.Projectiles.Add(projectile);
        }
    }
}
