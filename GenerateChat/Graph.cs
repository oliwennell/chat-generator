using System;
using System.Collections.Generic;
using System.Linq;

namespace GenerateChat
{
    public class Graph
    {
        private Func<string, bool> filterFunc = (s) => { return true; };
        private Func<string, string> conversionFunc = (s) => { return s; };
        private List<string> inputPhrases;

        private Dictionary<string, Node> nodesByWord = new Dictionary<string, Node>();
        public IEnumerable<Node> Nodes
        {
            get { return nodesByWord.Values; }
        }

        public static Graph FromPhrases(IEnumerable<string> phrases)
        {
            var result = new Graph();
            result.inputPhrases = phrases.ToList();
            return result;
        }

        public Graph WithFilter(Func<string,bool> filter)
        {
            filterFunc = filter;
            return this;
        }

        public Graph WithConversion(Func<string, string> conversion)
        {
            conversionFunc = conversion;
            return this;
        }

        public Graph Build()
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
