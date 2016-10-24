using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace TD
{
    class Hover
    {
        private MouseState mouseState;
        private MouseState oldState;

        private Texture2D texture;
        private Vector2 possition;
        private Color color;

        public Hover()
        {
            texture = null;
        }

    }
}
