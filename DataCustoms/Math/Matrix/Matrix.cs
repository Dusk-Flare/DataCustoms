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

        public T this[int x, int y]
        {
            get => GetValue(x, y);
            set => SetValue(x, y, value);
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

        public Matrix(Matrix<T> orig)
        {
            Rows = orig.Rows;
            Columns = orig.Columns;
            if (Rows == Columns)
            {
                if (Rows == 1) Type = MatrixType.Single;
                else Type = MatrixType.Square;
            }
            else if (Rows == 1) Type = MatrixType.Column;
            else if (Columns == 1) Type = MatrixType.Row;
            else Type = MatrixType.Rectangle;

            _matrix = new T[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] = orig[i, j];
                }
            }
        }

        public Matrix(T[,] orig)
        {
            Rows = orig.GetLength(0);
            Columns = orig.GetLength(1);
            if (Rows == Columns)
            {
                if (Rows == 1) Type = MatrixType.Single;
                else Type = MatrixType.Square;
            }
            else if (Rows == 1) Type = MatrixType.Column;
            else if (Columns == 1) Type = MatrixType.Row;
            else Type = MatrixType.Rectangle;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] = orig[i, j];
                }
            }
        }

        public void SetZero()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] = T.Zero;
                }
            }
        }

        public void SetDefault()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] = default;
                }
            }
        }

        public bool TryGetValue(int x, int y, out T result)
        {
            result = default;
            if(x < 0 || x >= Rows || y < 0 || y >= Columns) return false;
            result = _matrix[x, y];
            return true;
        }

        public T GetValue(int x, int y) => _matrix[x, y];

        public void SetValue(int x, int y, T value = default)
        {
            _matrix[x, y] = value;
            _dirty = true;
        }

        public void Add(Matrix<T> matrix)
        {
            if (Rows != matrix.Rows || Columns != matrix.Columns) throw new ArgumentException("Both matrices must be of same size");
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] += matrix[i, j];
                }
            }
        }

        public void Subtract(Matrix<T> matrix)
        {
            if (Rows != matrix.Rows || Columns != matrix.Columns) throw new ArgumentException("Both matrices must be of same size");
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] -= matrix[i, j];
                }
            }
        }

        public void Scale(T value)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] *= value;
                }
            }
        }

        public void Multiply(Matrix<T> matrix)
        {
            if (Columns != matrix.Rows) throw new ArgumentException($"Matrix multiplication requires the left operand's Columns '{Columns}' to equal right operand's Rows '{matrix.Rows}'.");
            Matrix<T> temp = new(this);
            SetZero();
            for (int k = 0; k < Rows; k++)
            {
                for (int i = 0; i < Columns; i++)
                {
                    for (int j = 0; j < matrix.Rows; j++)
                    {
                        this[i, j] += temp[i, k] * matrix[k, j];
                    }
                }
            }
        }

        public void Transpose()
        {
            Matrix<T> temp = new(this);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] += temp[j, i];
                }
            }
        }

        public static Matrix<T> Transpose(Matrix<T> matrix)
        {
            Matrix<T> result = new(matrix);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    result[i, j] += matrix[j, i];
                }
            }
            return result;
        }

        public static Matrix<T> operator +(Matrix<T> left, Matrix<T> right)
        {
            if (left.Rows != right.Rows || left.Columns != right.Columns) throw new ArgumentException("Both matrices must be of same size");
            Matrix<T> result = new(left);
            for (int i = 0; i < result.Rows; i++)
            {
                for (int j = 0; j < result.Columns; j++)
                {
                    result[i, j] += right[i, j];
                }
            }
            return result;
        }
        public static Matrix<T> operator -(Matrix<T> left, Matrix<T> right)
        {
            if (left.Rows != right.Rows || left.Columns != right.Columns) throw new ArgumentException("Both matrices must be of same size");
            Matrix<T> result = new(left);
            for (int i = 0; i < result.Rows; i++)
            {
                for (int j = 0; j < result.Columns; j++)
                {
                    result[i, j] -= right[i, j];
                }
            }
            return result;
        }
        public static Matrix<T> operator *(Matrix<T> matrix, T value)
        {
            Matrix<T> result = new(matrix);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    result[i, j] *= value;
                }
            }
            return result;
        }
        public static Matrix<T> operator *(Matrix<T> left, Matrix<T> right)
        {
            if (left.Columns != right.Rows) throw new ArgumentException($"Matrix multiplication requires the left operand's Columns '{left.Columns}' to equal right operand's Rows '{right.Rows}'.");
            Matrix<T> temp = new(left.Rows, right.Columns);
            for (int k = 0; k < left.Rows; k++)
            {
                for (int i = 0; i < left.Columns; i++)
                {
                    for (int j = 0; j < right.Rows; j++)
                    {
                        temp[i, j] += left[i, k] * right[k, j];
                    }
                }
            }
            return temp;
        }
    }
}
