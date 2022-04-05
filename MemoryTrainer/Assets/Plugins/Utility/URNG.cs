using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Unique Random number generator
    /// </summary>
    public class URNG
    {
        public int Capacity => mCapacity;
        private int mCapacity;

        private int[] mValues;
        private int mCounter;

        public URNG(int capacity)
        {
            mValues = new int[capacity];
            Init(capacity);
        }

        public void ReInit(int capacity)
        {
            Init(capacity);
            ResetCounter();
        }

        private void Init(int capacity)
        {
            if (mCapacity == capacity)
                return;

            mCapacity = capacity;

            if (mCapacity > mValues.Length)
                mValues = new int[mCapacity];

            for (int i = 0; i < mCapacity; i++)
                mValues[i] = i;

            ResetCounter();
        }

        public void ResetCounter()
        {
            mCounter = mCapacity - 1;
        }

        public int Get()
        {
            var index = Random.Range(0, mCounter); // [0 mCounter)
            var value = mValues[index];
            mValues[index] = mValues[mCounter];
            mValues[mCounter] = value;
            mCounter--;
            if (mCounter < 0)
                ResetCounter();
            return value;
        }
    }

    public struct URNGEnumerator<T> : IEnumerator<T>, IEnumerable<T>
    {
        private List<T> _list;
        private URNG _URNG;
        private int _index;
        private int _count;

        public URNGEnumerator(List<T> list, URNG urng)
        {
            _list = list;
            _URNG = urng;
            _URNG.ReInit(_list.Count);
            _index = -1;
            _count = -1;
        }

        public T Current => _list[_index];
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            _index = _URNG.Get();
            _count++;
            return _count < _list.Count;
        }

        public void Reset()
        {
            _index = -1;
            _count = -1;
            _URNG.ResetCounter();
        }

        public void Dispose()
        {
            _list = null;
            _URNG = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}