package lanta.math;

public class LinearEquations {
    public static <T extends Number> Matrix<T> gaussSeidel(Matrix<T> matrix, Matrix<T> results, Matrix<T> initialValues, double precision) {
        if (!matrix.isSquare()) throw new IllegalArgumentException("Matrix must be square");
        if (matrix.columns != initialValues.columns || initialValues.rows != 1) throw new IllegalArgumentException("Initial values size must match matrix in rows, and only have 1 column");
        if (matrix.columns != results.columns || results.rows != 1) throw new IllegalArgumentException("Results size must match matrix in rows, and only have 1 column");
        int size = matrix.rows;
        Matrix<T> values = new Matrix<>(initialValues);
        while (true) {
            Matrix<T> nextValues = updateValues(matrix, results, values, size);
            boolean isFinished = true;
            for (int i = 0; i < size; i++) {
                if (Math.abs(values.get(i).doubleValue() - nextValues.get(i).doubleValue()) > precision) {
                    isFinished = false;
                    break;
                }
            } if (isFinished) return nextValues;
            values = nextValues;
        }
    }

    private static <T extends Number> Matrix<T> updateValues(Matrix<T> matrix, Matrix<T> results, Matrix<T> values, int size) {
        Matrix<T> newValues = new Matrix<>(values);
        for (int i = 0; i < size; i++) {
            double sum = results.get(i).doubleValue();
            for (int j = 0; j < size; j++) {
                if (i != j) sum -= matrix.get(i, j).doubleValue() * newValues.get(j).doubleValue();
            }
            newValues.put(i, sum / matrix.get(i, i).doubleValue());
        }
        return newValues;
    }
}