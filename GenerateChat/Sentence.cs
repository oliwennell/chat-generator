using System;
using System.Collections.Generic;
using System.Linq;

namespace GetSlackMessages
{
    public class Sentence
    {
        private string actualSentence;

        public static implicit operator string(Sentence p)
        {
            return p.actualSentence;
        }

        private Sentence(string sentence)
        {
            actualSentence = sentence;
        }

        public static Sentence GenerateFrom(MarkovChain chain)
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
            if (SentenceRequiresFullStop(result)) result = result + ".";
            return new Sentence(result); 
        }

        private static bool SentenceRequiresFullStop(string result)
        {
            return !result.EndsWith("!") 
                && !result.EndsWith("?") 
                && !result.EndsWith(".");
        }
    }
}
