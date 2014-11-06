//-----------------------------------------------------------------------------
// Projectile.cs
//
// AddictionSoftware
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    abstract class Projectile
    {
        /// <summary>
        /// The Sprite who fired this projectile
        /// </summary>
        private Player projectileOwner;
        public Player ProjectileOwner
        {
            get { return projectileOwner; }
        }

        /// <summary>
        /// If true, the projectile is active in the world.
        /// </summary>
        protected bool active = true;
        public bool Active
        {
            get { return active; }
        }

        /// <summary>
        /// Sprite for this projectile
        /// </summary>
        protected Sprite projectileSprite;
        public Sprite ProjectileSprite
        {
            get { return projectileSprite; }
            set { projectileSprite = value; }
        }

        /// <summary>
        /// Amount of damage this projectile does
        /// </summary>
        protected float damageAmount = 0.0f;
        public float Damage
        {
            get { return damageAmount; }
        }

        /// <summary>
        /// Constructs a new projectile
        /// </summary>>
        protected Projectile(Player projectileOwner, Sprite projectileSprite, Vector2 velocity)
        {
            // Apply to params
            this.projectileOwner = projectileOwner;

            // Initialize Sprite Info
            this.projectileSprite = projectileSprite;
            this.projectileSprite.Scale = 0.5f;
            this.projectileSprite.Velocity = velocity;
            this.projectileSprite.Position = new Vector2(ProjectileOwner.playerSprite.Position.X, ProjectileOwner.playerSprite.Position.Y);
        }


        /// <summary>
        /// When the projectile hits an object
        /// </summary>
        public abstract void OnHit();


        /// <summary>
        /// Update the logic
        /// </summary>
        public virtual void Update(float elapsedTime)
        {
            if (projectileSprite.Position.Y < 0 - projectileSprite.Width * projectileSprite.Scale)
                active = false;

            if (projectileSprite.Rotation >= 360)
                projectileSprite.Rotation = 0;

            projectileSprite.Position -= projectileSprite.Velocity;
        }

        /// <summary>
        /// Draw the projectile
        /// </summary>
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
