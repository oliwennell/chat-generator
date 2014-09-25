using System;
using System.Collections.Generic;
using System.Linq;

namespace GetSlackMessages
{
    public class Phrase
    {
        private string actualPhrase;

        public static implicit operator string(Phrase p)
        {
            return p.actualPhrase;
        }

        private Phrase(string phrase)
        {
            actualPhrase = phrase;
        }

        public static Phrase GenerateFrom(MarkovChain chain)
        {
            var nodes = new List<Node>();

            var random = new Random();
            var firstNode = chain.Nodes.ElementAt(random.Next(chain.Nodes.Count() + 1));
            nodes.Add(firstNode);

            var node = firstNode;
            for (int wordIndex = 1; wordIndex < 9; wordIndex++)
            {
                node = node.PickTransition(random);
                nodes.Add(node);
            }

            var result = String.Join(" ", nodes.Select(n => n.Word));
            if (!result.EndsWith("!") && !result.EndsWith("?")) result = result + ".";
            return new Phrase(result); 
        }
    }
}
