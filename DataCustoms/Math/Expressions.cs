using System.Numerics;

namespace DataCustoms.Math
{
    public static class ExprOps
    {
        public static MonoVariate<T> Add<T>(MonoVariate<T> l, MonoVariate<T> r) where T : INumber<T>
        {
            return x => l(x) + r(x);
        }
        public static MonoVariate<T> Subtract<T>(MonoVariate<T> l, MonoVariate<T> r) where T : INumber<T>
        {
            return x => l(x) - r(x);
        }
        public static MonoVariate<T> Multiply<T>(MonoVariate<T> l, MonoVariate<T> r) where T : INumber<T>
        {
            return x => l(x) * r(x);
        }
        public static MonoVariate<T> Divide<T>(MonoVariate<T> l, MonoVariate<T> r) where T : INumber<T>
        {
            return x => l(x) / r(x);
        }
        public static MonoVariate<T> Negate<T>(MonoVariate<T> l) where T : INumber<T>
        {
            return x => -l(x);
        }
    }
    public delegate T MonoVariate<T>(T X) where T : INumber<T>;
    public delegate T BiVariate<T>(T X, T Y) where T : INumber<T>;
}
