using System.Text.RegularExpressions;

namespace DataCustoms.Math.Tokens
{
    public static class Tokenizer
    {
        private static Regex TokenRegex { get; } = new(@"\d+(?:\.\d+)?(?:[eE][+-]?\d+)?|\.\d+(?:[eE][+-]?\d+)?|[a-zA-Z_]+|[()+\-*/^~°]|\S");
        private static List<string> _operators = ["+", "-", "*", "/", "^"];
        private static Dictionary<string, string> _unaryOperators = new()
        {
            {"-", "~"}, 
            {"°", "degrees"}
        };
        private static Dictionary<string, MonoVariate<float>> _functions = new()
        {
            { "sin", MathF.Sin },
            { "cos", MathF.Cos },
            { "tan", MathF.Tan },
            { "floor", MathF.Floor },
            { "ceil", MathF.Ceiling },
            { "degrees", x => x * (MathF.PI / 180) },
            { "~", x => -x }
        };
        private static Dictionary<string, float> _constants = new()
        {
            { "e", MathF.E },
            { "pi", MathF.PI },
            { "tau", MathF.Tau }
        };
        private static HashSet<string> _rightAssociates = ["^", .._unaryOperators.Values, .. _functions.Keys];
        private static Dictionary<string, int> _priority = new()
        {
            { "+", 1 }, { "-", 1 },
            { "*", 2 }, { "/", 2 },
            { "^", 3 },
            { "function", 4 },
            { "~", 5 }, { "degrees", 5 }
        };
        private static HashSet<string> _implicits =
        [
            "x", "(", ")", .._constants.Keys, .._functions.Keys
        ];

        public static TokenType TypeOf(string token)
        {
            if (_operators.Contains(token) || _unaryOperators.ContainsKey(token)) return TokenType.Operator;
            if (_functions.ContainsKey(token)) return TokenType.Function;
            if (_constants.ContainsKey(token)) return TokenType.Constant;
            if (token == "x") return TokenType.Variable;
            if (token == "(") return TokenType.LParen;
            if (token == ")") return TokenType.RParen;
            if (Parser.TryParseNumber(token, out float _)) return TokenType.Number;
            return TokenType.None;
        }

        public static List<Token> Parse(string func)
        {
            List<Token> stack = [];
            foreach (string rawToken in TokenRegex.Matches(func).Select(m => m.Value).Select(m => m.ToLower()))
            {
                TokenType type = TypeOf(rawToken);
                if (type == TokenType.None) continue;
                Token token = type switch
                {
                    TokenType.Number => new(type, rawToken, Parser.ParseNumber<float>(rawToken)),
                    TokenType.Variable => new(type, rawToken),
                    TokenType.Constant => new(type, rawToken, _constants[rawToken]),
                    TokenType.Function => new(type, rawToken),
                    TokenType.Operator => new(type, rawToken),
                    TokenType.LParen => new(type, rawToken),
                    TokenType.RParen => new(type, rawToken),
                    _ => throw new FormatException($"Unknown token: '{rawToken}'")
                };
                stack.Add(token);
            }
            return stack;
        }
    }
}
