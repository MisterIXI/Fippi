using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PriorityQueue<T>
{
    private SortedDictionary<int, Queue<T>> dict = new SortedDictionary<int, Queue<T>>();

    public int Count { get; private set; }

    public void Enqueue(T item, int priority)
    {
        if (!dict.ContainsKey(priority))
        {
            dict[priority] = new Queue<T>();
        }
        dict[priority].Enqueue(item);
        Count++;
    }

    public T Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Queue is empty");
        }
        var item = dict.First().Value.Dequeue();
        if (dict.First().Value.Count == 0)
        {
            dict.Remove(dict.First().Key);
        }
        Count--;
        return item;
    }

    public T Peek()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Queue is empty");
        }
        return dict.First().Value.Peek();
    }

}
