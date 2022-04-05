using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPool<T>
{
    private Queue<T> _pool;

    public CPool()
    {
        _pool = new Queue<T>();
    }

    public bool TryGetInstance(out T instance)
    {
        if (_pool.Count > 0)
        {
            instance = _pool.Dequeue();
            return true;
        }
        else
        {
            instance = default;
            return false;
        }
    }

    public void Enqueue(T instance)
    {
        _pool.Enqueue(instance);
    }
}

