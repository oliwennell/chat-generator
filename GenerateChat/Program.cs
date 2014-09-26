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
            var data1 = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("data.json")).messages as IEnumerable<dynamic>;
            var data2 = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("data2.json")).messages as IEnumerable<dynamic>;

            var messages = (data1.Union(data2))
                .Where(d => d.text != null)
                .Select(d => d.text.Value)
                .Cast<string>();

            var markovChain = MarkovChain.FromPhrases(messages)
                .WithConversion(ConvertWord)
                .WithFilter(ShouldIncludeWord)
                .Build();

            var random = new Random();
            for (int i=0; i<10; ++i)
            {
                int numberOfWordsInSentence = random.Next(7, 30);

                var sentence = new Sentence()
                    .WithChain(markovChain)
                    .WithNumberOfWords(numberOfWordsInSentence)
                    .Build(random);

                Console.WriteLine(sentence);
                Console.WriteLine();
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
            if (!word.Any(Char.IsLetter)) return false;
            if (word.Any(c => c == '<' || c == '>')) return false;

            return true;
        }
    }
}
