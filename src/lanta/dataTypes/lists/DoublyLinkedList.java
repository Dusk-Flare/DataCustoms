package lanta.dataTypes.lists;

import java.util.*;
import java.util.function.Predicate;

public class DoublyLinkedList<T> extends SinglyLinkedList<T> {
    DoublyNode<T> headNode;
    DoublyNode<T> leadNode;

    public DoublyLinkedList() {

    }

    public DoublyLinkedList(Collection<? extends T> collection) {
        addAll(collection);
    }

    public DoublyLinkedList(T[] array) {
        addAll(array);
    }

    @Override
    public Iterator<T> iterator() {
        return new DoublyNodeIterator(this);
    }

    public DoublyNodeIterator doublyIterator() {
        return new DoublyNodeIterator(this);
    }

    @Override
    public DoublyLinkedList<T> copy(){
        return new DoublyLinkedList<>(this);
    }

    @Override
    public boolean isEmpty(){
        return headNode == null && leadNode == null;
    }

    @Override
    public void clear(){
        this.headNode = null;
        this.leadNode = null;
    }

    @Override
    public T pop(){
        if(headNode == null) return null;
        T element = headNode.value();
        headNode = headNode.next();
        return element;
    }

    @Override
    public T front(){
        if (leadNode == null) return null;
        return leadNode.value();
    }

    @Override
    public T poll(){
        DoublyNode<T> prev = headNode;
        if(isEmpty() || headNode.next() == null){
            headNode = null;
            return prev.value();
        }
        for(DoublyNode<T> cn = headNode; cn != null; cn = cn.next()){
            if(cn.next() == null){
                prev.next(null);
                return cn.value();
            } prev = cn;
        }
        return null;
    }

    public DoublyLinkedList<T> reverse(){
        DoublyLinkedList<T> tempList = new DoublyLinkedList<>(this);
        DoublyLinkedList<T> reversed = new DoublyLinkedList<>();
        while(!tempList.isEmpty()){
            reversed.append(tempList.pop());
        }
        return reversed;
    }

    @Override
    public boolean compareAddAfter(T data, Predicate<T> condition){
        DoublyNode<T> newNode = new DoublyNode<>(data);
        if(headNode == null) headNode = newNode;
        else {
            for(DoublyNode<T> cn = headNode; cn != null; cn = cn.next()){
                if(cn.test(condition)){
                    newNode.next(cn.next());
                    cn.next(newNode);
                    newNode.previous(cn);
                    return true;
                }
            }
        }
        return false;
    }

    @Override
    public boolean compareAddBefore(T data, Predicate<T> condition){
        DoublyNode<T> newNode = new DoublyNode<>(data);
        if(headNode == null) headNode = newNode;
        else {
            for(DoublyNode<T> cn = leadNode; cn != null; cn = cn.previous()){
                if(cn.test(condition)){
                    newNode.previous(cn.previous());
                    cn.previous(newNode);
                    newNode.previous(cn);
                    return true;
                }
            }
        }
        return false;
    }

    @Override
    public boolean add(T data){
        DoublyNode<T> newNode = new DoublyNode<>(data);
        if(headNode == null) return append(data);
        if(leadNode == null && headNode.next() != null) leadNode = headNode.next();
        else if (leadNode == null) {
            headNode.next(newNode);
            leadNode = headNode.next();
        } else {
            leadNode.next(newNode);
            newNode.previous(leadNode);
            leadNode = leadNode.next();
            return true;
        }
        return false;
    }

    public boolean append(T data){
        DoublyNode<T> newNode = new DoublyNode<>(data);
        if(headNode == null) headNode = newNode;
        else {
            headNode.previous(newNode);
            newNode.next(headNode);
            headNode = headNode.previous();
        }
        if(leadNode == null) leadNode = headNode.next();
        return false;
    }

    public DoublyLinkedList<T> search(T data){
        return compareSearch(value -> Objects.equals(data, value));
    }

    public DoublyLinkedList<T> compareSearch(Predicate<T> condition){
        DoublyLinkedList<T> result = new DoublyLinkedList<>();
        for(T value : this){
            if(condition.test(value)) result.add(value);
        }
        return result;
    }

    public DoublyLinkedList<T> intersect(DoublyLinkedList<T> list){
        DoublyLinkedList<T> intersection = new DoublyLinkedList<>();
        for(T data : this){
            intersection.add(list.search(data).pop());
        }
        return intersection;
    }

