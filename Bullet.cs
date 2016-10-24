using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TD
{
    class Bullet
    {
        #region Khai báo
        Vector2 position;
        Vector2 velocity;
        Vector2 origin;
        Vector2 center;

        
        public Vector2 Center
        {
            get { return center; }
            set { center = value; }
        }

        Texture2D texture;

        Animation animation;

        private int age;

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        private int level;
        private int speed;
       

        private float rotation;

        #endregion

        public Bullet(Vector2 position, int level, Texture2D bulletTexture)
        {
            texture = bulletTexture;
            this.position = position;
            this.level = level;
            animation = new Animation(position);
            this.speed = 10 * level;
            rotation = 0;
            age = 0;
            velocity = new Vector2(0, 0);
            center = new Vector2(0, 0);
        }


        public bool IsDead()
        {
            return age > 100;
        }

        public void Kill()
        {
            age = 200;
        }

        public void SetRotation(float value)
        {
            rotation = value;

            velocity = Vector2.Transform(new Vector2(-speed, 0)
                , Matrix.CreateRotationZ(rotation));//Need Fixing
        }

        public void Update(GameTime gameTime)
        {
            age += 10 ;
            position += velocity;
            center = new Vector2(position.X + Container.towerSize / 2, position.Y + Container.towerSize / 2);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(!IsDead())
            {
                spriteBatch.Draw(texture, center, null, Color.White, rotation, origin, 1.0f, SpriteEffects.None, 0f);
            }
        }
    }
}
