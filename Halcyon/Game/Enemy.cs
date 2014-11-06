//-----------------------------------------------------------------------------
// Enemy.cs
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    class Enemy
    {
        // Viewport
        private Viewport viewport;

        // Content manager
        private ContentManager content;

        // Struct for enemy information
        public struct EnemyData
        {
            // Sprite for the enemy
            public Sprite enemySprite;
            // Texture
            public string enemyTexture;
            // Enemy health
            public float Health;
            // Enemy Experience
            public float Experience;
        }
        private List<EnemyData> enemyData = new List<EnemyData>();

        // Sub-class for creating enemies
        public class Enemies
        {
            public EnemyData enemyData;

            public Enemies(EnemyData enemyData)
            {
                this.enemyData = enemyData;
            }

            /// <summary>
            /// Called when the enemy dies
            /// </summary>
            public void OnDead()
            {
                Game.current.AddExplosion(enemyData.enemySprite.Position);
            }
        }

        // Time between spawns
        private TimeSpan elapsedTime = TimeSpan.Zero;
        public int spawnTime = 700;
        // Max amount of enemies on the screen
        public int maxActive = 8;
        // Current amount of enemies
        public int curActive;
        public float enemySpeed = 6.0f;


        TimeSpan fct_time = TimeSpan.Zero;
        // fct font
        public SpriteFont fct_font;

        // fct list
        public class fct_data
        {
            public Vector2 fct_position;
            public bool fct_flag;
            public string fct_text;
            public float fct_time;
        }

        public List<fct_data> fct_list = new List<fct_data>();

        /// <summary>
        /// All of the enemies
        /// </summary>
        private ListRemovalCollection<Enemies> enemies = 
            new ListRemovalCollection<Enemies>();
        public ListRemovalCollection<Enemies> EnemiesList
        {
            get { return enemies; }
        }

        /// <summary>
        /// LoadContent will be called once per game
        /// Load all of your content here
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            // Apply Variables
            this.content = content;
            viewport = Game.current.ScreenManager.GraphicsDevice.Viewport;

            // Load all enemy assets here so we don't get any lag at all during gameplay
            for ( int i = 1; i < 10; i++ )
            {
                content.Load<Texture2D>( "Enemy/" + ( i ).ToString() );
            }

            // Fill List of sprites
            enemyData = ( from Data in XElement.Load( "Content/Enemy/EnemyData.xml" ).Descendants( "EnemyInfo" )
                            select new EnemyData
                            {
                                Health = ( float )Data.Attribute( "Health" ),
                                Experience = ( float )Data.Attribute( "Exp" ),
                                enemyTexture = ( string )Data.Attribute( "Texture" )
                            } ).ToList();

            enemies.Clear();

            // font
            fct_font = content.Load<SpriteFont>( "Font/hudfont" );
        }

        /// <summary>
        /// Game logic
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // fct
            // wait 1s and then delete
            for ( int i = fct_list.Count - 1; i >= 0; i-- )
            {
                if ( fct_list[i].fct_flag )
                {
                    fct_list[ i ].fct_time += ( float )gameTime.ElapsedGameTime.TotalSeconds;

                    fct_list[ i ].fct_position.Y -= 1.0f;

                    if ( fct_list[ i ].fct_time > 1f )
                    {
                        fct_list.RemoveAt( i );
                    }
                }
            }

            // Make sure it's past the min spawning time
            if ((elapsedTime += gameTime.ElapsedGameTime).Milliseconds > spawnTime)
            {
                // Reset elapsedTime
                elapsedTime = TimeSpan.Zero;

                // Make sure we don't have more enemies active than the max
                if (curActive < maxActive)
                {
                    // Create X amount of enemies
                    for (int i = 0; i < RandomMath.Random.Next(2, 4); i++)
                    {
                        // Create new Enemy
                        Enemies enemy = new Enemies(enemyData[RandomMath.Random.Next(enemyData.Count)]);
                        enemy.enemyData.enemySprite = new Sprite(content.Load<Texture2D>("Enemy/" + enemy.enemyData.enemyTexture));

                        enemy.enemyData.enemySprite.Position = new Vector2(
                            RandomMath.RandomBetween( enemy.enemyData.enemySprite.Width / 2,
                                viewport.Width - enemy.enemyData.enemySprite.Width ),
                            80f - enemy.enemyData.enemySprite.Height);
                        enemy.enemyData.enemySprite.Scale = 0.9f;

                        // Random chance that this enemy will also move on the X axis
                        if (RandomMath.Random.Next(100) > 80)
                            enemy.enemyData.enemySprite.Velocity = new Vector2(10.0f, enemySpeed);
                        else
                            enemy.enemyData.enemySprite.Velocity = new Vector2(0.0f, enemySpeed);

                        // Add this new enemy to the active enemy list
                        enemies.Add(enemy);
                        // Increase active enemy count
                        curActive++;
                    }
                }
            }

            foreach (Enemies enemy in enemies)
            {
                // X axis boundary check
                if (enemy.enemyData.enemySprite.Position.X < ((enemy.enemyData.enemySprite.Width / 2) * enemy.enemyData.enemySprite.Scale) 
                        || enemy.enemyData.enemySprite.Position.X > viewport.Width -
                            ((enemy.enemyData.enemySprite.Width / 2) * enemy.enemyData.enemySprite.Scale))
                    enemy.enemyData.enemySprite.Velocity = 
                        new Vector2(-enemy.enemyData.enemySprite.Velocity.X, enemy.enemyData.enemySprite.Velocity.Y);

                if (enemy.enemyData.enemySprite.Position.Y > (viewport.Height + enemy.enemyData.enemySprite.Height))
                {
                    enemies.QueuePendingRemoval(enemy);
                    curActive--;
                }

                // Update Position!
                enemy.enemyData.enemySprite.PrevPosition = enemy.enemyData.enemySprite.Position;
                enemy.enemyData.enemySprite.Position += enemy.enemyData.enemySprite.Velocity;
            }

            enemies.ApplyPendingRemovals();
        }

        /// <summary>
        /// This is called when the game should draw itself
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach ( Enemies enemy in enemies )
            {
                enemy.enemyData.enemySprite.Draw( gameTime, spriteBatch, SpriteEffects.None );
            }
        }
    }
}
