namespace DataCustoms.Math.Tokens
{
    public record struct Token(TokenType Type, string Name = "", MonoVariate<float> Value = null);
    [Flags]
    public enum TokenType
    {
        None = 0,
        Number = 1 << 0,
        Variable = 1 << 1,
        Constant = 1 << 2,
        Function = 1 << 3,
        Operator = 1 << 4,
        LParen = 1 << 5,
        RParen = 1 << 6,

        ImplicitMultiplier = Variable | Constant | Function | LParen,
    }
}
