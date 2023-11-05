using JHelpers;

namespace Player 
{
    public class PlayerSignal : ISignal
    {
        public readonly Player Player;

        public PlayerSignal(Player player)
        {
            Player = player;
        }
    }
}

