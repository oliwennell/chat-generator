using System;
using System.Collections.Generic;
using System.Linq;

namespace GenerateChat
{
    public class Sentence
    {
        private string actualSentence;
        private int numberOfWords;
        private MarkovChain chain;

        public static implicit operator string(Sentence p)
        {
            return p.actualSentence;
        }

        public Sentence() {}
        
        private Sentence(string sentence)
        {
            actualSentence = sentence;
        }

        public Sentence WithNumberOfWords(int numberOfWords)
        {
            this.numberOfWords = numberOfWords;
            return this;
        }

        public Sentence WithChain(MarkovChain chain)
        {
            this.chain = chain;
            return this;
        }

        public Sentence Build(Random random)
        {
            var nodes = new List<Node>();

            var firstNode = chain.Nodes.ElementAt(random.Next(0, chain.Nodes.Count()-1));
            nodes.Add(firstNode);

            var node = firstNode;
            for (int wordIndex = 1; wordIndex < numberOfWords; wordIndex++)
            {
                node = node.PickTransition(random);
                nodes.Add(node);
            }

            var result = String.Join(" ", nodes.Select(n => n.Word));
            result = EnsureSentenceCompleted(result);
            result = EnsureFirstWordCapitalised(result);
            return new Sentence(result); 
        }

        private static string EnsureFirstWordCapitalised(string result)
        {
            if (!Char.IsLetter(result.First()))
                return result;

            return Char.ToUpper(result.First()) + result.Substring(1);
        }

        private static string EnsureSentenceCompleted(string result)
        {
            if (result.EndsWith("!") 
                || result.EndsWith("?") 
                || result.EndsWith("."))
            {
                return result;
            }
            return result + '.';
        }
    }
}
