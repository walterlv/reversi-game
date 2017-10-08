using System;

namespace ReversiXNAGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ReversiXNAGame game = new ReversiXNAGame())
            {
                game.Run();
            }
        }
    }
#endif
}

