using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TD
{
    class Enemy
    {
        #region Khai báo
        int currentHealth,startHealth;

        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }
        int bountyGiven;

        public int BountyGiven
        {
            get { return bountyGiven; }
        }
        int attack;

        public int Attack
        {
            get { return attack; }
        }
        int level;
        float speed;
        float rotation;
        float timer, interval;
        Vector2 position;
        Vector2 orgin;
        Vector2 center;

        public Vector2 Center
        {
            get { return center; }
        }
        int currX, currY;
        Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        Texture2D healthBarTexture;
        Rectangle boundingBox;
        Rectangle healthBox;
        public bool isAlive, isWayPointsSet;
        private Queue<Vector2> waypoints;
        private Vector2 velocity { get; set; }

        int[,] map;
       
        #endregion
        
        public Enemy(int level, Map map, int maplevel, Texture2D texture, Texture2D healthBarTexture)
        {
            waypoints = new Queue<Vector2>();
            startHealth = currentHealth = 40 * level;
            bountyGiven = 25*level;
            attack = 5*level;
            this.level = level;
            speed = 1f+ (float)(level/10);
            position = new Vector2(0, 0);
            timer = 0f;
            interval = 100f;
            orgin = new Vector2(0, 0);
            center = new Vector2(0, 0);
            isAlive = true;
            boundingBox = new Rectangle(0, 0, 0, 0);
            currX = currY = 0;
            this.map = map.MapList[maplevel-1];
            Texture = texture;
            this.healthBarTexture = healthBarTexture;
            isWayPointsSet = false;
        }

        #region Các methods
        //Load Content

        public void LoadContent(ContentManager Content)
        {
            string s = "enemy_002";
            texture = Content.Load<Texture2D>(s);
            healthBarTexture = Content.Load<Texture2D>("healthbar");
        }

        //Update
        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                if (currX == texture.Width / Container.enemyTextureSize - 1)
                {
                    currX = 0;
                    currY++;
                }
                else
                    currX++;
                if (currY == texture.Height / Container.enemyTextureSize)
                    currX = currY = 0;
                timer = 0;
            }
            if (waypoints.Count > 0)
            {
                if (DistanceToDestination < speed)
                {
                    position = waypoints.Peek();
                    waypoints.Dequeue();
                }
                else
                {
                    Vector2 direction = waypoints.Peek() - position;
                    direction.Normalize();

                    velocity = Vector2.Multiply(direction, speed);

                    position += velocity;
                    center = new Vector2(position.X + Container.tileSize / 2, position.Y + Container.tileSize / 2);
                    orgin = new Vector2(Container.enemyTextureSize/2, Container.enemyTextureSize/2);
                }
            }
            else
                isAlive = false;
            boundingBox = new Rectangle(currX * Container.enemyTextureSize, currY * Container.enemyTextureSize, Container.enemyTextureSize, Container.enemyTextureSize);
            int k = rotationStatus(position);
            float rotationAdd = 0;
            float a = (float)(speed/1.3);
            {
                if (k == 0)
                {
                    rotationAdd = 0f;
                }
                if (k == 1)
                {
                    rotationAdd = ((float)a / 90) * Container.PI;
                }
                if (k == 2)
                {
                    rotationAdd = ((float)a / 90) * Container.PI;
                }
                if (k == 3)
                {
                    rotationAdd = -((float)a / 90) * Container.PI;
                }
                if (k == 4)
                {
                    rotationAdd = -((float)a / 30) * Container.PI;
                }
                if (k == 10)
                    rotationAdd = 0;
            }
            rotation += rotationAdd;
            if (currentHealth <= 0)
                isAlive = false;
            float healthPercent = (float)currentHealth  / (float)startHealth;
            healthBox = new Rectangle(((int)center.X + (int)position.X)/2, (int)center.Y, (int)(Container.healthBarWidth *healthPercent), Container.healthBarHeight);
        }

        //Set Waypoints (đường mà enemies sẽ di theo)
        public void SetWaypoints(Queue<Vector2> waypoints)
        {
            foreach (Vector2 waypoint in waypoints)
                this.waypoints.Enqueue(waypoint);

            if (waypoints.Peek().X == 0)
            {
                rotation = -Container.PI / 2;
                this.position = this.waypoints.Dequeue();
            }
            else
            {
                rotation = 0;
                this.position = this.waypoints.Dequeue();
            }
        }



        //Tính khoảng cách từ enemies tới waypoint tiếp theo
        public float DistanceToDestination
        {
            get { return Vector2.Distance(position, waypoints.Peek()); }
        }

        //Tính góc quay cho texture khi di chuyen
        int rotationStatus(Vector2 position)
        {
            int X = (int)(position.X / Container.tileSize);
            int Y = (int)(position.Y / Container.tileSize);
            if (X < 0 || Y < 0)
                return 10;
            if (map[Y, X] == 0)
                return -1;
            int Height = map.GetLength(0);
            int Width = map.GetLength(1);
            if(X>=0 && X+1<Width && Y>=0 &&Y+1<Height)
            {
                if (isContained(waypoints,Y+1,X)&& isContained(waypoints,Y+1,X+1))
                    return 3;//     |__
                if (isContained(waypoints, Y, X+1) && isContained(waypoints, Y + 1, X + 1))
                    return 1; //    __
                              //      |  
                if (X == 0)
                    return 0;
                if (isContained(waypoints, Y + 1, X) && isContained(waypoints, Y + 1, X - 1))
                    return 2;//     __|
                if (isContained(waypoints, Y, X-1) && isContained(waypoints, Y + 1, X - 1))
                    return 4;//     __
                               //  |
            }
           
            return 0;
        }

        //Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
            {
                spriteBatch.Draw(texture, center, boundingBox, Color.White, rotation, orgin, 1.0f, SpriteEffects.None, 0f);
                //Vẽ thanh hiển thị máu
                spriteBatch.Draw(healthBarTexture, healthBox, Color.White);
            }
        }

        //Kiểm tra xem map[Y,X] có nằm trong waypoints ko
        bool isContained(Queue<Vector2> waypoints, int Y,int X)
        {
            foreach (Vector2 waypoint in waypoints)
            {
                if ((int)(waypoint.X/Container.tileSize) == X && (int)(waypoint.Y/Container.tileSize) == Y)
                    return true;
            }
            return false;
        }
        #endregion

    }
}
