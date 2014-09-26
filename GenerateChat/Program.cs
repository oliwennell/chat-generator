using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GetSlackMessages
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("data.json"));
            var messages = (data.messages as IEnumerable<dynamic>)
                .Where(d => d.text != null)
                .Select(d => d.text.Value)
                .Cast<string>();

            var markovChain = MarkovChain.FromPhrases(messages)
                .WithConversion(ConvertWord)
                .WithFilter(ShouldIncludeWord)
                .Build();

            //markovChain.Nodes.ToList()
            //    .ForEach(n => Console.WriteLine("\"{0}\" links to {1}", n.Word, n.Links.Count));

            Console.WriteLine(Sentence.GenerateFrom(markovChain));

            Console.ReadLine();
            return 0;
        }

        private static string ConvertWord(string word)
        {
            if (word.Last() == '.')
                word = new string(word.Take(word.Length - 1).ToArray());

            return word
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("```", "");
        }

        private static bool ShouldIncludeWord(string word)
        {
            if (!word.Any(Char.IsLetter)) return false;
            if (word.Any(c => c == '<' || c == '>')) return false;

            return true;
        }
    }
}
