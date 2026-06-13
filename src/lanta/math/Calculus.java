package lanta.math;

import lanta.math.expressions.BivariateExpression;
import lanta.math.expressions.Expression;

import java.util.function.Function;

public class Calculus {
    private static final double DELTA = 1e-9;
    public static <T extends Number> Expression<T> limit(Expression<T> function, Function<Double, T> converter) {
        return x -> {
            double goal = x.doubleValue();
            double minorValue = function.eval(goal - DELTA).doubleValue();
            double majorValue = function.eval(goal + DELTA).doubleValue();
            if (Math.abs(minorValue - majorValue) < 1e-7) return converter.apply((minorValue + majorValue) / 2);
            return converter.apply(Double.NaN);
        };
    }

    public static <T extends Number> Expression<T> derivative(Expression<T> function, Function<Double, T> converter){
        return (x) -> converter.apply((function.eval(x.doubleValue() + DELTA).doubleValue() - function.eval(x.doubleValue() - DELTA).doubleValue()) / (2 * DELTA));
    }

    public static <T extends Number> Expression<T> integral(Expression<T> function, int delta, Function<Double, T> converter) throws IllegalArgumentException{
        BivariateExpression<T> expression = biIntegral(function, delta, converter);
        return y -> expression.eval(0, y);
    }

    public static <T extends Number> BivariateExpression<T> biIntegral(Expression<T> function, int delta, Function<Double, T> converter) throws IllegalArgumentException {
        if (delta % 2 != 0) throw new IllegalArgumentException("delta must be even");
        return (x, y) -> {
            double a = x.doubleValue();
            double b = y.doubleValue();
            double step = (b - a) / delta;
            double soma = function.eval(a).doubleValue() + function.eval(b).doubleValue();
            for (int i = 1; i < delta; i++) soma +=function.eval(a + i * step).doubleValue() * ((i % 2 == 0) ? 2 : 4);
            return converter.apply((step / 3.0) * soma);
        };
    }
}
