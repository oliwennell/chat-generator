using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GenerateChat
{
    public class Node
    {
        public string Word { get; private set; }

        private Dictionary<Node, int> linksByFrequency = new Dictionary<Node, int>();
        private List<KeyValuePair<float, Node>> linksByProbability;

        public Node(string word)
        {
            Word = word;
        }

        public Node PickTransition(Random random)
        {
            if (linksByProbability == null) linksByProbability = GenerateWeightedLinks(linksByFrequency);

            var transitionScalar = random.NextDouble();

            KeyValuePair<float,Node> item = linksByProbability[0];
            var index = 0;
            var weight = 0.0f;
            while (weight < transitionScalar && index < linksByProbability.Count)
            {
                weight += item.Key;
                item = linksByProbability[index++];
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
            if (!linksByFrequency.ContainsKey(node)) linksByFrequency.Add(node, 0);
            linksByFrequency[node]++;
        }
    }
}
