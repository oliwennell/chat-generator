using System;
using System.Collections.Generic;
using System.Linq;

namespace GetSlackMessages
{
    public class MarkovChain
    {
        private Func<string, bool> filterFunc = (s) => { return true; };
        private Func<string, string> conversionFunc = (s) => { return s; };
        private List<string> inputPhrases;

        private Dictionary<string, Node> nodesByWord = new Dictionary<string, Node>();
        public IEnumerable<Node> Nodes
        {
            get { return nodesByWord.Values; }
        }

        public static MarkovChain FromPhrases(IEnumerable<string> phrases)
        {
            var result = new MarkovChain();
            result.inputPhrases = phrases.ToList();
            return result;
        }

        public MarkovChain WithFilter(Func<string,bool> filter)
        {
            filterFunc = filter;
            return this;
        }

        public MarkovChain WithConversion(Func<string, string> conversion)
        {
            conversionFunc = conversion;
            return this;
        }

        public MarkovChain Build()
        {
            var words = inputPhrases.SelectMany(p => p.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(w => filterFunc(w))
                .Select(w => conversionFunc(w));

            GenerateFrom(words);
            return this;
        }

        private void GenerateFrom(IEnumerable<string> words)
        {
            string prevLowerWord = null;
            foreach (var word in words)
            {
                var lowerWord = word.ToLowerInvariant();
                if (!nodesByWord.ContainsKey(lowerWord)) nodesByWord.Add(lowerWord, new Node(lowerWord));

                if (prevLowerWord != null)
                {
                    var prevNode = nodesByWord[prevLowerWord];
                    prevNode.LinkTo(nodesByWord[lowerWord]);
                }

                prevLowerWord = lowerWord;
            }
        }
    }
}
