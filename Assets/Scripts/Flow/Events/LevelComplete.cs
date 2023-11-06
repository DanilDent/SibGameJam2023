using UnityEngine;
using JHelpers;

namespace GameFlow
{
    public class LevelComplete : ISignal
    {
        public readonly Level Level;

        public LevelComplete(Level level)
        {
            Level = level;
        }
    }
}