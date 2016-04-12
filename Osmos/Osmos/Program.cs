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
                game.Run();
            }
        }
    }
#endif
}

