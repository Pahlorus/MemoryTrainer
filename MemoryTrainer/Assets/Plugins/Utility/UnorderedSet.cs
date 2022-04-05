using System.Collections;
using System.Collections.Generic;

namespace Utility
{
    public class UnorderedSet<T> : IEnumerable<T> where T : UnorderedSetElement
    {
        private List<T> _collection;

        public int Count => _collection.Count;

        public UnorderedSet()
        {
            _collection = new List<T>();
        }

        public UnorderedSet(int capacity)
        {
            _collection = new List<T>(capacity);
        }

        public void Add(T element)
        {
            var index = _collection.Count;
            element.Index = index;
            _collection.Add(element);
        }

        public void Remove(T element)
        {
            var count = _collection.Count;
            var lastIndex = count - 1;
            switch (count)
            {
                case 0:
                    break;
                case 1:
                    _collection.RemoveAt(0);
                    break;
                default:
                    var index = element.Index;
                    if (index >= count)
                        break;
                    else if (index < lastIndex)
                    {
                        var lastElement = _collection[lastIndex];
                        lastElement.Index = index;
                        _collection[index] = lastElement;
                    }
                    _collection.RemoveAt(lastIndex);
                    break;
            }
        }

        public T this[int index]
        {
            get => _collection[index];
        }



        public void Clear()
        {
            _collection.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
    }

    public interface UnorderedSetElement
    {
        int Index { get; set; }
    }
}
