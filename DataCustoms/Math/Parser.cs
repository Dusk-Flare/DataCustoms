using Microsoft.VisualBasic;
using System.Collections;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataCustoms.Math
{
    public static partial class Parser
    {
        private static readonly Dictionary<string, int> _priority = new()
        {
            { "+", 1 }, { "-", 1 },
            { "*", 2 }, { "/", 2 },
            { "^", 3 },
            { "function", 4 },
            { "~", 5 }, {"°", 5 }
        };

        private static readonly Dictionary<string, MonoVariate<float>> _functions = new()
        {
            { "sin", MathF.Sin },
            { "cos", MathF.Cos },
            { "tan", MathF.Tan },
            { "floor", MathF.Floor },
            { "ceil", MathF.Ceiling },
            { "degrees", x => x * (MathF.PI / 180) }
        };

        private static readonly Dictionary<string, float> _constants = new()
        {
            { "e", MathF.E },
            { "pi", MathF.PI },
            { "tau", MathF.Tau }
        };

        private static readonly HashSet<string> _strings =
        [
            "x", "X", "(", .._constants.Keys, .._functions.Keys
        ];

        [GeneratedRegex(@"\d+(?:\.\d+)?(?:[eE][+-]?\d+)?|\.\d+(?:[eE][+-]?\d+)?|[a-zA-Z_]+|[()+\-*/^~°]|\S")]
        private static partial Regex TokenRegex();
        
        public record struct Token(TokenType Type, string Name, float Value);
        public enum TokenType
        {
            Number, 
            Variable, 
            Constant, 
            Function, 
            Operator, 
            LParen, 
            RParen
        }

        public static Stack<Token> TokensOf(string func)
        {
            Stack<Token> stack = [];
            foreach(string rawToken in TokenRegex().Matches(func).Select(m => m.Value))
            {

            }
            return stack;
        }
    }
}
