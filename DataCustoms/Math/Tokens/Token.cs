namespace DataCustoms.Math.Tokens
{
    public record struct Token(TokenType Type, string Name = "", float Value = 0f);
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
}
