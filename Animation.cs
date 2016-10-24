using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TD
{
    class Animation
    {
        int currentFrameX, currentFrameY;
        Vector2 position, origin;
        Rectangle sourRec;
        Texture2D animationTexture;
        int width, height;
        float timer, interval;
        bool isVisible;

        //Constructor
        public Animation(Vector2 newPosition)
        {
            currentFrameX = 0;
            currentFrameY = 0;
            position = newPosition;
            origin = new Vector2(0, 0);
            //sourRec = new Rectangle(0,0,0,0) ;
            animationTexture = null;
            width = Container.animationWidth;
            height = Container.animationHeight;
            timer = 0f;
            interval = 100f;
            isVisible = true;
        }

        //Update
        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer > interval)
            {
                timer = 0;
                if (currentFrameX == Container.animationFrameXMax)
                {
                    currentFrameX = 0;
                    currentFrameY++;
                }
                else
                    currentFrameX++;
            }
            //On the last frame -> set isVisible=false
            if(currentFrameY>Container.animationFrameYMax)
            {
                currentFrameY = 0;
                currentFrameX = 0;
                isVisible = false;
            }
            sourRec = new Rectangle(currentFrameX * width, currentFrameY * height, width, height);
            origin = new Vector2(sourRec.Width / 2, sourRec.Height / 2);
        }
        

        //Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            if(isVisible)
                spriteBatch.Draw(animationTexture, position, sourRec, Color.White, 0f, origin, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