    @Override
    boolean removeNode(SinglyNode<T> node){
        return false;
    }

    boolean removeNode(DoublyNode<T> node){
        if (headNode == null) return false;
        if (headNode.equals(node)) {
            pop();
            return true;
        }
        DoublyNode<T> previous = headNode;
        for(DoublyNode<T> cn = headNode; cn != null; cn = cn.next()){
            if(cn.equals(node)){
                previous.next(cn.next());
                if(cn.next() != null) cn.next().previous(previous);
                return true;
            }
            previous = cn;
        }
        return false;
    }

    @Override
    public boolean compareRemove(Predicate<T> condition){
        if(headNode == null) return false;
        if (headNode.test(condition)) {
            pop();
            return true;
        }
        DoublyNode<T> previous = headNode;
        for(DoublyNode<T> cn = headNode; cn != null; cn = cn.next()){
            if(cn.test(condition)){
                previous.next(cn.next());
                if(cn.next() != null) cn.next().previous(previous);
                return true;
            }
            previous = cn;
        }
        return false;
    }

    @Override
    public T extract(Predicate<T> condition){
        if(headNode == null) return null;
        if (headNode.test(condition)) return pop();
        DoublyNode<T> previous = headNode;
        for(DoublyNode<T> cn = headNode; cn != null; cn = cn.next()){
            if(cn.test(condition)){
                previous.next(cn.next());
                if(cn.next() != null) cn.next().previous(previous);
                return cn.value();
            }
            previous = cn;
        }
        return null;
    }

    @Override
    public boolean compareRemoveAllOf(Predicate<T> condition){
        boolean result = false;
        if(headNode == null) return false;
        while (headNode.test(condition)){
            pop();
            result = true;
        }
        DoublyNode<T> previous = headNode;
        for(DoublyNode<T> cn = headNode; cn != null; cn = cn.next()){
            if(cn.test(condition)){
                previous.next(cn.next());
                if(cn.next() != null) cn.next().previous(previous);
                cn = previous;
                result = true;
            }
            previous = cn;
        }
        return result;
    }

    @Override
    public String toString(){
        StringBuilder str = new StringBuilder();
        str.append("[");
        if(headNode == null) str.append("]");
        for(DoublyNode<T> cn = headNode; cn != null; cn = cn.next()){
            str.append(cn);
            str.append(cn.next() != null ? ", " : "]");
        }
        return str.toString();
    }

    @Override
    public boolean equals(Object o){
        if(this == o) return true;
        if(o == null || (getClass() != o.getClass())) return super.equals(o);
        DoublyLinkedList<?> newList = (DoublyLinkedList<?>) o;
        Iterator<T> thisIterator = iterator();
        Iterator<?> otherIterator = newList.iterator();

        while(thisIterator.hasNext() && otherIterator.hasNext()){
            if(!Objects.equals(thisIterator.next(), otherIterator.next())) return false;
            if(thisIterator.hasNext() ^ otherIterator.hasNext()) return false;
        }
        return true;
    }

    public class DoublyNodeIterator implements Iterator<T> {
        private DoublyNode<T> nextNode;
        private DoublyNode<T> lastReturned;
        private final DoublyNode<T> tailNode;

        DoublyNodeIterator(DoublyLinkedList<T> list) {
            this.tailNode = list.leadNode;
            this.nextNode = list.headNode;
            this.lastReturned = null;
        }

        @Override
        public boolean hasNext() {
            return nextNode != null;
        }

        @Override
        public T next() {
            if (!hasNext()) throw new NoSuchElementException();
            lastReturned = nextNode;
            nextNode = nextNode.next();
            return lastReturned.value();
        }

        public boolean hasPrevious() {
            if (nextNode == null) return tailNode != null;
            return nextNode.previous() != null;
        }

        public T previous() {
            if (!hasPrevious()) throw new NoSuchElementException();
            if (nextNode == null) {
                lastReturned = tailNode;
                nextNode = tailNode;
            } else {
                lastReturned = nextNode.previous();
                nextNode = lastReturned;
            }
            return lastReturned.value();
        }

        @Override
        public void remove() {
            if(lastReturned == null) throw new IllegalStateException();
            removeIf(_ -> true);
        }

        public void removeIf(Predicate<T> condition){
            if(lastReturned == null) throw new IllegalStateException();
            if(lastReturned.test(condition) && removeNode(lastReturned)) lastReturned = null;
        }
    }
}
