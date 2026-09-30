using System.Numerics;

namespace DataCustoms.Math
{
    public delegate T MonoVariate<T>(T X) where T : INumber<T>;
    public delegate T BiVariate<T>(T X, T Y) where T : INumber<T>;
}
