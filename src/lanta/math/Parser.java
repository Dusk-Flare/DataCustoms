package lanta.math;

import lanta.math.expressions.Expression;

import java.util.*;
import java.util.function.Function;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import static java.lang.Math.*;

public final class Parser {
    private static final Map<String, Integer> PRECEDENCE = Map.of(
            "+", 1, "-", 1,
            "*", 2, "/", 2,
            "^", 3,
            "function", 4,
            "~", 5, "°", 5
    );
    private static final Map<String, Double> CONSTANT_VALUES = Map.of(
            "e", Math.E,
            "pi", Math.PI
    );
    private static final Map<String, Expression<Double>> FUNCTION_VALUES = Map.of(
            "sin", x -> sin(x.doubleValue()),
            "cos", x -> cos(x.doubleValue()),
            "tan", x -> tan(x.doubleValue()),
            "floor", x -> floor(x.doubleValue()),
            "ceil", x -> ceil(x.doubleValue()),
            "toDegrees", x -> toDegrees(x.doubleValue())
    );

    private static final Set<String> CONSTANTS = CONSTANT_VALUES.keySet();
    private static final Set<String> FUNCTIONS = FUNCTION_VALUES.keySet();

    private static final Set<String> IMPLICITS = new HashSet<>(List.of(
            "x", "X", "("
    ));
    static {
        IMPLICITS.addAll(FUNCTIONS);
        IMPLICITS.addAll(CONSTANTS);
    }

    private static final Pattern TOKEN_PATTERN = Pattern.compile(
            "\\d+(?:\\.\\d+)?(?:[eE][+-]?\\d+)?|\\.\\d+(?:[eE][+-]?\\d+)?|[a-zA-Z_]+|[()+\\-*/^~°]|\\S"
    );

    public static <T extends Number> T toNumber(String text, Function<String, T> parser){
        try{
            return toNumber(text, parser, null);
        } catch(NumberFormatException e){
            return null;
        }
    }

    public static <T extends Number> T extractNumber(int number, int initialPlace, int finalPlace, Function<Number, T> parser){
        number /= (int) pow(10, initialPlace);
        return parser.apply(number % pow(10, abs(initialPlace - finalPlace)));
    }

    public static <T extends Number> Matrix<T> toMatrix(T[][] base, Function<Number, T> converter) {
        T[][] matrixArray = Arrays.copyOf(base, 5);
        for (int i = 0; i < matrixArray.length; i++) {
            matrixArray[i] = Arrays.copyOf(matrixArray[i], 5);
        }
        return new Matrix<>(matrixArray, converter);
    }

    public static <T extends Number> T toNumber(String text, Function<String, T> parser, T defaultsTo) throws NumberFormatException {
        text = text.replaceAll("^(-?(\\d+(\\.\\d+)?|\\.\\d+)).*", "$1");
        try {
            return parser.apply(text);
        } catch (NumberFormatException e) {
            if(defaultsTo == null) throw e;
            return defaultsTo;
        }
    }

    public static String toPostfix(String infix) {
        StringBuilder output = new StringBuilder();
        List<String> tokens = tokenize(infix);
        Stack<String> operators = new Stack<>();
        boolean nextCanBeUnary = true;

        for (int i = 0; i < tokens.size(); i++) {
            String token = tokens.get(i);

            if (isNumber(token)) {
                output.append(token).append(' ');
                nextCanBeUnary = false;
                if (hasImplicitMultiplication(tokens, i)) {
                    pushOperator("*", operators, output);
                }
                continue;
            }

            if (isConstant(token)) {
                output.append(token).append(' ');
                nextCanBeUnary = false;
                if (hasImplicitMultiplication(tokens, i)) {
                    pushOperator("*", operators, output);
                }
                continue;
            }

            if (isVariable(token)) {
                output.append("x ");
                nextCanBeUnary = false;
                if (hasImplicitMultiplication(tokens, i)) {
                    pushOperator("*", operators, output);
                }
                continue;
            }

            if (token.equals("(")) {
                operators.push(token);
                nextCanBeUnary = true;
                continue;
            }

            if (token.equals(")")) {
                unwindUntilOpenParenthesis(operators, output);
                nextCanBeUnary = false;
                continue;
            }

            if (isOperator(token) || isFunction(token)) {
                String op = nextCanBeUnary && token.equals("-") ? "~" : token;
                pushOperator(op, operators, output);
                nextCanBeUnary = true;
            }
        }

        while (!operators.isEmpty()) {
            output.append(operators.pop()).append(' ');
        }

        return output.toString().trim();
    }

