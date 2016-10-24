using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TD
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        float timer, oldTime;
        int mapLevel;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Map map = new Map();

        Level level;
        List<Enemy> enemyList = new List<Enemy>();
        Player player;
        Animation animation;
        Texture2D animationTexture;

        Texture2D enemyTexture;
        Texture2D healthBarTexture;

        int enemyPerWave;

        //Animation animation = new Animation(new Vector2(80, 80));
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Container.scrWidth;
            graphics.PreferredBackBufferHeight = Container.scrHeight;
            graphics.ApplyChanges();
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Random rand = new Random();
            //mapLevel = rand.Next(1, 4);
            mapLevel = 1;
            level = new Level(map, mapLevel);
            level.LoadContent(Content);
            timer = Container.timeBetweenEnemy+1;
            oldTime = 0f;
            enemyPerWave = Container.enemyPerWave;

            enemyTexture = Content.Load<Texture2D>("enemy_0070");
            healthBarTexture = Content.Load<Texture2D>("healthbar");
            animationTexture = Content.Load<Texture2D>("animation_001");

            Texture2D towerTexture = Content.Load<Texture2D>("tower__000");
            Texture2D baseTexture = Content.Load<Texture2D>("base");
            Texture2D bulletTexture = Content.Load<Texture2D>("bullet_00");
            player = new Player(level, towerTexture,baseTexture, bulletTexture);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            timer += 2;
            if (timer > Container.timeBetweenEnemy)
            {
                if (enemyPerWave > 0)
                {
                    timer = 0f;
                    enemyList.Add(new Enemy(1, map, mapLevel, enemyTexture, healthBarTexture));
                    enemyPerWave--;
                }
            }
            foreach (Enemy enemy in enemyList)
            {
                if (enemy.isWayPointsSet == false)
                {
                    Random rand = new Random();
                    int k = rand.Next(0, 20);
                    level.LoadWayPoint(k);
                    enemy.SetWaypoints(level.WayPoints);
                    enemy.isWayPointsSet = true;
                }
            }
        
            player.Update(gameTime,enemyList);
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].isAlive)
                    enemyList[i].Update(gameTime);
                else
                {
                    enemyList.RemoveAt(i);
                    i--;
                }
            }

            //animation.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            level.Draw(spriteBatch);
            //animation.Draw(spriteBatch);
            foreach (Enemy enemy in enemyList)
            {
                
                enemy.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
