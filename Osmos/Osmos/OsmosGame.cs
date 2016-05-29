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
using OsmosMultiplayer4;
using System.Net;
using System.IO;

namespace Osmos
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class OsmosGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D TextureBot { get; set; }
        Texture2D TextureUser { get; set; }
        Texture2D TexturePlayer { get; set; }
        List<Texture2D> achievments;

        Texture2D Background { get; set; }
        public Vector2 BackGroundPosition { get; set; }

        Vector2 Display;
        
        List<Circle> circles;

        int BOTS_NUM = 2;
        int MAX_WIDTH = 1920;
        int MAX_HEIGHT = 1080;
        const int UPD_FREQ = 5;

        Vector2 displayCenter;
        OsmosEventHandler handler;
        Random rnd = new Random();
        int cnt = 0;
        

        // LAN
        public string SERVER_IP;
        public string Name;

        public Client client = null;
        public Server server = null;
        public int userID;

        public OsmosGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }


        protected override void Initialize()
        {

            base.Initialize();

            using (StreamReader sr = new StreamReader("Server.info"))
            {
                SERVER_IP = sr.ReadLine();
                if (SERVER_IP == "server")
                {
                    SERVER_IP = null;
                    BOTS_NUM = Int32.Parse(sr.ReadLine());
                }

            }


            if (SERVER_IP == null)
            {
                server = new Server();
                server.start();
            }
            else
            {
                client = new Client(SERVER_IP, Server.PORT, Name);
            }



            handler = new OsmosEventHandler();

            for (int i = 0; i < handler.Achievements.Count; i++)
                handler.Achievements[i].Texture = achievments[i];
            
            Circle.MAX_HEIGHT = MAX_HEIGHT;
            Circle.MAX_WIDTH = MAX_WIDTH;
            circles = new List<Circle>();

            this.IsMouseVisible = true;
            Mouse.WindowHandle = Window.Handle;

            if (server == null) return;
            userID = 0;
            circles = new List<Circle>();
            circles.Add(Circle.getRandom());
            circles.Last<Circle>().User = true;
            circles.Last<Circle>().Activate();
            circles[userID].Bot = false;

            for (int i = 0; i < BOTS_NUM; i++)
            {
                circles.Add(Circle.getRandom());
                circles.Last<Circle>().Activate();
            }

        }


        protected override void LoadContent()
        {
            achievments = new List<Texture2D>();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Achievments

            achievments.Add(Content.Load<Texture2D>("Achievments\\bigger70"));
            achievments.Add(Content.Load<Texture2D>("Achievments\\bigger100"));
            achievments.Add(Content.Load<Texture2D>("Achievments\\absorb1"));
            achievments.Add(Content.Load<Texture2D>("Achievments\\absorb3"));
            achievments.Add(Content.Load<Texture2D>("Achievments\\absorb10"));


            // Circles
            TextureUser   = Content.Load<Texture2D>("circle");
            TextureBot    = Content.Load<Texture2D>("cicleBot");
            TexturePlayer = Content.Load<Texture2D>("circlePlayer");

            // Background
            Background = Content.Load<Texture2D>("background");
        
        }


        protected override void UnloadContent()
        {
           
        }

        protected override void Update(GameTime gameTime)
        {
            Display = new Vector2(GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height);
            cnt++;
            onReceiveClient();
            onReceiveServer();


            Circle.handler = handler;
            Circle.TextureBot = TextureBot;
            Circle.TexturePlayer = TexturePlayer;
            Circle.TextureUser = TextureUser;


            if (circles.Count == 0) return;
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (circles[userID].Radius <= 0)
            {
                // LogOut
                if (server == null)
                {
                    client.sendMessage(DataType.LogOut, null, 0, Name);
                } 
                this.Exit();
            }

            bool onMouseDown = circles[userID].OnMouseDown(Mouse.GetState(), Display);

            handler.UserCircle = circles[userID];
            handler.OnCircleIntersectCircles = circles;
            handler.listenAndHandle();
            
            
            displayCenter = 
                new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2 - circles[userID].Radius, 
                GraphicsDevice.Viewport.Bounds.Height / 2 - circles[userID].Radius);


            Vector2 userPrevPos = circles[userID].Position;

            List<Circle> allBotCircles = new List<Circle>();

            for (int i = 0; i < circles.Count; i++)
            {
                Circle activeCircle = circles[i];
                if (activeCircle.Radius <= 0) continue;
                activeCircle.Update(circles, i);
                if (activeCircle.IP == null && !activeCircle.User)
                    allBotCircles.Add(activeCircle);
            }
            sendUpdate(onMouseDown);

            //foreach (Circle active in allBotCircles)
            //{
            //    active.superCleverAI(rnd);
            //}

            Vector2 DPosition = displayCenter - circles[userID].Position;

            Vector2 newBackGroundPos = -circles[userID].Position; 
            BackGroundPosition = new Vector2(
                newBackGroundPos.X % Background.Width,
                newBackGroundPos.Y % Background.Height);
            //BackGroundPosition = new Vector2(BackGroundPosition.X + Background.Width, BackGroundPosition.Y + Background.Height);
            //BackGroundPosition = new Vector2(BackGroundPosition.X % Background.Width, BackGroundPosition.Y % Background.Height);


            foreach (Circle activeCircle in circles)
            {
                activeCircle.RelativePosition = activeCircle.Position + DPosition;
            }

            //Console.WriteLine("Radius is " + circles[userID].Radius);
            //Console.WriteLine("Relative Position is " + circles[userID].RelativePosition);

            base.Update(gameTime);
        }

       
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // Background
            List<KeyValuePair<int, int>> substract = new List<KeyValuePair<int, int>>();
            substract.Add(new KeyValuePair<int, int>(0, 0));
            substract.Add(new KeyValuePair<int, int>(-1, 0));
            substract.Add(new KeyValuePair<int, int>(0, -1));
            substract.Add(new KeyValuePair<int, int>(-1, -1));

            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(Background,
                new Rectangle(
                    (int)BackGroundPosition.X - Background.Width * (int)substract[i].Key, 
                    (int)BackGroundPosition.Y - Background.Height * (int)substract[i].Value, 
                    Background.Width, Background.Height), 
                Color.White);

            }

            // Achievments
            
            for (int i = 0; i < handler.Achievements.Count; i++) {
                var achiev = handler.Achievements[i];
                if (achiev.Show && !achiev.Achieved)
                {
                    achiev.ShowLeft--;
                    if (achiev.ShowLeft == 0)
                        achiev.Achieved = true;

                    spriteBatch.Draw(achiev.Texture,
                        new Rectangle((int)Display.X - achiev.Texture.Width, 0, achiev.Texture.Width, achiev.Texture.Height),
                        Color.White);
                    break;
                }
            }

            // Circles
            for (int i = 0; i < circles.Count; i++)
            {
                var activeCircle = circles[i];
                activeCircle.Draw(spriteBatch, (int)circles[userID].Radius, i == userID);
            }
            spriteBatch.End();


            base.Draw(gameTime);
        }


        void onReceiveServer() {
            if (server == null) return;

            List<Packet> packets = server.getAllReceived();
            foreach(Packet p in packets)
            {

                Packet sendPacket = new Packet();

                if (p.Type == DataType.LogIn)
                {
                    sendPacket.User = circles.Count;
                    circles.Add(Circle.getRandom());
                    circles[sendPacket.User].IP = p.IP;
                    p.Type = DataType.GetState;
                }


                if (p.Type == DataType.GetState)
                {
                    sendPacket.Circles = circles;
                    sendPacket.Type = DataType.GetState;
                    
                    for (int i = 0; i < circles.Count; i++)
                    {
                        Circle c = circles[i];
                        if (c.IP != null)
                        {
                            sendPacket.User = i;
                            server.sendPacketOne(sendPacket, c.IP);
                        }
                    }
                }


                if (p.Type == DataType.Update) {
                    updateCircle(p.Circles[0]);
                    server.sendPacket(p);
                }

            }

        }

        void onReceiveClient()
        {
            if (client == null) return;
            if (cnt % UPD_FREQ == 0)
            {
                client.sendMessage(DataType.GetState, null, 0, Name);
            }

            List<Packet> packets = client.getAllReceived();
            foreach(Packet p in packets)
            {
                if (p.Type == DataType.Update)
                {
                    updateCircle(p.Circles[0]);
                }

                if (p.Type == DataType.GetState)
                {
                    circles = p.Circles;
                    userID = p.User;
                }
            }

        }


        void updateCircle(Circle upd)
        {
            for (int i = 0; i < circles.Count; i++)
            {

                if (circles[i].IP == null && upd.IP == null)
                {
                    circles[i] = upd;
                    continue;
                }
                if (circles[i].IP != null && circles[i].IP.Equals(upd.IP))
                {
                    circles[i] = upd;
                    continue;
                }
            }
        }

        void sendUpdate(bool onMouseDown)
        {
            if (onMouseDown)
            {
                if (server != null)
                {
                    Packet p = new Packet();
                    p.Circles.Add(circles[userID]);
                    p.Type = DataType.Update;
                    p.SenderName = Name;
                    server.sendPacket(p);
                }
                else
                {
                    List<Circle> send = new List<Circle>();
                    send.Add(circles[userID]);
                    client.sendMessage(DataType.Update, send, 0, Name);
                }
            }
        }

    }
}
