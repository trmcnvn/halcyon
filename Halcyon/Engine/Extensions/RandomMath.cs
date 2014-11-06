//-----------------------------------------------------------------------------
// RandomMath.cs
//-----------------------------------------------------------------------------

// Namespace Usage
using System;
using Microsoft.Xna.Framework;

namespace Halcyon
{
    static public class RandomMath
    {
        /// <summary>
        /// The Random object used for all of the random calls.
        /// </summary>
        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        /// <summary>
        /// Generates a random value between the min / max value
        /// </summary>
        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        /// <summary>
        /// Generates a random direction vector
        /// </summary>
        public static Vector2 RandomDirection()
        {
            float angle = RandomBetween(0, MathHelper.TwoPi);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// Generates a random direction vector, based off the min/max value
        /// </summary>
        public static Vector2 RandomDirection(float min, float max)
        {
            float angle = RandomBetween(MathHelper.ToRadians(min), MathHelper.ToRadians(max) - MathHelper.PiOver2);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}
