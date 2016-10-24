using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TD
{
    class Player
    {
        #region Khai báo
        private int money;

        public int Money
        {
            get { return money; }
            set { money = value; }
        }

        private Level level;

        private int Round;

        List<Tower> towerList = new List<Tower>();

        private MouseState mouseState;
        private MouseState oldState;

        private Texture2D towerTexture;
        private Texture2D bulletTexture;
        private Texture2D baseTexture;

        private int cellX;

        public int CellX
        {
            get { return cellX; }
            set { cellX = value; }
        }

        private int cellY;

        public int CellY
        {
            get { return cellY; }
            set { cellY = value; }
        }

        private int tileX;
        private int tileY;
        #endregion

        #region Constructor
        public Player(Level level,Texture2D towerTexture, Texture2D baseTexture, Texture2D bulletTexture)
        {
            this.baseTexture = baseTexture;
            this.towerTexture = towerTexture;
            this.level = level;
            this.bulletTexture = bulletTexture;
        }
        #endregion

        #region Kiểm tra xem vị trí có thể xây Tower được không
        private bool IsCellClear()
        {
            bool inBounds = cellX >= 0 && cellY >= 0 && cellX < level.Width && cellY < level.Height;

            bool spaceClear = true;

            foreach (Tower tower in towerList)
            {
                spaceClear = (tower.Position != new Vector2(tileX, tileY));

                if (!spaceClear)
                    break;
            }

            bool onPath = (level.GetIndex(cellY, cellX) != 1);
            return inBounds && spaceClear && onPath;
        }
        #endregion

        #region Hàm update
        public void Update(GameTime gameTime, List<Enemy> enemyList)
        {
            mouseState = Mouse.GetState();

            cellX = (int)(mouseState.X / Container.towerSize);
            cellY = (int)(mouseState.Y / Container.towerSize);

            tileX = cellX * Container.roadSize;
            tileY = cellY * Container.roadSize;

            if (mouseState.LeftButton == ButtonState.Released && oldState.LeftButton == ButtonState.Pressed)
            {
                if (IsCellClear())
                {
                    Tower tower = new Tower(towerTexture, 1 , new Vector2(tileX, tileY), baseTexture, bulletTexture);
                    towerList.Add(tower);
                }
            }

            foreach (Tower tower in towerList)
            {
                if (tower.Target == null || !tower.IsInRange(tower.Target))//Nếu như không có target hoặc target nằm ngoài bán kính thì get target mới
                {
                    tower.GetClosestEnemy(enemyList);
                }
                tower.Update(gameTime);
            }
            oldState = mouseState;
        }
        #endregion

        #region Hàm Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tower tower in towerList)
            {
                tower.Draw(spriteBatch);
            }
        }
        #endregion

    }
}
