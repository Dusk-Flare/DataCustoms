package lanta.math.expressions;

@FunctionalInterface
public interface Expression<T extends Number> {
    T eval(Number x);
}