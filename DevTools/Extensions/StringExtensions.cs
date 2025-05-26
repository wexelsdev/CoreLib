using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace CoreLib.DevTools.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            List<string> words = SplitIntoWords(input);
            
            if (words.Count == 0)
                return string.Empty;

            string camelCase = words[0].ToLowerInvariant();
        
            for (int i = 1; i < words.Count; i++)
            {
                camelCase += CultureInfo.InvariantCulture.TextInfo.ToTitleCase(words[i]);
            }

            return camelCase;
        }
        
        public static string ToUnderScore(this string input)
        {
            List<string> words = SplitIntoWords(input);
            
            return words.Count == 0 ? string.Empty : string.Join("_", words);
        }

        public static List<string> SplitIntoWords(this string input)
        {
            List<string> words = new();
        
            IEnumerable<string> parts = Regex.Split(input, @"[^a-zA-Z0-9]+")
                .Where(part => !string.IsNullOrEmpty(part));

            foreach (string part in parts)
            {
                IEnumerable<string> subWords = Regex.Split(part, @"(?<=[a-z0-9])(?=[A-Z])")
                    .Where(sub => !string.IsNullOrEmpty(sub))
                    .Select(sub => sub.ToLowerInvariant());
            
                words.AddRange(subWords);
            }

            return words;
        }
    }
}