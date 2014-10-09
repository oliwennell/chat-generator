using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GenerateChat
{
    public class Program
    {
        private const int numSentencesToGenerate = 20;

        public static int Main(string[] args)
        {
            //var data1 = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("data.json")).messages as IEnumerable<dynamic>;
            //var data2 = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("data2.json")).messages as IEnumerable<dynamic>;
            //var phrases = (data1.Union(data2))
            //    .Where(d => d.text != null)
            //    .Select(d => d.text.Value)
            //    .Cast<string>();

            var phrases = Directory.GetFiles(Directory.GetCurrentDirectory(), "input*.txt")
                .SelectMany(f => File.ReadAllLines(f));

            var graph = Graph.FromPhrases(phrases)
                .WithConversion(ConvertWord)
                .WithFilter(ShouldIncludeWord)
                .Build();

            var random = new Random();
            for (int i=0; i<numSentencesToGenerate; ++i)
            {
                int numberOfWordsInSentence = random.Next(3, 15);

                var sentence = new Sentence()
                    .WithChain(graph)
                    .WithNumberOfWords(numberOfWordsInSentence)
                    .Build(random);

                Console.WriteLine("");
                Console.WriteLine(sentence);
            }

            Console.ReadLine();
            return 0;
        }

        private static string ConvertWord(string word)
        {
            if (word.Last() == '.') word = new string(word.Take(word.Length - 1).ToArray());

            return word
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("```", "");
        }

        private static bool ShouldIncludeWord(string word)
        {
            if (word.All(c => Char.IsUpper(c))) return false; // Probably copyright notice etc.
            if (!word.Any(Char.IsLetter)) return false;
            if (word.Any(c => c == '<' || c == '>')) return false;

            return true;
        }
    }
}
