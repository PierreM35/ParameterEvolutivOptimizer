using ParameterEvolutivOptimizer.Abstractions;
using System;

namespace ParameterEvolutivOptimizer.Model
{
    public class GeneInteger : IMutatable
    {
        private static readonly Random _rnd = new Random(0);
        private readonly int _minimum;
        private readonly int _maximum;

        public int IsValue { get; set; }

        public GeneInteger(int minimum, int maximum)
        {
            if (minimum > maximum)
                throw new ArgumentException("min > max");

            _minimum = minimum;
            _maximum = maximum;

            IsValue = _rnd.Next(_minimum, _maximum);
        }

        public GeneInteger(int minimum, int maximum, int isValue) : this(minimum, maximum)
        {
            IsValue = isValue;
        }

        public void Mutate(double mutateRange)
        {
            var range = Convert.ToInt16(Math.Ceiling(IsValue * mutateRange / 2));
            IsValue = _rnd.Next(IsValue - range, IsValue + range);
        }

        public IMutatable Clone()
        {
            return new GeneInteger(_minimum, _maximum);
        }

        public bool Equals(IMutatable other)
        {
            if (!(other is GeneInteger geneInteger))
                return false;

            return IsValue == geneInteger.IsValue;
        }

        public override string ToString()
        {
            return string.Format($"[{_minimum} - {IsValue} - {_maximum}]");
        }
    }
}
