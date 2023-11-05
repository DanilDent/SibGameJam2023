using JHelpers;

namespace GameTime
{
    public class ClockAtStepSignal : ISignal
    {
        public readonly int Step;

        public ClockAtStepSignal(int quater)
        {
            Step = quater;
        }
    }
}
