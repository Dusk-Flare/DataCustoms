package lanta.math;

import lanta.math.expressions.BivariateExpression;
import lanta.math.expressions.Expression;

import java.util.ArrayList;
import java.util.Map;
import java.util.function.Function;

public final class Methods {

    public static  <T> T[] convertArray(Object[] original, Function<Object, T> converter){
        if (original == null || original.length == 0) throw new IllegalArgumentException("Array data cannot be null or empty.");
        int rows = original.length;
        T[] copy = newArray(rows);
        for (int i = 0; i < rows; i++) copy[i] = converter.apply(original[i]);
        return copy;
    }

    public static <T> T[] newArray(int rows){
        @SuppressWarnings("unchecked")
        T[] array = (T[]) new Object[rows];
        return array;
    }
    public <T extends Number> Expression<T> Sum(Expression<T> expression, Function<Double, T> converter, int from, int to){
        Expression<T> result = expression;
        for (int i = from; i < to; i++) {
            Expression<?> temp = result;
            result = (x) -> converter.apply(temp.eval(x).doubleValue() + expression.eval(x).doubleValue());
        }
        return result;
    }

    public static <T extends Number> T Sum(Number[] values, Expression<T> expression, Function<Number, T> converter){
        double sum = 0;
        for(Number value : values) sum += expression.eval(value).doubleValue();
        return converter.apply(sum);
    }

    public static <T extends Number> T Sum(Map.Entry<T, T>[] values, BivariateExpression<T> expression, Function<Number, T> converter){
        double sum = 0;
        for(Map.Entry<T, T> value : values) sum += expression.eval(value.getKey(), value.getValue()).doubleValue();
        return converter.apply(sum);
    }
}
