package lanta.math;

import java.util.Arrays;
import java.util.Collection;
import java.util.function.Function;
import java.util.stream.Stream;

public class Matrix<T extends Number> {
    private final T[][] content;
    public final Function<Number, T> converter;
    public final int rows;
    public final int columns;

    public Matrix(Matrix<T> matrix){
        this(matrix.content, matrix.converter);
    }

    public Matrix(T[][] data, Function<Number, T> converter){
        if (data == null || data.length == 0) throw new IllegalArgumentException("Matrix data cannot be null or empty.");
        if (data[0] == null) throw new IllegalArgumentException("First row cannot be null.");
        if (!isRetangularArray(data)) throw new IllegalArgumentException("Matrix must be rectangular.");
        this.rows = data.length;
        this.columns = data[0].length;
        this.content = copyArray(data);
        this.converter = converter;
    }

    public Matrix(T[] data, Function<Number, T> converter){
        if (data == null || data.length == 0) throw new IllegalArgumentException("Matrix data cannot be null or empty.");
        this.rows = data.length;
        this.columns = 1;
        this.content = extrudeArray(data);
        this.converter = converter;
    }

    public Matrix(String source, Function<String, T> parser, Function<Number, T> converter) {
        if (source == null || source.isBlank()) throw new IllegalArgumentException("Matrix source string cannot be null or empty.");
        String[] lines = source.lines().map(String::trim).filter(line -> !line.isEmpty()).toArray(String[]::new);
        if (lines.length == 0) throw new IllegalArgumentException("No data rows found, check string format.");
        String[] firstRow = lines[0].split("\\s*,\\s*");
        if (firstRow.length == 0) throw new IllegalArgumentException("First row has no columns.");

        this.rows = lines.length;
        this.columns = firstRow.length;

        T[][] data = newArray(rows, columns);
        for (int i = 0; i < rows; i++) {
            String[] cells = lines[i].split("\\s*,\\s*");
            if (cells.length != columns) throw new IllegalArgumentException("Unexpected amount of rows");
            for (int j = 0; j < columns; j++) {
                try {
                    data[i][j] = Parser.toNumber(cells[j].trim(), parser, null);
                } catch (Exception e) {
                    throw new IllegalArgumentException("Failed to parse text", e);
                }
            }
        }
        this.content = data;
        this.converter = converter;
    }

    public T determinant() {
        if (!isSquare()) throw new ArithmeticException("Square matrix required");
        Double[][] array = convertArray(content, Number::doubleValue);
        double det = 1.0;
        for (int i = 0; i < rows; i++) {
            int pivot = i;
            for (int k = i+1; k < rows; k++)
                if (Math.abs(array[k][i]) > Math.abs(array[pivot][i])) pivot = k;
            if (Math.abs(array[pivot][i]) < 1e-12) return converter.apply(0.0);
            if (pivot != i) {
                Double[] row = array[i];
                array[i] = array[pivot];
                array[pivot] = row;
                det = -det;
            }
            det *= array[i][i];
            for (int k = i+1; k < rows; k++) {
                double factor = array[k][i] / array[i][i];
                for (int j = i+1; j < rows; j++) array[k][j] -= factor * array[i][j];
            }
        }
        return converter.apply(det);
    }

    public T get(int i) throws ArrayIndexOutOfBoundsException{
        if(isRowVector()) return content[0][i];
        return content[i][0];
    }

    public T get(int i, int j) throws ArrayIndexOutOfBoundsException{
        return content[i][j];
    }

    public void put(int i, Number value) throws ArrayIndexOutOfBoundsException{
        if(isRowVector()) content[0][i] = converter.apply(value);
        else content[i][0] = converter.apply(value);
    }

    public void put(int i, int j, Number value) throws ArrayIndexOutOfBoundsException{
        content[i][j] = converter.apply(value);
    }

    public void scale(Number scalar){
        for (int i = 0; i < this.rows; i++) {
            for (int j = 0; j < this.columns; j++) {
                content[i][j] = converter.apply(content[i][j].doubleValue() * scalar.doubleValue());
            }
        }
    }

