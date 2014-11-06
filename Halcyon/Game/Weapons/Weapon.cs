//-----------------------------------------------------------------------------
// Weapon.cs
//
// AddictionSoftware
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Halcyon
{
    abstract class Weapon
    {
        /// <summary>
        /// Owner of this weapon
        /// </summary>
        protected Player weaponOwner;

        /// <summary>
        /// The time remaining before the next shot
        /// </summary>
        protected float timeToNextFire = 0.0f;

        /// <summary>
        /// The time between each shot
        /// </summary>
        protected float fireDelay = 0.0f;

        /// <summary>
        /// Constructs a new weapon.
        /// </summary>
        protected Weapon(Player weaponOwner)
        {
            this.weaponOwner = weaponOwner;
        }

        /// <summary>
        /// Update the weapon
        /// </summary>
        public virtual void Update(float elapsedTime)
        {
            // Count down to when the weapon can fire again
            if (timeToNextFire > 0.0f)
                timeToNextFire = MathHelper.Max(timeToNextFire - elapsedTime, 0.0f);
        }

        /// <summary>
        /// Fire the weapon in the direction given
        /// </summary>
        public virtual void Fire(Vector2 velocity)
        {
            // Get out of this if we are still on cool down
            if (timeToNextFire > 0.0f)
                return;

            // Set the timer
            timeToNextFire = fireDelay;

            // Create the projectile
            CreateProjectiles(velocity);
        }

        /// <summary>
        /// Create the projectile
        /// </summary>
        protected abstract void CreateProjectiles(Vector2 velocity);
    }
}
