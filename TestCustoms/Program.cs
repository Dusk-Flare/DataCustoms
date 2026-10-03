using DataCustoms;
using DataCustoms.Math.Matrix;

namespace TestCustoms
{
    public class Program : DataMain
    {
        public static void Main(string[] args)
        {
            Matrix<float> matrix = new(3, 3);
            matrix[2, 0] = 2;
            matrix[1, 1] = 2;
            matrix[0, 2] = 2;
            matrix *= .5f;
            Logger.LogInfo(matrix, matrix.Rows, matrix.Columns, matrix.Type, matrix.ValueType);
        }
    }
}