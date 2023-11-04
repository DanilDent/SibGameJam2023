using JHelpers;

namespace GameTime
{
    public class ClockStepSignal : ISignal
    {
        public readonly int Step;

        public ClockStepSignal(int quater)
        {
            Step = quater;
        }
    }
}
