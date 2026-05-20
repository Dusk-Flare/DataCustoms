package lanta.math.expressions;

@FunctionalInterface
public  interface BivariateExpression<T extends Number>{
    T eval(Number x, Number y);
}