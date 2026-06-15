import lanta.math.FunctionForm;
import lanta.math.expressions.Expression;

public class Main {
    public static void main(String[] args) {
        Double[] x = {1., 2., 3., 4., 5., 6., 7.};
        Double[] y = {3.1, 2.2, 1.3, 0.4, -1.5, -2.6, -3.7};
        Expression<Double> form = FunctionForm.findFunction(x, y, Number::doubleValue, 0.000000000000005);
        for (int i = 0; i < 100; i++) System.out.printf("%.1f%n", form.eval(i));
    }
}