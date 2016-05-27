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
using System.Net;

/// <TODO>
///  Display Center
///  Super Stupid AI
/// </TODO>

namespace OsmosLibrary
{
    [Serializable]
    public class Circle : IMovableObject
    {

        public static OsmosEventHandler handler;
        public static Random rnd = new Random();

        public static HashSet<Circle> ActiveInstance = new HashSet<Circle>();
        public static int MAX_WIDTH;
        public static int MAX_HEIGHT;
        public static float GRAVITY = 6.674e-11F;
        private Circle circle;

        static public Texture2D TextureBot { get; set; }
        static public Texture2D TextureUser { get; set; }
        static public Texture2D TexturePlayer { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 RelativePosition { get; set; }
        public Vector2 Force { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Radius { get; set; }
        public Color Color { get; set; }
        const int STD_SIZE = 50;

        bool mouseWasDown = false;


        // LAN
        public EndPoint IP { get; set; }
        public bool Bot;
        public bool User;


        public Circle()
        {
            this.Position = new Vector2(0, 0);
            this.Force = new Vector2(0, 0);
            this.Width = this.Height = 0;
            this.Color = Color.White;
            Radius = 0;
            User = false;
            Bot = true;
        }

        public Circle(Vector2 position, Vector2 force, float radius, Color color)
        {
            this.Position = position;
            this.Force = force;
            this.Radius = radius;
            this.Width = (int)radius;
            this.Height = (int)radius;
            this.Color = color;
            User = false;
            Bot = true;
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


        public void Update(List<Circle> circles, int myID)
        {
            Vector2 gForce = new Vector2(0, 0);
            for (int i = 0; i < circles.Count; i++)
            {
                if (i == myID) continue;
                Vector2 addGForce = circles[i].Position - this.Position;
                float f = GRAVITY * (circles[i].Radius * this.Radius) / addGForce.LengthSquared();
                f *= circles[i].Radius;
                addGForce.X *= f;
                addGForce.Y *= f;
                gForce += addGForce;
            }

            this.Position -= this.Force - gForce;
            this.Position = new Vector2(this.Position.X % MAX_WIDTH, this.Position.Y % MAX_HEIGHT);
            this.Position = new Vector2(this.Position.X + MAX_WIDTH, this.Position.Y + MAX_HEIGHT);
            this.Position = new Vector2(this.Position.X % MAX_WIDTH, this.Position.Y % MAX_HEIGHT);
        }

        public static Circle getRandom()
        {
            Circle ret = new Circle(new Vector2(rnd.Next(MAX_WIDTH), rnd.Next(MAX_HEIGHT)),
                    new Vector2(rnd.Next(-2, 2), rnd.Next(-2, 2)), rnd.Next(40, 60), Color.Blue);
            return ret;
        }

        public bool OnMouseDown(MouseState mouseState, Vector2 display)
        {
            bool ret = false;

            if (mouseState.X < 0 || mouseState.Y < 0) return false;
            if (mouseState.X > display.X || mouseState.Y > display.Y) return false;
            
            if (mouseState.LeftButton == ButtonState.Pressed && !mouseWasDown)
            {
                Vector2 force = new Vector2((mouseState.X - getCenter(RelativePosition).X) / 100,
                    (mouseState.Y - getCenter(RelativePosition).Y) / 100);

                jetPropulsion(force);
                ret = true;                
            }
            mouseWasDown = mouseState.LeftButton == ButtonState.Pressed;
            return ret;
        }

        void jetPropulsion(Vector2 force)
        {
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



            //Circle newCircle = new Circle(newPos, newForce, newRad, Color.Blue);
            //newCircle.Activate();
            AddForce(force);
        }

        public void superCleverAI(Random rnd)
        {
            Vector2 addForce = new Vector2(0, 0);

            addForce.X = (float) (rnd.NextDouble() * rnd.NextDouble() * rnd.NextDouble());
            addForce.Y = (float) (rnd.NextDouble() * rnd.NextDouble() * rnd.NextDouble());
            addForce.X *= addForce.X;
            addForce.Y *= addForce.Y;

            if (addForce.Length() < 0.9 || Radius < 20) return;

            addForce.X *= (float)rnd.Next(-5, 5);
            addForce.Y *= (float)rnd.Next(-5, 5);

            jetPropulsion(addForce);
        }

        double recalcRadius(float Radius, float deltaSquare) {
            return Math.Max(Math.Sqrt(Math.Max(Radius * Radius + deltaSquare/ Math.PI, 0)), (double)0);
        }

        public void OnIntersectSmaller(Circle circle)
        {
            if (!intersects(circle)) return;

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
            if (Radius <= 0 || other.Radius <= 0) return false;
            Vector2 distance = other.getCenter(other.Position) - getCenter(Position);
            return distance.Length() <= other.Radius + Radius;
        }

        public void AddForce(Vector2 force)
        {
            Force += force;
        }

        public void Draw(SpriteBatch spriteBatch, int gamerRadius, bool localUser)
        {
            Texture2D thisTexture = TextureBot;
            if (IP != null || !Bot)
                thisTexture = TexturePlayer;
            if (localUser)
                thisTexture = TextureUser;

            double zoom = STD_SIZE / (gamerRadius * 1.0);
            zoom = 1;
            int newRadius = (int)(Radius * zoom); 
            if (Math.Abs(RelativePosition.X) * zoom <= MAX_WIDTH + Radius && Math.Abs(RelativePosition.Y) * zoom <= MAX_HEIGHT + Radius || true)
                spriteBatch.Draw(
                    thisTexture, 
                    new Rectangle((int)this.RelativePosition.X, (int)this.RelativePosition.Y, newRadius * 2, newRadius * 2), Color.White);
        }

        public Vector2 getCenter(Vector2 Position)
        {
            return new Vector2(Position.X + this.Radius, Position.Y + this.Radius);
        }

    }
}
