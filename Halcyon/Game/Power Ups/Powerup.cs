//-----------------------------------------------------------------------------
// Powerup.cs
//
// AddictionSoftware
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    abstract class Powerup
    {
        // Texture of the power up
        protected Texture2D powerTexture;

        // Sound to play when a power up is collected
        protected string powerSound;

        // List of the animations
        protected List<Animation> powerList;

        // Constructor
        protected Powerup(Texture2D powerTexture)
        {
            this.powerTexture = powerTexture;
            this.powerList = new List<Animation>();
        }

        // Abstract for creating the power up
        public abstract void CreatePower(Vector2 position);

        // Update animation list
        public virtual void Update(GameTime gameTime)
        {
            for (int i = powerList.Count - 1; i >= 0; i--)
            {
                powerList[i].Update(gameTime);
                if (powerList[i].Active == false)
                    powerList.RemoveAt(i);
            }
        }
        
        // Draw the animation
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < powerList.Count; i++)
            {
                powerList[i].Draw(spriteBatch);
            }
        }
    }
}
