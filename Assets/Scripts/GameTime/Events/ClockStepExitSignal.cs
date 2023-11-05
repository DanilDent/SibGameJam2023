using JHelpers;

namespace GameTime
{
    public class ClockStepExitSignal : ISignal
    {
        public readonly int Step;

        public ClockStepExitSignal(int quater)
        {
            Step = quater;
        }
    }
}
