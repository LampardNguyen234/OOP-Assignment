using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TD
{
    class Tower
    {
        Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 origin;
        Vector2 center;
        Rectangle boudingBox;
        
        Texture2D texture, baseTexture, bulletTexture;
        bool isAlive;
        float rotation;
        float radius;
        float smallestRange;

        List<Bullet> bulletList = new List<Bullet>();

        Animation animation;

        public float Radius
        {
          get { return radius; }
        }
        int attack;
        int currX, currY;
        int price;
        int level;

        public Enemy target;

        public Enemy Target
        {
            get { return target; }
            set { target = value; }
        }
        private float timer;
        private float interval;

        public Tower(Texture2D texture, int level, Vector2 position, Texture2D baseTexture, Texture2D bulletTexture)
        {
            this.baseTexture = baseTexture;
            this.texture = texture;
            isAlive = true;
            rotation = 0;
            radius = (float)150 * level;
            attack = 1 * level;
            currX = currY = 0;
            this.position = position;
            price = 50 * level;
            origin = new Vector2(Container.towerSize/2, Container.towerSize/2);
            center = position + new Vector2((float)(Container.tileSize / 2), (float)(Container.tileSize / 2));
            smallestRange = radius;
            this.level = level;
            this.bulletTexture = bulletTexture;
            timer = 0f;
            interval = 200f;
        }

        //Kiểm tra xem enemy có nằm trong bán kính không
        public bool IsInRange(Enemy target)
        {
            if (target == null)
                return false;
            if (Vector2.Distance(center, target.Center) <= radius)
                return true;

            return false;
        }

        //Tìm Enemy gần nhất
        public void GetClosestEnemy(List<Enemy> enemyList)
        {
            target = null;
            foreach (Enemy enemy in enemyList)
            {
                if (Vector2.Distance(center, enemy.Center) < smallestRange)
                {
                    smallestRange = Vector2.Distance(center, enemy.Center);
                    target = enemy;
                }
            }
        }

        //FaceTarget: Hướng về target
        void FaceTarget()
        {
            if (target == null)
                return;
            Vector2 direction = center - target.Center;
            direction.Normalize();
            rotation = (float)Math.Atan2(direction.Y, direction.X);
        }



        //Update
        public void Update(GameTime gameTime)
        {
            if (target != null)
                if (target.isAlive == false)
                    target = null;
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                if (target != null)
                {
                    Bullet bullet = new Bullet(position, level, bulletTexture);
                    bulletList.Add(bullet);
                }
                timer = 0;
            }
            for (int i = 0; i < bulletList.Count; i++)
            {
                Bullet bullet = bulletList[i];

                bullet.SetRotation(rotation);
                bullet.Update(gameTime);


                if(Collision(bullet))
                {
                    target.CurrentHealth -= attack;
                    //bullet.Kill();
                }
                if (!IsInRange(target))
                    bullet.Kill();
                if (bullet.IsDead())
                {
                    bulletList.Remove(bullet);
                    i--;
                }
            }
            FaceTarget();
        }

        //Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
            {
                spriteBatch.Draw(baseTexture, center, null, Color.White, 0f, origin, 1.0f, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, center, null, Color.White, rotation, origin, 1.0f, SpriteEffects.None, 0f);
            }
            foreach (Bullet bullet in bulletList)
            {
                if(CheckBulletOutOfTower(bullet))
                    bullet.Draw(spriteBatch);
            }
            if(target!=null)
            {
                if(target.isAlive==false)
                {
                    animation = new Animation(target.Center);
                    animation.Draw(spriteBatch);
                }
            }

        }

        bool CheckBulletOutOfTower(Bullet bullet)
        {
            if ((int)bullet.Center.X / Container.towerSize == (int)center.X / Container.towerSize
                 && (int)bullet.Center.Y / Container.towerSize == (int)center.Y / Container.towerSize)
                return false;
            return true;
        }

        bool Collision(Bullet bullet)
        {
            if (target == null)
                return false;
            Vector2 distance = bullet.Center - target.Center;
            if (Math.Abs(distance.X) <= target.Texture.Width/2 && Math.Abs(distance.Y) <= target.Texture.Height/2)
                return true;
            return false;
        }
    }
}
