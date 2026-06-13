package lanta.math;

import lanta.math.expressions.Expression;

import java.util.Arrays;
import java.util.Map;
import java.util.function.Function;

public final class FunctionForm {
    public static <T extends Number> Expression<T> findFunction(T[] xValues, T[] yValues, Function<Number, T> converter) {
        T sumX = Methods.Sum(xValues, converter::apply, converter);
        @SuppressWarnings("unchecked")
        T sumX2 = Methods.Sum((T[]) Arrays.stream(xValues).map(x -> converter.apply(Math.pow(x.doubleValue(), 2))).toArray(), converter::apply, converter);
        // Matrix with the sums
        //and result of the sums
        // resulting in the coeficients.
        for (int i = 0; i < xValues.length; i++) {
            Map.entry(xValues[i], yValues[i]);
        }

        T sumXY = Methods.Sum(, (x, y) -> converter.apply(x.doubleValue()* y.doubleValue()), converter)
    }
}
