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

namespace OsmosLibrary
{
    public class Circle: IMovableObject
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Force { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }

        public Circle ()
        {
            this.Texture = null;
            this.Position = new Vector2(0, 0);
            this.Force = new Vector2(0, 0);
            this.Width = this.Height = 0;
            this.Color = Color.White;
        }

        public Circle (Texture2D texture, Vector2 position, Vector2 force, int width, int height, Color color)
        {
            this.Texture = texture;
            this.Position = position;
            this.Force = force;
            this.Width = width;
            this.Height = height;
            this.Color = color;
        }

        

        public void Update()
        {
            this.Position -= this.Force;
        }


        public void OnMouseDown(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                AddForce(new Vector2((mouseState.X - getCenter().X) / 1000,
                    (mouseState.Y - getCenter().Y) / 1000));
            }
        }


        public void AddForce(Vector2 force)
        {
            Force += force;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, new Rectangle((int)this.Position.X, (int)this.Position.Y, 
                this.Width, this.Height), this.Color);
        }

        public Vector2 getCenter()
        {
            return new Vector2(this.Position.X + this.Width / 2, this.Position.Y + this.Height / 2);
        }
    }
}
