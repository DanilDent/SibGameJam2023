using JHelpers;

namespace GameTime
{
    public class ClockStepEnterSignal : ISignal
    {
        public readonly int Step;

        public ClockStepEnterSignal(int quater)
        {
            Step = quater;
        }
    }
}
