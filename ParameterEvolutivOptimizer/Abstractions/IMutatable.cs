using System;

namespace ParameterEvolutivOptimizer.Abstractions
{
    public interface IMutatable : IEquatable<IMutatable>
    {
        /// <param name="mutateRange">Set in percents of the actual value the range around the actual value in which the new valule (after mutation) must be.</param>
        void Mutate(double mutateRange);
        IMutatable Clone();
    }
}
