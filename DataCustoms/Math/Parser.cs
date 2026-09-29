using System.Globalization;
using System.Numerics;

namespace DataCustoms.Math
{    
    public static partial class Parser
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
    }
}
