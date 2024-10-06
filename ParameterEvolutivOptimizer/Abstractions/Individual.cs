using ParameterEvolutivOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParameterEvolutivOptimizer.Abstractions
{
    public abstract class Individual : IComparable<Individual>, IEquatable<Individual>
    {
        private static readonly Random _rnd = new Random();
        private readonly double _mutateProbability;
        private readonly double _mutateRange;

        protected List<IMutatable> Genes { get; private set; }
        public abstract Response[] Responses { get; }
        public double Evaluation { get; protected set; }

        protected Individual(List<IMutatable> genes, double mutateProbability = 0.2, double mutateRange = 0.01)
        {
            _mutateProbability = mutateProbability;
            _mutateRange = mutateRange;

            Genes = genes;
        }

        protected abstract Individual CreateFrom(List<IMutatable> genes);

        public void ComputeEvaluation(Response[] bestResponses, Response[] worstResponses)
        {
            for (int i = 0; i < Responses.Length; i++)
                Responses[i].Evaluate(bestResponses[i], worstResponses[i]);

            Evaluation = 0;
            foreach (var response in Responses)
                Evaluation += response.Evaluation;
        }

        internal Individual[] Reproduce(Individual with)
        {
            if (!with.GetType().Equals(GetType()))
                throw new ArgumentException("Reproduction requires both parents to have the same type");

            var mixedGenes = MixUp(with);

            return new Individual[2] { CreateGenomeFrom(mixedGenes[0].ToList()), CreateGenomeFrom(mixedGenes[1].ToList()) };
        }

        public virtual bool Equals(Individual other)
        {
            for (int i = 0; i < Genes.Count; i++)
                if (!Genes[i].Equals(other.Genes[i]))
                    return false;

            return true;
        }

        public int CompareTo(Individual other)
        {
            if (Evaluation < other.Evaluation)
                return -1;
            else if (Evaluation > other.Evaluation)
                return 1;
            else
                return 0;
        }
        
        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var r in Responses)
                builder.Append($", {r.Value}");

            return $"{builder.ToString().Substring(2)}-{Evaluation}";
        }

        #region Private helpers

        private Individual CreateGenomeFrom(List<IMutatable> genes)
        {
            foreach (var gene in genes)
            {
                double v = _rnd.NextDouble();
                if (v < _mutateProbability)
                    gene.Mutate(_mutateRange);
            }

            return CreateFrom(genes);
        }

        private IMutatable[][] MixUp(Individual with)
        {
            var genes1 = new IMutatable[Genes.Count];
            var genes2 = new IMutatable[Genes.Count];
            int cross = _rnd.Next(0, Genes.Count - 1);
            for (int i = 0; i < Genes.Count; i++)
            {
                if (i <= cross)
                {
                    genes1[i] = Genes[i].Clone();
                    genes2[i] = with.Genes[i].Clone();
                }
                else
                {
                    genes1[i] = with.Genes[i].Clone();
                    genes2[i] = Genes[i].Clone();
                }
            }

            return new IMutatable[][] { genes1, genes2 };
        }

        #endregion
    }
}
