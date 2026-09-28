using System.Collections;
using System.Text;

namespace DataCustoms.Collections.Lists
{
    public class SinglyLinkedList<T> : ICollection<T>, IEnumerable<SinglyNode<T>>
    {
        public SinglyLinkedList() { }

        public SinglyLinkedList(ICollection<T> collection)
        {
            AddAll(collection);
        }

        public SinglyLinkedList(T[] array)
        {
            AddAll(array);
        }

        public SinglyLinkedList<T> Copy()
        {
            return [.. this];
        }
        public int Count
        {
            get
            {
                int count = 0;
                foreach (T item in this)
                {
                    count++;
                }
                return count;
            }
        }

        public bool IsReadOnly => false;

        public bool IsEmpty => _headNode == null;

        private SinglyNode<T> _headNode;

        public void Add(T item)
        {
            SinglyNode<T> temp = _headNode;
            while (temp.Next != null) temp = temp.Next;
            temp.SetNext(item);
        }

        public void AddAll(ICollection<T> collection)
        {
            foreach(T data in collection) Add(data);
        }

        public void AddAll(T[] array)
        {
            foreach(T data in array) Add(data);
        }

        public bool CompareAddAfter(T data, Predicate<T> condition)
        {
            SinglyNode<T> newNode = new(data);
            if (_headNode == null) _headNode = newNode;
            else
            {
                for (SinglyNode<T> cn = _headNode; cn != null; cn = cn.Next)
                {
                    if (cn.Test(condition))
                    {
                        newNode.Next = cn.Next;
                        cn.Next = newNode;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CompareAddBefore(T data, Predicate<T> condition)
        {
            SinglyNode<T> newNode = new(data);
            SinglyNode<T> previous = null;
            if (_headNode == null) _headNode = newNode;
            else
            {
                for (SinglyNode<T> cn = _headNode; cn != null; cn = cn.Next)
                {
                    if (cn.Test(condition))
                    {
                        previous?.Next = newNode;
                        newNode.Next = cn;
                        return true;
                    }
                    previous = cn;
                }
            }
            return false;
        }

        public T Pop()
        {
            if (_headNode == null) throw new InvalidOperationException("No elements left.");
            T element = _headNode.Value;
            _headNode = _headNode.Next;
            return element;
        }

        public T Poll()
        {
            SinglyNode<T> prev = _headNode;
            if (IsEmpty || _headNode.Next == null)
            {
                _headNode = null;
                return prev.Value;
            }
            for (SinglyNode<T> cn = _headNode; cn != null; cn = cn.Next)
            {
                if (cn.Next == null)
                {
                    prev.Next = null;
                    return cn.Value;
                }
                prev = cn;
            }
            throw new InvalidOperationException("No elements left.");
        }

        public T Peek()
        {
            if (_headNode == null) throw new InvalidOperationException("No elements left.");
            return _headNode.Value;
        }

        public T Front()
        {
            if (_headNode == null) throw new InvalidOperationException("No elements left.");
            T data = default;
            foreach (T value in this) data = value;
            return data;
        }

        public void Push(T data)
        {
            SinglyNode<T> newHead = new(data)
            {
                Next = _headNode
            };
            _headNode = newHead;
        }

        public void CopyTo(T[] array, int index)
        {
            ArgumentNullException.ThrowIfNull(array);
            SinglyNode<T> node = _headNode;
            for (int i = 0; i < Count; i++)
            {
                if (node == null) return;
                array[i] = node.Value;
                node = node.Next;
            }
        }

        public void Clear() => _headNode = null;

        public bool Contains(T item)
        {
            foreach (T value in this)
            {
                if(Equals(value, item)) return true;
            }
            return false;
        }

        public bool Remove(T item)
        {
            SinglyNode<T> last = _headNode;
            foreach (SinglyNode<T> node in this as IEnumerable<SinglyNode<T>>)
            {
                SinglyNode<T> next = node.Next;
                if (Equals(node.Value, item))
                {
                    last.Next = next;
                    return true;
                }
                last = node;
            }
            return false;
        }

        public bool RemoveAllOf(T data)
        {
            return CompareRemoveAllOf(value => Equals(data, value));
        }

        public bool CompareRemoveAllOf(Predicate<T> condition)
        {
            bool result = false;
            if (_headNode == null) return false;
            while (_headNode.Test(condition))
            {
                Pop();
                result = true;
            }
            SinglyNode<T> previous = _headNode;
            for (SinglyNode<T> cn = _headNode; cn != null; cn = cn.Next)
            {
                if (cn.Test(condition))
                {
                    previous.Next = cn.Next;
                    cn = previous;
                    result = true;
                }
                previous = cn;
            }
            return result;
        }

        public void RemoveDuplicates()
        {
            foreach (T value in this)
            {
                if (Search(value).Count != 1) Remove(value);
            }
        }

        public bool CompareRemove(Predicate<T> condition)
        {
            if (_headNode == null) return false;
            if (_headNode.Test(condition))
            {
                Pop();
                return true;
            }
            SinglyNode<T> previous = _headNode;
            for (SinglyNode<T> cn = _headNode; cn != null; cn = cn.Next)
            {
                if (cn.Test(condition))
                {
                    previous.Next = cn.Next;
                    return true;
                }
                previous = cn;
            }
            return false;
        }

        public SinglyLinkedList<T> Search(T data)
        {
            return CompareSearch(value => Equals(data, value));
        }

        public SinglyLinkedList<T> CompareSearch(Predicate<T> condition)
        {
            SinglyLinkedList<T> result = [];
            foreach (T value in this)
            {
                if (condition.Invoke(value)) result.Add(value);
            }
            return result;
        }

        public SinglyLinkedList<T> Intersect(SinglyLinkedList<T> list)
        {
            SinglyLinkedList<T> intersection = [];
            foreach (T value in this)
            {
                intersection.Add(list.Search(value).Pop());
            }
            return intersection;
        }

        public IEnumerator<T> GetEnumerator()
        {
            SinglyNode<T> temp = _headNode;
            while (temp != null)
            {
                yield return temp.Value;
                temp = temp.Next;
            }
        }

        IEnumerator<SinglyNode<T>> IEnumerable<SinglyNode<T>>.GetEnumerator()
        {
            SinglyNode<T> temp = _headNode;
            while (temp != null)
            {
                yield return temp;
                temp = temp.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder str = new();
            str.Append('[');
            if (_headNode == null) str.Append(']');
            for (SinglyNode<T> cn = _headNode; cn != null; cn = cn.Next)
            {
                str.Append(cn);
                str.Append(cn.Next != null ? ", " : ']');
            }
            return str.ToString();
        }
    }
}
