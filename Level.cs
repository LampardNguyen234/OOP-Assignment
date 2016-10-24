using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TD
{
    class Level
    {
        //Bien map luu vi tri cac tile, so 1 chi vi tri do la duong.
        public int[,] map;

       private Queue<Vector2> wayPoints = new Queue<Vector2>();
       public Queue<Vector2> WayPoints
       {
           get { return wayPoints; }
           set { wayPoints = value; }
       }
       int level;
       private List<Texture2D> tileTextureList = new List<Texture2D>();
       private List<Texture2D> roadTextureList = new List<Texture2D>();

        private Vector2 position;
        public int Width
        {
            get { return map.GetLength(1); }
        }
        public int Height
        {
            get { return map.GetLength(0); }
        }
        public Level(Map map, int mapLevel)
        {
            level = mapLevel;
            this.map = map.MapList[level-1];
        }

        #region     Tạo waypoints
        public void LoadWayPoint(int k)
        {
            while (wayPoints.Count > 0)
                wayPoints.Dequeue();
            Random rand = new Random();                     // tạo số ngẫu nhiên
            Stack<Vector2> way=new Stack<Vector2>();
            List<Vector2> beginningList = new List<Vector2>();
            for (int x = 0; x < Width; x++)
            {
                if (map[0, x] == 1)
                { 
                    Vector2 beginningPoint = new Vector2(x, 0) * Container.roadSize;
                    beginningList.Add(beginningPoint);
                }
            }
            for (int y = 0; y < Height;y++ )
            {
                if (map[y, 0] == 1)
                    beginningList.Add(new Vector2(0, y) * Container.roadSize);
            }
            int m = rand.Next(0, beginningList.Count);
            if (m == 1)
                m = 1;
            way.Push(beginningList[m]);
            for (int y = 1; y < Height; y++)
            {
                for (int x = 1; x < Width; x++)
                {
                    if (map[y, x] == 1 && !isContained(way, y, x))
                    {
                        if (y == 0 || x < 1 || x == Width - 1)
                        {
                            if ((int)(way.Peek().Y / Container.roadSize) != Height - 1) 
                                way.Push(new Vector2(x, y) * Container.roadSize);
                        }
                        else
                        {
                            Vector2 tempPos = way.Peek() / Container.roadSize;
                            #region Đường đi nằm ở hàng cuối cùng
                            if (y == Height - 1)
                            {
                                if ((y == (int)tempPos.Y && x - 1 == (int)tempPos.X) ||
                                     (y - 1 == (int)tempPos.Y && x == (int)tempPos.X))
                                {
                                    way.Push(new Vector2(x, y) * Container.roadSize);
                                    continue;
                                }
                            }
                            #endregion
                            #region Đi từ trái sang phải
                            if (y == (int)tempPos.Y && x - 1 == (int)tempPos.X)
                            {
                                #region Trường hợp có thể đi qua phải hay đi xuống
                                if (map[y + 1, x] == 1 && map[y, x + 1] == 1)
                                {
                                    int i = rand.Next(0, 20);
                                    if (k + i < 20)
                                    {
                                        way.Push(new Vector2(x, y) * Container.roadSize);
                                        way.Push(new Vector2(x + 1, y) * Container.roadSize);
                                        continue;
                                    }
                                    else
                                    {
                                        way.Push(new Vector2(x, y) * Container.roadSize);
                                        way.Push(new Vector2(x, y + 1) * Container.roadSize);
                                        continue;
                                    }
                                }
                                #endregion

                                way.Push(new Vector2(x, y) * Container.roadSize);
                                continue;
                            }
                            #endregion
                            #region Đi từ trên xuống
                            if (y - 1 == (int)tempPos.Y && x == (int)tempPos.X)
                            {
                                #region Có thể đi thẳng xuống hoặc đi qua phải
                                if (map[y + 1, x] == 1 && 1 == map[y, x + 1])
                                {
                                    int i = rand.Next(0, 20);
                                    if (i + k < 20)
                                    {
                                        way.Push(new Vector2(x, y) * Container.roadSize);
                                        way.Push(new Vector2(x + 1, y) * Container.roadSize);
                                        continue;
                                    }
                                    else
                                    {
                                        way.Push(new Vector2(x, y) * Container.roadSize);
                                        way.Push(new Vector2(x, y + 1) * Container.roadSize);
                                        continue;
                                    }
                                }
                                #endregion
                                #region Có thể đi thẳng xuống
                                if (y < Height - 2)
                                {
                                    if (map[y + 1, x - 1] == 1 && map[y + 1, x] == 1 && map[y + 2, x] == 1)
                                    {
                                        way.Push(new Vector2(x, y) * Container.roadSize);
                                        way.Push(new Vector2(x, y + 1) * Container.roadSize);
                                        way.Push(new Vector2(x, y + 2) * Container.roadSize);
                                        continue;
                                    }
                                }
                                #endregion
                            #endregion
                                way.Push(new Vector2(x, y) * Container.roadSize);
                                continue;
                            }
                        }
                    }
                }
            }
            Stack<Vector2> temp = new Stack<Vector2>();
            while(way.Count>0)
            {
                temp.Push(way.Pop());
            }
            while (temp.Count > 0)
                WayPoints.Enqueue(temp.Pop());
        }
        #endregion

        bool isRoad(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;
            if (map[x, y] == 0)
                return false;
            else
                return true;
        }

        //Lay chi so cua o vuong 
        public int GetIndex(int cellY,int cellX)
        {
            if (cellX < 0 || cellX >= Width || cellY < 0 || cellY >= Height)
                return -1;
            else
                return map[cellY, cellX];
        }

        //Kiểm tra xem map[Y,X] đã có trong waypoints chưa
        bool isContained(Stack<Vector2> waypoints, int Y, int X)
        {
            foreach (Vector2 waypoint in waypoints)
            {
                if ((int)(waypoint.X / Container.tileSize) == X && (int)(waypoint.Y / Container.tileSize) == Y)
                    return true;
            }
            return false;
        }

        //LoadContent
        public void LoadContent(ContentManager Content)
        {
            for (int k = 0; k < 5; k++)
            {
                string sTile = "tile_" + level.ToString("d2") + "_" + k.ToString("d2");
                Texture2D tile = Content.Load<Texture2D>(sTile);
                tileTextureList.Add(tile);
                if (k < 5)
                {
                    string sRoad = "road_" + level.ToString("d2") + "_00";  
                    Texture2D road = Content.Load<Texture2D>(sRoad);
                    roadTextureList.Add(road);
                }
                
            }
                
        }

        int CheckRoad(int Y, int X)
        {
            if(map[Y,X]==0)
            {
                if (Y < Height - 1 && X < Width - 1 && Y - 1 >= 0 && X - 1 >= 0)
                {
                    if (map[Y - 1, X] == 1 && map[Y, X - 1] == 1 && map[Y, X + 1] == 1 && map[Y+1,X]==1)
                        return -6;
                    if (map[Y - 1, X] == 1 && map[Y - 1, X + 1] == 1 && map[Y, X + 1] == 1)
                        return -2;
                    if (map[Y + 1, X + 1] == 1 && map[Y + 1, X] == 1 && map[Y, X + 1] == 1) 
                        return -3;
                    if (map[Y + 1, X] == 1 && map[Y + 1, X - 1] == 1 && map[Y, X - 1] == 1)
                        return -4;
                    if (map[Y - 1, X] == 1 && map[Y - 1, X - 1] == 1 && map[Y, X - 1] == 1)
                        return -5;
                }
                if(Y==Height-1)
                {
                    if (map[Y - 1, X] == 1 && map[Y - 1, X - 1] == 1 && map[Y, X - 1] == 1)
                        return -5;
                    if(!(X==Width-1))
                        if (map[Y - 1, X] == 1 && map[Y - 1, X + 1] == 1 && map[Y, X + 1] == 1)
                            return -2;
                }
                if (Y == 0 && X < Width - 1 && X -1 >=0) 
                {
                    if (map[Y, X - 1] == 1 && map[Y + 1, X - 1] == 1 && map[Y + 1, X] == 1) 
                        return -4;
                    if (map[Y, X + 1] == 1 && map[Y + 1, X + 1] == 1 && map[Y + 1, X] == 1)
                        return -3;
                }
                return -1; //Grass
            }
            if(Y<Height-1 && X<Width-1&&Y-1>=0 &&X-1>=0)
            {
                if (map[Y, X] == 1 && map[Y + 1, X] == 1 && map[Y, X - 1] == 1 && map[Y, X + 1] == 1)
                    return 0;
                if (map[Y, X] == 1 && map[Y + 1, X] == 1 && map[Y - 1, X] == 1 && map[Y, X + 1] == 1) 
                    return 0;
                if (map[Y, X] == 1 && map[Y + 1, X] == 1 && map[Y - 1, X] == 1 && map[Y, X - 1] == 1)
                    return 0;
                if (map[Y, X] == 1 && map[Y, X - 1] == 1 && map[Y + 1, X] == 1)
                    return 1;
                if (map[Y, X] == 1 && map[Y, X - 1] == 1 && map[Y - 1, X] == 1)
                    return 2;
                if (map[Y, X] == 1 && map[Y, X + 1] == 1 && map[Y - 1, X] == 1)
                    return 3;
            }
            return 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = Color.Tan;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    
                    position.X=x*Container.tileSize;
                    position.Y=y*Container.tileSize;
                    Texture2D texture = null;
                    texture = roadTextureList[0];
                    spriteBatch.Draw(texture, position, color);
                    int k = CheckRoad(y, x);
                    if(k<0)
                    {
                        texture = roadTextureList[0];
                        spriteBatch.Draw(texture, position, color);
                        texture = tileTextureList[-1 - k];
                        spriteBatch.Draw(texture, position, color);
                    }
                    else
                    {
                        texture = roadTextureList[k];
                        spriteBatch.Draw(texture, position, color);
                    }
                }
            }
        }

    }
}
