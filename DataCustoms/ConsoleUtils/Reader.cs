using DataCustoms.Logging;
using DataCustoms.Math;
using System.Numerics;

namespace DataCustoms.ConsoleUtils
{
    public static class Reader
    {
        private static readonly Mercury _logger = LogWriter.Logger;

        public static string ReadLine(string prompt)
        {
            if (prompt != null) Console.WriteLine(prompt);
            string line = Console.ReadLine();
            _logger.LogInfo("User Input: ", line);
            return line;
        }

        public static bool TryReadNumber<T>(string prompt, out T value) where T : INumber<T>
        {
            string input = ReadLine(prompt);
            return Parser.TryParseNumber(input, out value);
        }

        public static T ReadNumber<T>(string prompt) where T : INumber<T>
        {
            string input = ReadLine(prompt);
            if (!Parser.TryParseNumber(input, out T value)) throw new FormatException($"Could not parse {input}");
            return value;
        }

        public static T ReadNumber<T>(string prompt, T defaultValue) where T : INumber<T>
        {
            T value;
            try
            {
                value = ReadNumber<T>(prompt);
            } 
            catch (FormatException)
            {
                value = defaultValue;
            }
            return value;
        }
    }
}
