using ParameterEvolutivOptimizer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParameterEvolutivOptimizer.Model
{
    public class Population
    {
        private static readonly Random _rnd = new Random(0);
        private readonly int _popSize;
        private readonly LinkedList<Individual> _individuals;
        private Response[] _worstResponses;
        private Response[] _bestResponses;

        public Individual BestIndividual => _individuals.First.Value;
        public bool HasImproved { get; internal set; }

        public Population(int popSize)
        {
            _popSize = popSize;
            _individuals = new LinkedList<Individual>();
        }

        public void Add(Individual individual)
        {
            if (_individuals.Contains(individual))
                return;

            UpdateBestResponses(individual);

            if (_individuals.Count == 0 || individual.CompareTo(_individuals.First.Value) > 0)
                _individuals.AddFirst(individual);
            else
                Add(_individuals.First, individual);

            if (_individuals.Count > _popSize)
                _individuals.RemoveLast();
        }

        public Individual[] SelectRandomFrom(int nbToSelect, int groupSize)
        {
            if (nbToSelect >= groupSize)
                throw new ArgumentException("nbToSelect must be < than size of population we select from");

            var selectedIndividuals = new Individual[nbToSelect];
            for (int i = 0; i < nbToSelect; i++)
            {
                int index = _rnd.Next(0, groupSize);
                selectedIndividuals[i] = _individuals.Nodes().ToList()[index].Value;
            }

            return selectedIndividuals;
        }

        internal IEnumerable<Individual> AsList()
        {
            return _individuals;
        }

        #region Private helpers

        private void UpdateBestResponses(Individual individual)
        {
            if (_bestResponses == null)
            {
                InitRefResponses(individual);
                return;
            }

            bool extremResponsesChanged = false;
            for (int i = 0; i < _bestResponses.Length; i++)
            {
                var newResponse = individual.Responses[i];
                if (newResponse.IsBetterThan(_bestResponses[i]))
                {
                    _bestResponses[i] = newResponse.Clone();
                    extremResponsesChanged = true;
                    HasImproved = true;
                }
                if (newResponse.IsWorstThan(_worstResponses[i]))
                {
                    _worstResponses[i] = newResponse.Clone();
                    extremResponsesChanged = true;
                }
            }

            if (extremResponsesChanged)
                foreach (var i in _individuals)
                    i.ComputeEvaluation(_bestResponses, _worstResponses);

            individual.ComputeEvaluation(_bestResponses, _worstResponses);
        }

        private void InitRefResponses(Individual individual)
        {
            _bestResponses = new Response[individual.Responses.Length];
            _worstResponses = new Response[individual.Responses.Length];
            for (int i = 0; i < individual.Responses.Length; i++)
            {
                _bestResponses[i] = individual.Responses[i].Clone();
                _worstResponses[i] = individual.Responses[i].Clone();
            }
        }

        private void Add(LinkedListNode<Individual> currentNode, Individual genome)
        {
            if (currentNode.Next == null || genome.CompareTo(currentNode.Next.Value) > 0)
                _individuals.AddAfter(currentNode, genome);
            else
                Add(currentNode.Next, genome);
        }

        #endregion
    }
}
