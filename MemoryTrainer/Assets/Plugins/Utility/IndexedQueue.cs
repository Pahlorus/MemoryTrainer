using System.Collections;
using System.Collections.Generic;

namespace Utility
{
    public class IndexedQueue<T>
    {
        private T[] _array;
        private int _start;
        private int _len;

        public IndexedQueue(int initialBufferSize)
        {
            _array = new T[initialBufferSize];
            _start = 0;
            _len = 0;
        }

        public void Enqueue(T t)
        {
            if (_len == _array.Length)
            {
                //increase the size of the cicularBuffer, and copy everything
                T[] bigger = new T[_array.Length * 2];
                for (int i = 0; i < _len; i++)
                {
                    bigger[i] = _array[(_start + i) % _len];
                }
                _start = 0;
                _array = bigger;
            }
            _array[(_start + _len) % _array.Length] = t;
            ++_len;
        }

        public T Dequeue()
        {
            if (_len == 0)
                throw new System.InvalidOperationException("Queue is empty");
            var result = _array[_start];
            _start = (_start + 1) % _array.Length;
            --_len;
            return result;
        }

        public int Count { get { return _len; } }

        public T this[int index]
        {
            get
            {
                return _array[(_start + index) % _array.Length];
            }
            set
            {
                _array[(_start + index) % _array.Length] = value;
            }
        }

        public IEnumerable<T> AsEnumerable() => new QueueEnumerable(this, false);
        public IEnumerable<IndexedValue> AsIndexedEnumerable() => new IndexedQueueEnumerable(this, false);
        public IEnumerable<T> AsReversedEnumerable() => new QueueEnumerable(this, true);
        public IEnumerable<IndexedValue> AsReversedIndexedEnumerable() => new IndexedQueueEnumerable(this, true);

        private struct QueueEnumerable : IEnumerable<T>
        {
            private IndexedQueue<T> _parent;
            private bool _reversed;

            public QueueEnumerable(IndexedQueue<T> parent, bool reversed)
            {
                _parent = parent;
                _reversed = reversed;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new QueueEnumerator(_parent, _reversed);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new QueueEnumerator(_parent, _reversed);
            }
        }

        private struct QueueEnumerator : IEnumerator<T>
        {
            private IndexedQueue<T> _parent;
            private int _index;
            private bool _reversed;

            public T Current => _parent[_index];
            object IEnumerator.Current => _parent[_index];

            public QueueEnumerator(IndexedQueue<T> parent, bool reversed)
            {
                _parent = parent;
                _reversed = reversed;
                _index = reversed ? _parent.Count : -1;
            }

            public void Dispose()
            {
                Reset();
                _parent = null;
            }

            public bool MoveNext()
            {
                if (_reversed)
                {
                    _index--;
                    return _index >= 0;
                }
                else
                {
                    _index++;
                    return _index < _parent.Count;
                }
            }

            public void Reset()
            {
                _index = _reversed ? _parent.Count : -1;
            }
        }

        private struct IndexedQueueEnumerable : IEnumerable<IndexedValue>
        {
            private IndexedQueue<T> _parent;
            private bool _reversed;

            public IndexedQueueEnumerable(IndexedQueue<T> parent, bool reversed)
            {
                _parent = parent;
                _reversed = reversed;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new IndexedQueueEnumerator(_parent, _reversed);
            }

            IEnumerator<IndexedValue> IEnumerable<IndexedValue>.GetEnumerator()
            {
                return new IndexedQueueEnumerator(_parent, _reversed);
            }
        }

        private struct IndexedQueueEnumerator : IEnumerator<IndexedValue>
        {
            private IndexedQueue<T> _parent;
            private int _index;
            private bool _reversed;

            object IEnumerator.Current => new IndexedValue { Index = _index, Value = _parent[_index] };
            IndexedValue IEnumerator<IndexedValue>.Current => new IndexedValue { Index = _index, Value = _parent[_index] };

            public IndexedQueueEnumerator(IndexedQueue<T> parent, bool reversed)
            {
                _parent = parent;
                _reversed = reversed;
                _index = reversed ? _parent.Count : -1;
            }

            public void Dispose()
            {
                Reset();
                _parent = null;
            }

            public bool MoveNext()
            {
                if (_reversed)
                {
                    _index--;
                    return _index < _parent.Count;
                }
                else
                {
                    _index++;
                    return _index >= 0;
                }

            }

            public void Reset()
            {
                _index = _reversed ? _parent.Count : -1;
            }
        }

        public struct IndexedValue
        {
            public int Index;
            public T Value;
        }
    }
}