using ParameterEvolutivOptimizer.Abstractions;
using System;

namespace ParameterEvolutivOptimizer.Model
{
    public class GeneDouble : IMutatable
    {
        private static readonly Random _rnd = new Random(0);

        private readonly double _minimum;
        private readonly double _maximum;

        public double IsValue { get; private set; }

        public GeneDouble(double min, double max)
        {
            if (min > max)
                throw new ArgumentException("min > max");

            _minimum = min;
            _maximum = max;

            IsValue = _minimum + (_maximum - _minimum) * _rnd.NextDouble();
        }

        public void Mutate(double mutateRange)
        {
            var range = IsValue * mutateRange;
            IsValue = IsValue - range / 2 + _rnd.NextDouble() * range;
        }

        public IMutatable Clone()
        {
            return new GeneDouble(_minimum, _maximum);
        }

        public bool Equals(IMutatable other)
        {
            if (!(other is GeneDouble geneDouble))
                return false;

            return IsValue == geneDouble.IsValue;
        }

        public override string ToString()
        {
            return string.Format($"[{_minimum} - {IsValue} - {_maximum}]");
        }
    }
}