    public static Expression<Double> buildExpression(String postfix) {
        Stack<Expression<Double>> stack = new Stack<>();

        for (String token : postfix.split("\\s+")) {
            if (token.isEmpty()) continue;

            if (isOperator(token)) {
                if (token.equals("~")) {
                    Expression<Double> arg = stack.pop();
                    stack.push(x -> -arg.eval(x));
                } else if (token.equals("°")) {
                    Expression<Double> arg = stack.pop();
                    stack.push(x -> Math.toRadians(arg.eval(x)));
                } else{
                    Expression<Double> right = stack.pop();
                    Expression<Double> left = stack.pop();
                    stack.push(applyOperator(token, left, right));
                }
                continue;
            }

            if (isFunction(token)) {
                Expression<Double> arg = stack.pop();
                stack.push(applyFunction(token, arg));
                continue;
            }

            if (isVariable(token)) {
                stack.push(Number::doubleValue);
                continue;
            }

            if (isConstant(token)) {
                stack.push(_ -> CONSTANT_VALUES.get(token));
                continue;
            }

            double value = Double.parseDouble(token);
            stack.push(_ -> value);
        }

        return stack.pop();
    }

    private static List<String> tokenize(String expression) {
        List<String> tokens = new ArrayList<>();
        Matcher m = TOKEN_PATTERN.matcher(expression);
        while (m.find()) {
            String token = m.group();
            tokens.add(token);
        }
        return tokens;
    }

    private static boolean hasImplicitMultiplication(List<String> tokens, int index) {
        int next = index + 1;
        return next < tokens.size() && IMPLICITS.contains(tokens.get(next));
    }

    private static void pushOperator(String operator, Stack<String> stack, StringBuilder output) {
        while (!stack.isEmpty() && !stack.peek().equals("(")) {
            String topOp = stack.peek();
            int topPrec = PRECEDENCE.getOrDefault(isFunction(topOp) ? "function" : topOp, 0);
            int currPrec = PRECEDENCE.get(isFunction(operator) ? "function" : operator);

            if (topPrec > currPrec || (topPrec == currPrec && !operator.matches("[~°^]") && !isFunction(operator))) {
                output.append(stack.pop()).append(' ');
            } else break;
        }
        System.out.println(operator);
        stack.push(operator);
    }

    private static void unwindUntilOpenParenthesis(Stack<String> stack, StringBuilder output) {
        while (!stack.isEmpty() && !stack.peek().equals("(")) {
            output.append(stack.pop()).append(' ');
        }
        if (!stack.isEmpty()) stack.pop();
    }

    private static Expression<Double> applyOperator(String operator, Expression<Double> left, Expression<Double> right) {
        return x -> {
            double a = left.eval(x);
            double b = right.eval(x);
            return switch (operator) {
                case "+" -> a + b;
                case "-" -> a - b;
                case "*" -> a * b;
                case "/" -> a / b;
                case "^" -> pow(a, b);
                default  -> 0.0;
            };
        };
    }

    private static Expression<Double> applyFunction(String function, Expression<Double> operand) {
        Expression<Double> operation = FUNCTION_VALUES.get(function);
        return x -> {
            double a = operand.eval(x);
            return operation.eval(a);
        };
    }

    private static boolean isNumber(String token) {
        return toNumber(token, Double::parseDouble) != null;
    }

    private static boolean isVariable(String token) {
        return token.equalsIgnoreCase("x");
    }

    private static boolean isConstant(String token) {
        return CONSTANTS.contains(token);
    }

    private static boolean isOperator(String token) {
        return PRECEDENCE.containsKey(token);
    }

    private static boolean isFunction(String token) {
        return FUNCTIONS.contains(token);
    }
}