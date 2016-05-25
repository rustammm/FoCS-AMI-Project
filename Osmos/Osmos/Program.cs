using System;

namespace Osmos
{
    
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (OsmosGame game = new OsmosGame())
            {
                if (args.Length > 1)
                {
                    game.SERVER_IP = "127.0.0.1";
                    game.Name = args[1];
                    if (args.Length > 2)
                        game.SERVER_IP = args[2];
                }
                game.Run();
            }
        }
    }
#endif
}

