package lanta.math;

import lanta.math.expressions.BivariateExpression;
import lanta.math.expressions.Expression;

import java.util.Map;
import java.util.function.Function;

public final class Methods {
<<<<<<< Updated upstream
    public static <T extends Number> Expression<T> Sum(Expression<T> expression, Function<Number, T> converter, int from, int to){
        Expression<T> expr = converter::apply;
        for (int i = from; i < to; i++) {
            Expression<T> finalExpr = expr;
            expr = (x) -> converter.apply(finalExpr.eval(x).doubleValue() + expression.eval(x).doubleValue());
        }
        return expr;
=======
    public <T extends Number> Expression<T> Sum(Expression<T> expression, Function<Double, T> converter, int from, int to){
        Expression<T> result = expression;
        for (int i = from; i < to; i++) {
            Expression<?> temp = result;
            result = (x) -> converter.apply(temp.eval(x).doubleValue() + expression.eval(x).doubleValue());
        }
        return result;
>>>>>>> Stashed changes
    }

    public static <T extends Number> T Sum(Number[] values, Expression<T> expression, Function<Number, T> converter){
        double sum = 0;
        for(Number value : values) sum += expression.eval(value).doubleValue();
        return converter.apply(sum);
    }

    public static <T extends Number> T Sum(Map.Entry<Number, Number>[] values, BivariateExpression<T> expression, Function<Number, T> converter){
        double sum = 0;
        for(Map.Entry<Number, Number> value : values) sum += expression.eval(value.getKey(), value.getValue()).doubleValue();
        return converter.apply(sum);
    }
}
