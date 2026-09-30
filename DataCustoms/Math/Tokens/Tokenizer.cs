using System.Text.RegularExpressions;

namespace DataCustoms.Math.Tokens
{
    public static class Tokenizer
    {
        private static Regex TokenRegex { get; } = new(@"\d+(?:\.\d+)?(?:[eE][+-]?\d+)?|\.\d+(?:[eE][+-]?\d+)?|[a-zA-Z_]+|[()+\-*/^~°]|\S");
        private static Dictionary<string, string> _unaryOperators = new()
        {
            {"-", "~"}, 
            {"°", "°"}
        };
        private static Dictionary<string, BiVariate<float>> _operators = new()
        {
            { "+", (x, y) => x + y  },
            { "-", (x, y) => x - y  },
            { "*", (x, y) => x * y  },
            { "/", (x, y) => x / y  },
            { "^", MathF.Pow }
        };
        private static Dictionary<string, MonoVariate<float>> _functions = new()
        {
            { "sin", MathF.Sin },
            { "cos", MathF.Cos },
            { "tan", MathF.Tan },
            { "floor", MathF.Floor },
            { "ceil", MathF.Ceiling },
            { "°", x => x * (MathF.PI / 180) },
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
            { "~", 5 }, { "°", 5 }
        };

        public static TokenType TypeOf(string token)
        {
            if (_operators.ContainsKey(token)) return TokenType.Operator;
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
                float numberValue;

                TokenType type = TypeOf(rawToken);
                if (type == TokenType.None) continue;
                if (type == TokenType.Number) numberValue = Parser.ParseNumber<float>(rawToken);
                else numberValue = 0f;
                Token token = type switch
                {
                    TokenType.Number => new(type, rawToken, _ => numberValue),
                    TokenType.Variable => new(type, rawToken, x => x),
                    TokenType.Constant => new(type, rawToken, _ => _constants[rawToken]),
                    TokenType.Function => new(type, rawToken, _functions[rawToken]),
                    TokenType.Operator => new(type, rawToken),
                    TokenType.LParen => new(type, rawToken),
                    TokenType.RParen => new(type, rawToken),
                    _ => throw new FormatException($"Unknown token: '{rawToken}'")
                };
                stack.Add(token);
            }
            return stack;
        }

        public static MonoVariate<float> ApplyOperator(Token opr, MonoVariate<float> l, MonoVariate<float> r)
        {
            if(opr.Type != TokenType.Operator) return null;
            BiVariate<float> operation = _operators[opr.Name];
            return x => operation(l(x), r(x));
        }

        public static MonoVariate<float> ApplyFunction(Token func, MonoVariate<float> arg)
        {
            if (func.Type != TokenType.Function) return null;
            MonoVariate<float> operation = func.Value;
            return x => operation(arg(x));
        }

        public static bool TryGetPriority(Token token, out int priority)
        {
            string name = token.Type == TokenType.Function ? "function" : token.Name;
            return _priority.TryGetValue(name, out priority);
        }
        public static bool IsRightAssociate(Token token) => _rightAssociates.Contains(token.Name);
        public static bool ImplMult(Token token) => token.Type.HasFlag(TokenType.ImplicitMultiplier);
        public static bool TryGetUnary(Token token, out Token? unary)
        {
            unary = null;
            if (!_unaryOperators.TryGetValue(token.Name, out string unaryName)) return false;
            unary = new(TokenType.Function, unaryName, _functions[unaryName]);
            return true;
        }
    }
}
