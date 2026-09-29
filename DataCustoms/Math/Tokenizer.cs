using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;

namespace DataCustoms.Math
{
    public static class Tokenizer
    {
        public static Regex TokenRegex { get; } = new(@"\d+(?:\.\d+)?(?:[eE][+-]?\d+)?|\.\d+(?:[eE][+-]?\d+)?|[a-zA-Z_]+|[()+\-*/^~°]|\S");
        public static List<string> Operators = ["+", "-", "*", "/", "^"];
        public static List<string> UnaryOperators = ["~", "°"];
        public static Dictionary<string, MonoVariate<float>> Functions = new()
        {
            { "sin", MathF.Sin },
            { "cos", MathF.Cos },
            { "tan", MathF.Tan },
            { "floor", MathF.Floor },
            { "ceil", MathF.Ceiling },
            { "degrees", x => x * (MathF.PI / 180) }
        };
        public static Dictionary<string, float> Constants = new()
        {
            { "e", MathF.E },
            { "pi", MathF.PI },
            { "tau", MathF.Tau }
        };
        public static List<string> Priority = [.. Operators, "function", .. UnaryOperators];
        private static HashSet<string> Implicits =
        [
            "x", "(", ")", ..Constants.Keys, ..Functions.Keys
        ];

        public record struct Token(TokenType Type, string Name, float Value = 0f);
        public enum TokenType
        {
            None,
            Number,
            Variable,
            Constant,
            Function,
            Operator,
            LParen,
            RParen
        }

        public static TokenType TypeOf(string token)
        {
            if (Operators.Contains(token) || UnaryOperators.Contains(token)) return TokenType.Operator;
            else if (Functions.ContainsKey(token)) return TokenType.Function;
            else if (Constants.ContainsKey(token)) return TokenType.Constant;
            else if (token == "x") return TokenType.Variable;
            else if (token == "(") return TokenType.LParen;
            else if (token == ")") return TokenType.RParen;
            else if (Parser.TryParseNumber<float>(token, out _)) return TokenType.Number;
            else return TokenType.None;
        }

        public static Stack<Token> Parse(string func)
        {
            Stack<Token> stack = [];
            foreach (string rawToken in TokenRegex.Matches(func).Select(m => m.Value).Select(m => m.ToLower()))
            {
                TokenType type = TypeOf(rawToken);
                if (type == TokenType.None) continue;
                Token token = type switch
                {
                    TokenType.Number => new(type, rawToken, Parser.ParseNumber<float>(rawToken)),
                    TokenType.Variable => new(type, rawToken),
                    TokenType.Constant => new(type, rawToken, Constants[rawToken]),
                    TokenType.Function => new(type, rawToken),
                    TokenType.Operator => new(type, rawToken),
                    TokenType.LParen => new(type, rawToken),
                    TokenType.RParen => new(type, rawToken),
                    _ => throw new FormatException($"Unknown token: '{rawToken}'")
                };
                stack.Push(token);
            }
            return stack;
        }
    }
}
