using System;

using ReversiXNAGame.Players;

namespace ReversiXNAGame.Settings
{
    class PlayerSetting
    {
        public PlayerTypes Type;
        public int AIIndex;
    }

    internal static class PlayerSettings
    {
        static bool isInitialized = false;
        static PlayerSetting[] player = new PlayerSetting[2];
        public static PlayerSetting[] Player
        {
            get
            {
                if (!isInitialized) initialize();
                return player;
            }
            set
            {
                player = value;
            }
        }

        private static void initialize()
        {
            player[0] = new PlayerSetting();
            player[1] = new PlayerSetting();
            isInitialized = true;
        }
    }
}
