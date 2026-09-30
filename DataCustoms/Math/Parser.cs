using DataCustoms.Math.Tokens;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace DataCustoms.Math
{    
    public static class Parser
    {
        public static bool TryParseNumber<T>(string number, out T value) where T : INumber<T>
        {
            return T.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        public static T ParseNumber<T>(string number) where T : INumber<T>
        {
            if(TryParseNumber(number, out T value)) return value;
            throw new FormatException($"Failed to Parse: '{number}'");
        }

        public static string ToPostfix(string infix)
        {
            StringBuilder output = new();
            bool canBeUnary = true;
            Stack<Token> operators = new();
            List<Token> tokens = Tokenizer.Parse(infix);
            Token mult = new(TokenType.Operator, "*");
            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];
                switch (token.Type)
                {
                    case TokenType.Number or TokenType.Constant or TokenType.Variable:
                        output.Append(token.Name).Append(' ');
                        canBeUnary = false;
                        if((tokens.Count > i + 1) && TokenType.ImplicitMultiplier.HasFlag(tokens[i+1].Type))
                        {
                            PushOperator(mult, operators, output);
                        }
                        continue;
                    case TokenType.LParen:
                        operators.Push(token);
                        canBeUnary = true;
                        continue; 
                    case TokenType.RParen:
                        while (operators.Count > 0)
                        {
                            Token temp = operators.Pop();
                            if (temp.Type == TokenType.LParen) break;
                            output.Append(temp.Name).Append(' ');
                        }
                        canBeUnary = false;
                        continue;
                    case TokenType.Operator or TokenType.Function:
                        if (!canBeUnary || !Tokenizer.TryGetUnary(token, out Token? unary)) PushOperator(token, operators, output);
                        else PushOperator(unary.Value, operators, output);
                        canBeUnary = true;
                        continue;
                }
            }
            while (operators.Count > 0)
            {
                Token token = operators.Pop();
                output.Append(token.Name).Append(' ');
            }

            return output.ToString().Trim();
        }

        public static MonoVariate<float> CompileInfix(string infix)
        {
            string postfix = ToPostfix(infix);
            return Compile(postfix);
        }

        public static MonoVariate<float> Compile(string postfix)
        {
            List<Token> tokens = Tokenizer.Parse(postfix);
            Stack<MonoVariate<float>> stack = new();
            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number or TokenType.Constant or TokenType.Variable:
                        stack.Push(token.Value);
                        continue;
                    case TokenType.Operator:
                        if(stack.Count < 2) throw new InvalidOperationException($"Insuficient arguments for operator: {token.Name}");
                        MonoVariate<float> r = stack.Pop();
                        MonoVariate<float> l = stack.Pop();
                        stack.Push(Tokenizer.ApplyOperator(token, l, r));
                        continue;
                    case TokenType.Function:
                        if(stack.Count < 1) throw new InvalidOperationException($"Insuficient arguments for function: {token.Name}");
                        MonoVariate<float> arg = stack.Pop();
                        stack.Push(Tokenizer.ApplyFunction(token, arg));
                        continue;

                }
            }
            if(stack.Count > 1) throw new InvalidOperationException($"Not all values were resolved: {stack}");
            return stack.Pop();
        }

        private static void PushOperator(Token opr, Stack<Token> operators, StringBuilder output)
        {
            while (operators.Count > 0)
            {
                Token topOp = operators.Peek();
                if (topOp.Type == TokenType.LParen) break;
                if (!Tokenizer.TryGetPriority(topOp, out int topPrec)) topPrec = 0;
                Tokenizer.TryGetPriority(opr, out int currPrec);

                if (topPrec > currPrec || (topPrec == currPrec && !Tokenizer.IsRightAssociate(opr))) 
                {
                    output.Append(operators.Pop().Name).Append(' ');
                } 
                else break;
            }
            operators.Push(opr);
        }
    }
}
