using System;
using System.Text;
using System.Text.RegularExpressions;

namespace normalize_test
{
    public class Program
    {
        private static string Sanitise(string input, string replaceCharacterPattern)
        {
            var output = string.Empty;
            if (!string.IsNullOrWhiteSpace(input))
            {
                string normalizedInput = input.Normalize(NormalizationForm.FormD);
                output = Regex.Replace(normalizedInput, replaceCharacterPattern, string.Empty);
                output = output.Trim();
            }
            return output;
        }

        public static int Main(string[] args)
        {
            var result = 0;
            var source = "Königsberg";
            var expected = "Konigsberg";
            var actual = Sanitise(source, "[^A-Za-z]");
            Console.WriteLine();
            Console.WriteLine($"{source} -> {actual}");

            if (actual != expected)
            {
                Console.WriteLine($"Expected: {expected}");
                Console.WriteLine("TEST FAILED");
                result = 1;
            }
            else
            {
                Console.WriteLine("TEST OK");
            }

            Console.WriteLine();
            return result;
        }
    }
}