    public boolean isSquare(){
        return rows == columns;
    }

    public boolean isRowVector(){
        return rows == 1;
    }

    public boolean isColumnVector(){
        return columns == 1;
    }

    public boolean isDominant(){
        if(!isSquare()) return false;
        for (int i = 0; i < rows; i++) {
            double sum = 0.0;
            for (int j = 0; j < columns; j++) {
                if(i != j) sum += Math.abs(content[i][j].doubleValue());
            }
            if(!(Math.abs(content[i][i].doubleValue()) > sum)) return false;
        }
        return true;
    }

    public void add(Matrix<?> matrix){
        if((rows != matrix.rows) || (columns != matrix.columns)) throw new IllegalStateException("Both matrices must be of same proportions");
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                content[i][j] = converter.apply(content[i][j].doubleValue() + matrix.content[i][j].doubleValue());
            }
        }
    }

    public void subtract(Matrix<?> matrix){
        if((rows != matrix.rows) || (columns != matrix.columns)) throw new IllegalStateException("Both matrices must be of same proportions");
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                content[i][j] = converter.apply(content[i][j].doubleValue() - matrix.content[i][j].doubleValue());
            }
        }
    }

    public Matrix<T> transpose() {
        T[][] transposed = newArray(columns, rows);
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                transposed[j][i] = content[i][j];
            }
        }
        return new Matrix<>(transposed, this.converter);
    }

    public static <T extends Number> Matrix<T> convertType(Matrix<?> matrix, Function<Number, T> converter){
        int rows = matrix.rows;
        int columns = matrix.columns;
        T[][] copy = convertArray(matrix.content, converter);
        return new Matrix<>(copy, converter);
    }

    public static Matrix<Double> add(Matrix<?> a, Matrix<?> b){
        if((a.rows != b.rows) || (a.columns != b.columns)) throw new IllegalStateException("Both matrices must be of same proportions");
        Double[][] newArray = new Double[a.rows][a.columns];
        for (int i = 0; i < a.rows; i++) {
            for (int j = 0; j < a.columns; j++) {
                newArray[i][j] = a.content[i][j].doubleValue() + b.content[i][j].doubleValue();
            }
        }
        return new Matrix<>(newArray, Number::doubleValue);
    }

    public static Matrix<Double> subtract(Matrix<?> a, Matrix<?> b){
        if((a.rows != b.rows) || (a.columns != b.columns)) throw new IllegalStateException("Both matrices must be of same proportions");
        Double[][] newArray = new Double[a.rows][a.columns];
        for (int i = 0; i < a.rows; i++) {
            for (int j = 0; j < a.columns; j++) {
                newArray[i][j] = a.content[i][j].doubleValue() - b.content[i][j].doubleValue();
            }
        }
        return new Matrix<>(newArray, Number::doubleValue);
    }

    public static <T extends Number> Matrix<T> transpose(Matrix<T> matrix){
        return matrix.transpose();
    }

    public static <T extends Number> Matrix<T> add(Matrix<?> a, Matrix<?> b, Function<Number, T> converter){
        return convertType(add(a, b), converter);
    }

    public static <T extends Number> Matrix<T> subtract(Matrix<?> a, Matrix<?> b, Function<Number, T> converter){
        return convertType(subtract(a, b), converter);
    }

    public static Matrix<Double> multiply(Matrix<?> a, Matrix<?> b){
        Double[][] result = new Double[a.rows][b.columns];
        if(a.columns != b.rows) throw new IllegalArgumentException("a must have as many columns as b has rows");
        for (int i = 0; i < a.rows; i++) {
            for (int j = 0; j < b.columns; j++) {
                double sum = 0;
                for (int k = 0; k < b.rows; k++) {
                    sum += a.content[i][k].doubleValue() * b.content[k][j].doubleValue();
                }
                result[i][j] = sum;
            }
        }
        return new Matrix<>(result, Number::doubleValue);
    }

    public static <T extends Number> Matrix<T> multiply(Matrix<?> a, Matrix<? extends Number> b, Function<Number, T> converter, Class<T> type){
        if(a.columns != b.rows) throw new IllegalArgumentException("a must have as many columns as b has rows");
        return convertType(multiply(a, b), converter);
    }

    public static  <T extends Number> T[][] convertArray(Number[][] original, Function<Number, T> converter){
        if (original == null || original.length == 0) throw new IllegalArgumentException("Array data cannot be null or empty.");
        if (original[0] == null) throw new IllegalArgumentException("First row cannot be null.");
        int rows = original.length;
        int columns = original[0].length;
        T[][] copy = newArray(rows, columns);
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                copy[i][j] = converter.apply(original[i][j]);
            }
        }
        return copy;
    }

    public static  <T extends Number> T[] convertArray(Number[] original, Function<Number, T> converter){
        if (original == null || original.length == 0) throw new IllegalArgumentException("Array data cannot be null or empty.");
        if (original[0] == null) throw new IllegalArgumentException("First row cannot be null.");
        int rows = original.length;
        T[] copy = newArray(rows);
        for (int i = 0; i < rows; i++) {
            copy[i] = converter.apply(original[i]);
        }
        return copy;
    }

    public static <T extends Number> boolean isRetangularArray(T[]... array){
        if(array.length == 0 || array[0] == null) return false;
        return Stream.of(array).allMatch(value -> value.length == array[0].length);
    }

    public static <T extends Number> T[][] newArray(int rows, int columns){
        @SuppressWarnings("unchecked")
        T[][] array = (T[][]) new Number[rows][columns];
        return array;
    }

    public static <T extends Number> T[][] newArray(T[]... arrays) throws IllegalArgumentException{
        if(!isRetangularArray(arrays)) throw new IllegalArgumentException("Arrays must be of same proportions.");
        int rows = arrays.length;
        int columns = arrays[0].length;
        T[][] array = newArray(rows, columns);
        System.arraycopy(arrays, 0, array, 0, rows);
        return array;
    }

    public static <T extends Number> T[] newArray(int length){
        @SuppressWarnings("unchecked")
        T[] array = (T[]) new Number[length];
        return array;
    }

    public static <T extends Number> T[] newArray(Collection<T> content){
        int length = content.size();
        @SuppressWarnings("unchecked")
        T[] array = (T[]) new Number[length];
        int i = 0;
        for (T value : content) {
            if (i >= length) break;
            array[i] = value;
            i++;
        }
        return array;
    }

    public static <T extends Number> T[][] copyArray(T[][] original){
        if (original == null || original.length == 0) throw new IllegalArgumentException("Array data cannot be null or empty.");
        if (original[0] == null) throw new IllegalArgumentException("First row cannot be null.");
        int rows = original.length;
        int columns = original[0].length;
        T[][] newArray = Arrays.copyOf(original, rows);
        for (int i = 0; i < rows; i++) {
            newArray[i] = Arrays.copyOf(original[i], columns);
        }
        return newArray;
    }

    public static <T extends Number> T[][] extrudeArray(T[] original){
        if (original == null || original.length == 0) throw new IllegalArgumentException("Array data cannot be null or empty.");
        T[][] resized = newArray(original.length, 1);
        for (int i = 0; i < original.length; i++) resized[i][0] = original[i];
        return resized;
    }

    public static <T extends Number> T[] populateArray(T[] array, T value){
        Arrays.fill(array, value);
        return array;
    }

    public static <T extends Number> T[][] populateArray(T[][] array, T value){
        for (T[] arr : array) Arrays.fill(arr, value);
        return array;
    }

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < rows; i++) {
            if (i > 0) builder.append("\n");
            for (int j = 0; j < columns; j++) {
                builder.append((j > 0) ? ", " : "").append(content[i][j]);
            }
        }
        return builder.toString();
    }

    @Override
    public int hashCode(){
        return Arrays.deepHashCode(content);
    }
}
