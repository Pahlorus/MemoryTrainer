using System.Collections.Generic;

using UnityEngine;

namespace Utility
{
    public struct ProbabilityHolder<Value>
    {
        private Dictionary<Value, float> _probalities;
        private float _sum;

        public void Init()
        {
            if (_probalities == null)
                _probalities = new Dictionary<Value, float>();

            _sum = 0;
            _probalities.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="weight">must be >= 0</param>
        public void Set(Value value, float weight)
        {
            weight = Mathf.Max(0, weight);

            if (_probalities.TryGetValue(value, out float oldWeight))
                _sum -= oldWeight;

            _probalities[value] = weight;
            _sum += weight;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="random">[0..1)</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(float random, out Value value)
        {
            random = Mathf.Clamp01(random) * _sum;
            foreach (var pair in _probalities)
            {
                var weight = pair.Value;
                if (random < weight)
                {
                    value = pair.Key;
                    return true;
                }
                else
                    random -= weight;
            }
            value = default;
            return false;
        }

        public float this[Value index]
        {
            get => _probalities.TryGetValue(index, out var value) ? value : 0;
            set => Set(index, value);
        }

        public IEnumerable<KeyValuePair<Value, float>> AsEnumerable() => _probalities;
    }
}
