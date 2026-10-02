namespace DataCustoms.Math.Matrix
{
    public enum MatrixType
    {
        None,
        Single,
        Square,
        Rectangle,
        Row,
        Column
    }
    public enum MatrixValueType
    {
        None = 0,
        Null = 1 << 0,
        Diagonal = 1 << 1,
        AntiDiagonal = 1 << 2,
        Identity = 1 << 3,
        AntiIdentity = 1 << 4,
        Scalar = 1 << 5,
        AntiScalar = 1 << 6,
        Symetric = 1 << 7,
        AntiSymetric = 1 << 8,
        Triangular = 1 << 9,
        UpperTriangular = 1 << 10,
        AntiTriangular = 1 << 11,
        AntiUpperTriangular = 1 << 12,

        Anti = AntiDiagonal | AntiScalar | AntiIdentity | AntiSymetric | AntiTriangular | AntiUpperTriangular,
    }
}
