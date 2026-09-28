namespace DataCustoms.Collections.Lists
{
    public class SinglyNode<T>(T data)
    {
        public T Value { get; set; } = data;
        public SinglyNode<T> Next { get; set; } = null;
        public void SetNext(T value) => Next = new(value);
        public bool Test(Predicate<T> predicate) => predicate.Invoke(Value);
        public override string ToString() => Value?.ToString();
    }
}
