package lanta.math;

import lanta.math.expressions.BivariateExpression;
import lanta.math.expressions.Expression;

import java.util.Map;
import java.util.function.Function;

public final class Methods {
    public Expression<?> Sum(Expression<?> expression, int from, int to){
        Expression<?> expr;
        for (int i = from; i < to; i++) {
            //expr = ;
        }
    }

    public <T extends Number> T Sum(Number[] values, Expression<T> expression, Function<Double, T> converter){
        double sum = 0;
        for(Number value : values) sum += expression.eval(value).doubleValue();
        return converter.apply(sum);
    }

    public <T extends Number> T Sum(Map.Entry<Number, Number>[] values, BivariateExpression<T> expression, Function<Double, T> converter){
        double sum = 0;
        for(Map.Entry<Number, Number> value : values) sum += expression.eval(value.getKey(), value.getValue()).doubleValue();
        return converter.apply(sum);
    }
}
