using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GetSlackMessages
{
    public class Node
    {
        public string Word { get; private set; }

        private Dictionary<Node, int> links = new Dictionary<Node, int>();
        public IReadOnlyDictionary<Node, int> Links { get; private set; }

        private List<KeyValuePair<float, Node>> weightedLinks;

        public Node(string word)
        {
            Word = word;
            Links = new ReadOnlyDictionary<Node, int>(links);
        }

        public Node PickTransition(Random random)
        {
            if (weightedLinks == null) weightedLinks = GenerateWeightedLinks(links);

            var transitionScalar = random.NextDouble();

            KeyValuePair<float,Node> item = weightedLinks[0];
            var index = 0;
            var weight = 0.0f;
            while (weight < transitionScalar)
            {
                weight += item.Key;
                item = weightedLinks[index++];
            }
            return item.Value;
        }

        private static List<KeyValuePair<float, Node>> GenerateWeightedLinks(Dictionary<Node, int> links)
        {
            float sum = links.Values.Sum();
            var result = new List<KeyValuePair<float, Node>>();
            foreach (var link in links)
            {
                var weight = link.Value / sum;
                result.Add(new KeyValuePair<float, Node>(weight, link.Key));
            }
            return result;
        }

        public void LinkTo(Node node)
        {
            if (!links.ContainsKey(node)) links.Add(node, 0);
            links[node]++;
        }
    }
}
