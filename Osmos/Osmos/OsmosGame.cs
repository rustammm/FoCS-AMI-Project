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
using OsmosLibrary;

namespace Osmos
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class OsmosGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D circleTexture;
        Circle circleLocalGamer;
        HashSet<Circle> circleBots;
        int BOTS_NUM = 100;
        const int MAX_WIDTH = 10000;
        const int MAX_HEIGHT = 10000;
        Vector2 displayCenter;
        OsmosEventHandler handler;
        Random rnd = new Random();

        
        public OsmosGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            handler = new OsmosEventHandler();
            Circle.handler = handler;
            Circle.MAX_HEIGHT = MAX_HEIGHT;
            Circle.MAX_WIDTH = MAX_WIDTH;

            circleLocalGamer = new Circle(circleTexture, 
                new Vector2(rnd.Next(0, 0), rnd.Next(0, 0)), // R
                new Vector2(0, 0), 50, Color.Red);

            circleLocalGamer.Activate();


            circleBots = new HashSet<Circle>();
            for (int i = 0; i < BOTS_NUM; i++)
            {
                circleBots.Add(new Circle(circleTexture,
                    new Vector2(rnd.Next(MAX_WIDTH), rnd.Next(MAX_HEIGHT)),
                    new Vector2(rnd.Next(-2, 2), rnd.Next(-2, 2)), rnd.Next(5, 60), Color.Blue));
                circleBots.Last<Circle>().Activate();
            }

            this.IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
         // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            circleTexture = Content.Load<Texture2D>("circle");
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // if ((cntUpdates++) % 10 != 0) return; 

            if (circleLocalGamer.Radius <= 0)
                this.Exit();


            circleLocalGamer.OnMouseDown(Mouse.GetState());
            handler.OnCircleIntersectCircles = Circle.ActiveInstance;
            handler.listenAndHandle();
            // TODO: Add your update logic here

            
            displayCenter = 
                new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);


            List<Circle> toDelete = new List<Circle>();
            List<Circle> allBotCircles = new List<Circle>();

            foreach (Circle activeCircle in Circle.ActiveInstance)
            {
                activeCircle.Update();
                if (activeCircle != circleLocalGamer)
                    allBotCircles.Add(activeCircle);
                if (activeCircle.Radius <= 0)
                {
                    toDelete.Add(activeCircle);
                }     
            }

            foreach (Circle deactive in toDelete)
            {
                deactive.Deactivate();
            }

            foreach (Circle deactive in allBotCircles)
            {
                deactive.superCleverAI(rnd);
            }


            Vector2 DPosition = displayCenter - circleLocalGamer.Position;



            foreach (Circle activeCircle in Circle.ActiveInstance)
            {
                activeCircle.RelativePosition = activeCircle.Position + DPosition;
            }

            // DEBUG
            //Console.Write("DPos: " + DPosition + "\n");
            //Console.Write("dispalayCenter: " + displayCenter + "\n");
            //Console.Write("UserRadius: " + circleLocalGamer.Radius + "\n");
            //Console.Write("User Rel Pos: " + circleLocalGamer.RelativePosition + "\n");

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach (Circle activeCircle in Circle.ActiveInstance)
            {
                activeCircle.Draw(spriteBatch);
            }
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
