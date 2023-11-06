
using UnityEngine;
using JHelpers;

namespace GameFlow
{
    public class LevelLoaded : ISignal
    {
        public readonly Level Level;

        public LevelLoaded(Level level)
        {
            Level = level;
        }
    }
}