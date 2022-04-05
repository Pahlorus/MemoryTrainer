using System.Collections.Generic;

using UnityEngine;

namespace Utility
{
    public class AverageCounter
    {
        public readonly int Size;
        private Queue<float> _values;
        public float Average { get; private set; }
        public float Sum { get; private set; }
        private float _lastValue;

        public AverageCounter(int size)
        {
            Size = size;
            _values = new Queue<float>(Size);
        }

        public void Add(float value)
        {
            var count = _values.Count;
            if (count == Size)
            {
                _lastValue = _values.Dequeue();
                Sum -= _lastValue;
            }

            _values.Enqueue(value);
            Sum += value;
            Average = Sum / count;
        }

        public void Clear()
        {
            _values.Clear();
            Sum = 0;
            Average = 0;
            _lastValue = 0;
        }

        public void Fill(float value)
        {
            Clear();
            for (int i=0; i<=Size;i++)
                Add(value);
            _lastValue = value;
        }

        public float GetFluctuation()
        {
            var last = _lastValue;
            var fluctuation = 0f;
            foreach (var value in _values)
            {
                fluctuation += Mathf.Abs(value - last);
                last = value;
            }
            return fluctuation / Size;
        }
    }
}