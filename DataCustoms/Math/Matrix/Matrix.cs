using System.Numerics;

namespace DataCustoms.Math.Matrix
{
    public class Matrix<T> where T : INumber<T>
    {
        public int Rows;
        public int Columns;
        private readonly T[,] _matrix;
        private bool _dirty = true;
        public MatrixType Type { get; private set; }
        private MatrixValueType _valueType = MatrixValueType.None;
        public MatrixValueType ValueType
        {
            get
            {
                if (!_dirty) return _valueType;
                _dirty = false;
                if(IsEmpty()) return _valueType = MatrixValueType.Null;
                if (Diagonal())
                {
                    if (Scalar())
                    {
                        if (_matrix[0, 0] == T.One) return _valueType = MatrixValueType.Identity;
                        else return _valueType = MatrixValueType.Scalar;
                    }
                    else return _valueType = MatrixValueType.Diagonal;
                }
                if (Diagonal(true))
                {
                    if (Scalar(true))
                    {
                        if (_matrix[Rows - 1, 0] == T.One) return _valueType = MatrixValueType.AntiIdentity;
                        else return _valueType = MatrixValueType.AntiScalar;
                    }
                    else return _valueType = MatrixValueType.AntiDiagonal;
                }

                if (Symetric()) return _valueType = MatrixValueType.Symetric;
                if (Triangular()) return _valueType = MatrixValueType.Triangular;
                if (UpperTriangular()) return _valueType = MatrixValueType.UpperTriangular;
                if (Symetric(true)) return _valueType = MatrixValueType.AntiSymetric;
                if (Triangular(true)) return _valueType = MatrixValueType.AntiTriangular;
                if (UpperTriangular(true)) return _valueType = MatrixValueType.AntiUpperTriangular;
                
                return _valueType = MatrixValueType.None;
            }
        }
        private bool IsEmpty()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (_matrix[i, j] != default) return false;
                }
            }
            return true;
        }
        private bool Scalar(bool antiDiagonal = false)
        {
            if (Type != MatrixType.Square) return false;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    int x = antiDiagonal ? (Rows - 1): 0;
                    bool onDiagonal = antiDiagonal ? i + j == Rows - 1 : i == j;
                    if (!onDiagonal && _matrix[i, j] != default) return false;
                    else if (onDiagonal && _matrix[i, j] != _matrix[x, 0]) return false;
                }
            }
            return true;
        }
        private bool Symetric(bool antiDiagonal = false)
        {
            if (Type != MatrixType.Square) return false;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    int x = antiDiagonal ? (Rows - 1) - j : j;
                    int y = antiDiagonal ? (Columns - 1) - i : i;
                    if (_matrix[i, j] != _matrix[x, y]) return false;
                }
            }
            return true;
        }

        private bool Diagonal(bool antiDiagonal = false)
        {
            if (Type != MatrixType.Square) return false;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    bool onDiagonal = antiDiagonal ? i + j == Rows - 1 : i == j;
                    if (!onDiagonal && _matrix[i, j] != default) return false;
                }
            }
            return true;
        }

        private bool Triangular(bool antiDiagonal = false)
        {
            if (Type != MatrixType.Square) return false;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    bool aboveDiagonal = antiDiagonal ? i + j < Rows - 1 : j > i;
                    if (aboveDiagonal && _matrix[i, j] != default) return false;
                }
            }
            return true;
        }

        private bool UpperTriangular(bool antiDiagonal = false)
        {
            if (Type != MatrixType.Square) return false;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    bool belowDiagonal = antiDiagonal ? i + j > Rows - 1 : j < i;
                    if (belowDiagonal && _matrix[i, j] != default) return false;
                }
            }
            return true;
        }

        public Matrix(int x, int y)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(x, 1, $"{nameof(x)} is lower than 1");
            ArgumentOutOfRangeException.ThrowIfLessThan(y, 1, $"{nameof(y)} is lower than 1");
            Rows = x;
            Columns = y;
            if (x == y)
            {
                if (x == 1) Type = MatrixType.Single;
                else Type = MatrixType.Square;
            }
            else if (x == 1) Type = MatrixType.Column;
            else if (y == 1) Type = MatrixType.Row;
            else Type = MatrixType.Rectangle;
            _matrix = new T[x, y];
        }
    }
}
