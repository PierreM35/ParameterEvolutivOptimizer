using ParameterEvolutivOptimizer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ParameterEvolutivOptimizer.Model
{
    public class Solver
    {
        private readonly Func<Individual> _randomCreator;
        private readonly int _popSize;
        private readonly double _tau;
        private readonly int _immigrationPressure;
        private readonly int _generationNb;
        private readonly int _convergenceCriteria;
        private Population _population;

        /// <summary>
        /// Class to solve a parameter optimisation problem using an evolution algorythm
        /// </summary>
        /// <param name="randomCreator">Function that create randam individual</param>
        /// <param name="popSize"></param>
        /// <param name="tau">Selection pressure: the lower, the less individuals (among the bests) get the right to reproduce</param>
        /// <param name="immigrationPressure">Sets the immigration (5 => 5 immigrants per generation)</param>
        /// <param name="generationNb">Number of iterations (= generation) the algorythm will executed</param>
        /// <param name="convergenceCriteria">If no improvments are achieved over that number of iterations, the solver stops.</param>
        public Solver(Func<Individual> randomCreator, int popSize = 100, double tau = 0.4, int immigrationPressure = 5, int generationNb = 1000, int convergenceCriteria = 100)
        {
            _randomCreator = randomCreator;
            _popSize = popSize;
            _tau = tau;
            _immigrationPressure = immigrationPressure;
            _generationNb = generationNb;
            _convergenceCriteria = convergenceCriteria;
        }

        public IEnumerable<Individual> Solve(IProgress<Status> progress, CancellationToken cancelToken, int reportFrequency = 100)
        {
            _population = CreatePopulation();
            ImprovePopulation(progress, reportFrequency, cancelToken);

            return _population.AsList();
        }

        public IEnumerable<Individual> GetPopulation()
        {
            return _population.AsList();
        }

        #region Private helpers

        private void ImprovePopulation(IProgress<Status> progress, int reportFrequency, CancellationToken cancelToken)
        {
            int iterationWithoutImprovments = 0;
            for (int i = 0; i < _generationNb; i++)
            {
                if (cancelToken.IsCancellationRequested)
                    break;

                _population.HasImproved = false; 
                DoReproduction();
                DoImmigration();

                iterationWithoutImprovments = _population.HasImproved ? 0 : iterationWithoutImprovments + 1;
                if (iterationWithoutImprovments > _convergenceCriteria)
                    break;                

                if (progress != null && i % reportFrequency == 0)
                    progress.Report(new Status(i, _population.BestIndividual.Responses.ToList()));
            }
        }

        private void DoReproduction()
        {
            var parents = _population.SelectRandomFrom(2, (int)(_tau * _popSize));
            var children = parents[0].Reproduce(parents[1]);
            foreach (var child in children)
                _population.Add(child);
        }

        private Population CreatePopulation()
        {
            var population = new Population(_popSize);
            for (int i = 0; i < _popSize; ++i)
                population.Add(_randomCreator.Invoke());

            return population;
        }

        private void DoImmigration()
        {
            for (int i = 0; i < _immigrationPressure; i++)
                _population.Add(_randomCreator.Invoke());
        }

        #endregion
    }
}
