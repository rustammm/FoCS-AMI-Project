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

/// <TODO>
///  Display Center
///  Super Stupid AI
/// </TODO>

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
        public float Radius { get; set; }
        public Color Color { get; set; }


        bool mouseWasDown = false;

        public Circle ()
        {
            this.Texture = null;
            this.Position = new Vector2(0, 0);
            this.Force = new Vector2(0, 0);
            this.Width = this.Height = 0;
            this.Color = Color.White;
            Radius = 0;
        }

        public Circle (Texture2D texture, Vector2 position, Vector2 force, float radius, Color color)
        {
            


            this.Texture = texture;
            this.Position = position;
            this.Force = force;
            this.Radius = radius;
            this.Width = (int)radius;
            this.Height = (int)radius;
            this.Color = color;

        }

        public void Activate()
        {
            if (ActiveInstance == null)
                ActiveInstance = new HashSet<Circle>();

            
            ActiveInstance.Add(this);
            handler.handleOnCircleIntersect(OnIntersectSmaller, this);
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
            if (mouseState.LeftButton == ButtonState.Pressed && !mouseWasDown)
            {
                Vector2 force = new Vector2((mouseState.X - getCenter(RelativePosition).X) / 100,
                    (mouseState.Y - getCenter(RelativePosition).Y) / 100);


                float forceAndMass = (float)Math.Sqrt(Math.PI * Radius * Radius * force.Length());

                float newRad = (float)recalcRadius((float)0, forceAndMass);

                float newRadius = (float)recalcRadius(this.Radius, -forceAndMass);
                this.Position += new Vector2(Radius - newRadius, Radius - newRadius);


                this.Radius = newRadius;


                Vector2 newPos = force;
                newPos.Normalize();
                newPos.X *= Radius + newRad + 1;
                newPos.Y *= Radius + newRad + 1;
                newPos += getCenter(Position);
                newPos -= new Vector2(newRad, newRad);


                Vector2 newForce = -force;
                newForce.Normalize();
                newForce.X *= forceAndMass / 10;
                newForce.Y *= forceAndMass / 10;
                newForce += Force;
                


                Circle newCircle = new Circle(Texture, newPos, newForce, newRad, Color.Blue);
                newCircle.Activate();

                AddForce(force);
            }
            mouseWasDown = mouseState.LeftButton == ButtonState.Pressed;
        }


        double recalcRadius(float Radius, float deltaSquare) {
            return Math.Max(Math.Sqrt(Math.Max(Radius * Radius + deltaSquare/ Math.PI, 0)), (double)0);
        }

        void OnIntersectSmaller(Circle circle)
        {
            if (!intersects(circle)) return;
            if (Color != Color.Red) return;

            if (this.Radius >= 50)
                this.Radius = this.Radius;

            float distance = (circle.getCenter(circle.Position) - getCenter(Position)).Length();
            float sumRadius = circle.Radius + Radius;
            float devidingArea = sumRadius - distance;

            //int changedRadiusCircle = (int)(circle.Radius + devidingArea * (circle.Radius / sumRadius) - devidingArea/2);
            //int changeRadiusThis = (int)(Radius + devidingArea * (Radius / sumRadius) - devidingArea/2);

            float intersectHeight = (float)Math.Sqrt(Radius * Radius - (Radius - devidingArea/2) * (Radius - devidingArea / 2));
            float apprSquare = devidingArea * intersectHeight;

            float changedRadiusCircle = (float)recalcRadius(circle.Radius, -apprSquare);
            float changeRadiusThis = (float)recalcRadius(this.Radius, apprSquare);


            circle.Position += new Vector2(circle.Radius - changedRadiusCircle, circle.Radius - changedRadiusCircle);
            circle.Radius = changedRadiusCircle;
            Position -= new Vector2(this.Radius - changeRadiusThis, this.Radius - changeRadiusThis);
            this.Radius = changeRadiusThis;

            //Console.Write("Debug OnIntersect:\n");
            //Console.Write("Radius1 = " + circle.Radius + "\n");
            //Console.Write("Radius2 = " + Radius + "\n");

            //Console.Write("devideArea = " + devidingArea + "\n");

            //Console.Write("Changed Radius1 = " + changedRadiusCircle + "\n");
            //Console.Write("Changed Radius2 = " + changeRadiusThis + "\n");

        }

        public bool intersects(Circle other)
        {
            Vector2 distance = other.getCenter(other.Position) - getCenter(Position);
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
            if (Math.Abs(RelativePosition.X) <= MAX_WIDTH + Radius && Math.Abs(RelativePosition.Y) <= MAX_HEIGHT + Radius)
                spriteBatch.Draw(
                    this.Texture, 
                    new Rectangle((int)this.RelativePosition.X, (int)this.RelativePosition.Y, (int)this.Radius * 2, (int)this.Radius * 2), 
                    this.Color);
        }

        public Vector2 getCenter(Vector2 Position)
        {
            return new Vector2(Position.X + this.Radius, Position.Y + this.Radius);
        }

    }
}
