using System;

namespace ParameterEvolutivOptimizer.Model
{
    public class Response
    {
        private readonly bool _isToMaximize;
        private readonly double _weight;

        private double _minAllowed;
        private double _maxAllowed;
        private double _evaluation;

        public string Name { get; }
        public double Value { get; set; }
        public double Evaluation 
        {
            get => _evaluation;
            internal set
            {
                if (_evaluation == value)
                    return;

                _evaluation = value;
                OnEvaluationChanged();
            }
        }

        public Response(string name, double value, bool isToMaximize, double weight)
        {
            Name = name;
            Value = value;
            _isToMaximize = isToMaximize;
            _weight = weight;
        }

        public event EventHandler EvaluationChanged;
        private void OnEvaluationChanged() => EvaluationChanged?.Invoke(this, EventArgs.Empty);

        public bool IsBetterThan(Response other)
        {
            return _isToMaximize ? Value > other.Value : Value < other.Value;
        }

        public bool IsWorstThan(Response other)
        {
            return _isToMaximize ? Value < other.Value : Value > other.Value;
        }

        public void Evaluate(Response bestResponse, Response worstResponse)
        {
            if (bestResponse.Value == worstResponse.Value)
                Evaluation = 0;
            else
            {
                var a = 1 / (bestResponse.Value - worstResponse.Value);
                var b = -a * worstResponse.Value;

                Evaluation = Math.Round((a * Value + b) * _weight, 3);
            }
        }

        internal Response Clone()
        {
            return new Response(Name, Value, _isToMaximize, _weight);
        }

        public override string ToString()
        {
            return $"{Name}: {Value}, {(_isToMaximize ? "ToMaximize" : "ToMinimize")}";
        }
    }
}
