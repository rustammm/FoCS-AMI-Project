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

        public static OsmosEventHandler handler;

        public static HashSet<Circle> ActiveInstance;
        public static int MAX_WIDTH;
        public static int MAX_HEIGHT;
        private Circle circle;

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 RelativePosition { get; set; }
        public Vector2 Force { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Radius { get; set; }
        public Color Color { get; set; }

        public Circle ()
        {
            this.Texture = null;
            this.Position = new Vector2(0, 0);
            this.Force = new Vector2(0, 0);
            this.Width = this.Height = 0;
            this.Color = Color.White;
            Radius = 0;
        }

        public Circle (Texture2D texture, Vector2 position, Vector2 force, int radius, Color color)
        {
            


            this.Texture = texture;
            this.Position = position;
            this.Force = force;
            this.Radius = radius;
            this.Width = radius;
            this.Height = radius;
            this.Color = color;

        }

        public void Activate()
        {
            if (ActiveInstance == null)
                ActiveInstance = new HashSet<Circle>();

            
            ActiveInstance.Add(this);
            handler.handleOnCircleIntersect(OnIntersect, this);
        }

        public void Deactivate()
        {
            ActiveInstance.Remove(this);
        }

        public Circle(Circle circle)
        {
            this.circle = circle;
        }

        ~Circle()
        {
            Deactivate();
        }
        

        public void Update()
        {
            this.Position -= this.Force;
            this.Position = new Vector2(this.Position.X % MAX_WIDTH, this.Position.Y % MAX_HEIGHT);
            this.Position = new Vector2(this.Position.X + MAX_WIDTH, this.Position.Y + MAX_HEIGHT);
            this.Position = new Vector2(this.Position.X % MAX_WIDTH, this.Position.Y % MAX_HEIGHT);
        }


        public void OnMouseDown(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                AddForce(new Vector2((mouseState.X - getCenter().X) / 1000,
                    (mouseState.Y - getCenter().Y) / 1000));
            }
        }

        void OnIntersect(Circle circle)
        {
            if (!intersects(circle)) return;
            if (Color != Color.Red) return;

            float distance = (circle.getCenter() - getCenter()).Length();
            float sumRadius = circle.Radius + Radius;
            float devidingArea = sumRadius - distance;

            int changedRadiusCircle = (int)(circle.Radius + devidingArea * (circle.Radius / sumRadius) - devidingArea/2);
            int changeRadiusThis = (int)(Radius + devidingArea * (Radius / sumRadius) - devidingArea/2);

            Console.Write("Debug OnIntersect:\n");
            Console.Write("Radius1 = " + circle.Radius + "\n");
            Console.Write("Radius2 = " + Radius + "\n");

            Console.Write("devideArea = " + devidingArea + "\n");

            circle.Radius = changedRadiusCircle;
            this.Radius = changeRadiusThis;
            Console.Write("Changed Radius1 = " + circle.Radius + "\n");
            Console.Write("Changed Radius2 = " + Radius + "\n");

        }

        public bool intersects(Circle other)
        {
            Vector2 distance = other.getCenter() - getCenter();
            return distance.Length() <= other.Radius + Radius;
        }

        public void AddForce(Vector2 force)
        {
            Force += force;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Radius <= 0) {
                this.Deactivate();
            }
            if (Math.Abs(Position.X) <= MAX_WIDTH + Radius && Math.Abs(Position.Y) <= MAX_HEIGHT + Radius)
                spriteBatch.Draw(
                    this.Texture, 
                    new Rectangle((int)this.RelativePosition.X, (int)this.RelativePosition.Y, this.Radius * 2, this.Radius * 2), 
                    this.Color);
        }

        public Vector2 getCenter()
        {
            return new Vector2(this.Position.X + this.Width / 2, this.Position.Y + this.Height / 2);
        }
    }
}
