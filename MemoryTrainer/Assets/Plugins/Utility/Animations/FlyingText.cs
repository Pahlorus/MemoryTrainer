
namespace Utility.Animations
{
    using System;

    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;

    using Utility.Attributes;

    public class FlyingText : MonoBehaviour
    {
        [Serializable]
        public class AlphaEvent : UnityEvent<Color> { }

        private const int _MAX = 5;
        private static readonly IndexedQueue<FlyingText> _FLYING_TEXTS = new IndexedQueue<FlyingText>(_MAX);

        [SerializeField, Require] TMP_Text _text;
        [SerializeField, Require] FlyingTextData _data;
        [SerializeField, Require] Transform _tr;

        [SerializeField] private Enabler _animation;
        private Vector2 _position;
        private Vector2 _direction;

        private bool _destroyed;

        public AlphaEvent OnAlphaChanged;

        private void Awake()
        {
            _animation = new Enabler();
            EnsureCount();
            _FLYING_TEXTS.Enqueue(this);
        }

        private void Update()
        {
            if (_animation.Update(Time.deltaTime / _data.Time))
            {
                var value = _animation.Value;

                var color = _data.Color.Evaluate(value);
                var distance = _data.Distance.Evaluate(value);
                var scale = _data.Scale.Evaluate(value);

                _text.color = color;
                OnAlphaChanged.Invoke(new Color(1, 1, 1, color.a));

                _tr.position = _data.Clamp(_position + _direction * distance);
                _tr.localScale = new Vector3(scale, scale, scale);
            }
            else
                Stop();
        }

        private void EnsureCount()
        {
            while (_FLYING_TEXTS.Count >= _MAX)
            {
                var flyingText = _FLYING_TEXTS.Dequeue();
                flyingText.Stop();
            }
        }
        
        private void Rotate()
        {
            _tr.rotation = Quaternion.Euler(0, 0, 90);
        }

        public void Fly(Vector2 position, Vector2 direction, string text, bool rotate)
        {
            if (rotate)
                Rotate();

            var angle = _data.RandomAngle * 0.5f;
            angle = UtilityExtensions.RandomRange(-angle, angle);
            var distance = _data.RandomDistance;
            distance = UtilityExtensions.RandomRange(-distance, distance);
            distance = Mathf.Pow(2, distance);

            _position = position;
            _direction = direction.Rotate(angle) * distance;
            _text.text = _data.Prefix + text;
            _animation.Init(false);
            _animation.SetEnabled(true);
        }

        public void Stop()
        {
            if (_destroyed)
                return;
            _destroyed = true;
            Destroy(gameObject);
        }
    }
}