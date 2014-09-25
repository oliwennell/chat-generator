using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GetSlackMessages
{
    public class Node
    {
        public string Word { get; private set; }

        private Dictionary<Node, int> links = new Dictionary<Node, int>();
        public IReadOnlyDictionary<Node, int> Links { get; private set; }

        public Node(string word)
        {
            Word = word;
            Links = new ReadOnlyDictionary<Node, int>(links);
        }

        public Node PickTransition(Random random)
        {
            throw new NotImplementedException();
        }

        public void LinkTo(Node node)
        {
            if (!links.ContainsKey(node)) links.Add(node, 0);
            links[node]++;
        }
    }
}
