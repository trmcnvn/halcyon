//-----------------------------------------------------------------------------
// TripleLaserWeapon.cs
//
// AddictionSoftware
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    class TripleLaserWeapon : LaserWeapon
    {
        /// <summary>
        /// The spread of the second and third laser projectiles' directions, in radians
        /// </summary>
        readonly float laserSpreadRadians = MathHelper.ToRadians(2.5f);

        /// <summary>
        /// Constructs a new laser weapon
        /// </summary>
        public TripleLaserWeapon(Player weaponOwner)
            : base( weaponOwner )
        {
            // Set firing delay
            fireDelay = 0.17f;
        }

        /// <summary>
        /// Create the projectile
        /// </summary>
        protected override void CreateProjectiles(Vector2 velocity)
        {
            // Spread of the 3 lasers
            float rotation = (float)Math.Acos(Vector2.Dot(new Vector2(0f, - 1f), velocity));
            rotation *= (Vector2.Dot(new Vector2(0f, -1f), 
                new Vector2(velocity.Y, -velocity.X)) > 0f) ? 1f : -1f;

            Vector2 direction2 = new Vector2(
                (float)Math.Sin(rotation - laserSpreadRadians),
                -(float)Math.Cos(rotation - laserSpreadRadians));

            Vector2 direction3 = new Vector2(
                (float)Math.Sin(rotation + laserSpreadRadians),
                -(float)Math.Cos(rotation + laserSpreadRadians));

            // Create the first laser
            LaserProjectile projectile = new LaserProjectile(weaponOwner, "Player/laser-red",
                new Vector4(1f, 0.47f, 0.41f, .5f), velocity);
            weaponOwner.Projectiles.Add(projectile);

            // Create the second laser
            projectile = new LaserProjectile(weaponOwner, "Player/laser-red",
                new Vector4(1f, 0.47f, 0.41f, .5f), direction2);
            weaponOwner.Projectiles.Add(projectile);

            // Create the third laser
            projectile = new LaserProjectile(weaponOwner, "Player/laser-red",
                new Vector4(1f, 0.47f, 0.41f, .5f), direction3);
            weaponOwner.Projectiles.Add(projectile);
        }
    }
}
