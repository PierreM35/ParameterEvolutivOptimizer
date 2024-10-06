using System.Collections.Generic;

namespace ParameterEvolutivOptimizer.Model
{
    public class Status
    {
        public int GenerationNb { get; private set; }
        public List<Response> BestResponses { get; set; }

        public Status(int generationNb, List<Response> bestResponses)
        {
            GenerationNb = generationNb;
            BestResponses = bestResponses;
        }
    }
}
