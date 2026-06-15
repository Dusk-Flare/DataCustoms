package lanta.math;

import lanta.math.expressions.BivariateExpression;
import lanta.math.expressions.Expression;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Map;
import java.util.function.Function;
import java.util.stream.IntStream;

public final class FunctionForm {
    public static <T extends Number> Expression<T> findFunction(T[] xValues, T[] yValues, Function<Number, T> converter, double precision) throws IllegalArgumentException {
        if (xValues.length != yValues.length) throw new IllegalArgumentException("xValues and yValues must have the same length.");
        if (xValues.length < 2) throw new IllegalArgumentException("At least 2 points are required for linear regression.");
        T sumX = Methods.Sum(xValues, converter::apply, converter);
        T sumY = Methods.Sum(yValues, converter::apply, converter);
        T[] X2 = Arrays.stream(xValues).map(x -> converter.apply(Math.pow(x.doubleValue(), 2))).toArray(Matrix::newArray);
        T sumX2 = Methods.Sum(X2, converter::apply, converter);
        T sumXY = IntStream.range(0, xValues.length)
                .mapToObj(i -> Map.entry(xValues[i], yValues[i]))
                .map(e -> converter.apply(e.getKey().doubleValue() * e.getValue().doubleValue()))
                .reduce(converter.apply(0), (a, b) -> converter.apply(a.doubleValue() + b.doubleValue()));

        @SuppressWarnings("unchecked")
        T[][] sumArray = Matrix.newArray(
                Matrix.newArray(Arrays.asList(converter.apply(xValues.length), sumX)),
                Matrix.newArray(Arrays.asList(sumX, sumX2))
        );

        T[] resultArray = Matrix.newArray(Arrays.asList(sumY, sumXY));
        T[] guessArray = Matrix.populateArray(Matrix.newArray(resultArray.length), converter.apply(0));

        Matrix<T> sums = new Matrix<>(sumArray, converter);
        Matrix<T> results = new Matrix<>(resultArray, converter).transpose();
        Matrix<T> guess = new Matrix<>(guessArray, converter).transpose();
        Matrix<T>  coefficients = LinearEquations.gaussSeidel(sums, results, guess, precision);

        int size = Math.max(coefficients.rows, coefficients.columns);
        Expression<T> function = x -> converter.apply(0.0);
        for (int i = 0; i < size; i++) {
            int exponent = i;
            T coefficient = coefficients.get(i);
            Expression<T> finalFunction = function;
            function = x -> converter.apply((coefficient.doubleValue() * Math.pow(x.doubleValue(), exponent)) + finalFunction.eval(x).doubleValue());
        }
        return function;
    }

    public static <T extends Number> BivariateExpression<T> findFunction(T[] xValues, T[] yValues, T[] zValues, Function<Number, T> converter, double precision) throws IllegalArgumentException {
        if (xValues.length != yValues.length || xValues.length != zValues.length) throw new IllegalArgumentException("All input arrays must have the same length.");
        if (xValues.length < 3) throw new IllegalArgumentException("At least 3 points required for bivariate linear regression.");
        int n = xValues.length;
        T zero = converter.apply(0.0);
        T sumX = Methods.Sum(xValues, converter::apply, converter);
        T sumY = Methods.Sum(yValues, converter::apply, converter);
        T sumZ = Methods.Sum(zValues, converter::apply, converter);
        T sumX2 = Methods.Sum(xValues, x -> converter.apply(Math.pow(x.doubleValue(), 2)), converter);
        T sumY2 = Methods.Sum(yValues, y -> converter.apply(Math.pow(y.doubleValue(), 2)), converter);
        T sumXY = IntStream.range(0, n)
                .mapToObj(i -> Map.entry(xValues[i], yValues[i]))
                .map(e -> converter.apply(e.getKey().doubleValue() * e.getValue().doubleValue()))
                .reduce(zero, (a, b) -> converter.apply(a.doubleValue() + b.doubleValue()));
        T sumXZ = IntStream.range(0, n)
                .mapToObj(i -> Map.entry(xValues[i], zValues[i]))
                .map(e -> converter.apply(e.getKey().doubleValue() * e.getValue().doubleValue()))
                .reduce(zero, (a, b) -> converter.apply(a.doubleValue() + b.doubleValue()));
        T sumYZ = IntStream.range(0, n)
                .mapToObj(i -> Map.entry(yValues[i], zValues[i]))
                .map(e -> converter.apply(e.getKey().doubleValue() * e.getValue().doubleValue()))
                .reduce(zero, (a, b) -> converter.apply(a.doubleValue() + b.doubleValue()));

        @SuppressWarnings("unchecked")
        T[][] matrixData = Matrix.newArray(
                Matrix.newArray(Arrays.asList(converter.apply(n), sumX, sumY)),
                Matrix.newArray(Arrays.asList(sumX, sumX2, sumXY)),
                Matrix.newArray(Arrays.asList(sumY, sumXY, sumY2))
        );
        Matrix<T> A = new Matrix<>(matrixData, converter);
        T[] rhsData = Matrix.newArray(Arrays.asList(sumZ, sumXZ, sumYZ));
        Matrix<T> B = new Matrix<>(rhsData, converter).transpose();
        T[] guessData = Matrix.populateArray(Matrix.newArray(3), converter.apply(0.0));
        Matrix<T> guess = new Matrix<>(guessData, converter).transpose();
        Matrix<T> coefficients = LinearEquations.gaussSeidel(A, B, guess, precision);
        T a = coefficients.get(0);
        T b = coefficients.get(1);
        T c = coefficients.get(2);
        return (x, y) -> converter.apply(a.doubleValue() + b.doubleValue() * x.doubleValue() + c.doubleValue() * y.doubleValue());
    }
}
