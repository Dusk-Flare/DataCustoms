using DataCustoms;
using DataCustoms.Math;
public class Program : DataMain
{
    public static void Main(string[] args)
    {
        string infix = "2^2+3(2x+3)^2";
        string postfix = Parser.ToPostfix(infix);
        MonoVariate<float> func = Parser.Compile(postfix);
        Logger.LogInfo(infix, postfix, func(2));
    }
}
