using ParameterEvolutivOptimizer.Abstractions;
using System.Collections.Generic;

namespace ParameterEvolutivOptimizer.Model
{
    public static class ExtensionMethods
    {
        public static IEnumerable<LinkedListNode<Individual>> Nodes(this LinkedList<Individual> list)
        {
            for (var node = list.First; node != null; node = node.Next)
            {
                yield return node;
            }
        }
    }
}
