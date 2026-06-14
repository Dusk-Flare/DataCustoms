package lanta.dataTypes;

import lanta.dataTypes.lists.SinglyLinkedList;

public class Queue<T> {
    SinglyLinkedList<T> queue;
    public Queue(){
        queue = new SinglyLinkedList<>();
    }

    public boolean isEmpty(){
        return queue.isEmpty();
    }

    public int size(){
        return queue.size();
    }

    public T poll(){
        return queue.poll();
    }

    public T peek(){
        return queue.front();
    }

    public void push(T data){
        queue.push(data);
    }

    @Override
    public String toString(){
        return queue.toString();
    }
}
