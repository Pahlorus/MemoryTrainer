using UnityEngine;

using Utility.Attributes;

namespace Utility.Animations
{
    public class ShiftAnimation : MonoBehaviour
    {
        [SerializeField] private Enabler _enabler;
        [SerializeField, Require] private Transform _transform;
        [SerializeField] private Vector3 _hideShift;
        private Vector3 _initialPosition;
        private bool _inited;

        public float Speed { get => _enabler.Speed; set => _enabler.Speed = value; }


        public WaitAnimationHandle WaitAnimationHandle { get; private set; }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (_inited)
                return;

            WaitAnimationHandle = new WaitAnimationHandle(_enabler);
            _initialPosition = transform.localPosition;
            _enabler.Init(false);
            UpdateTransform();
            _inited = true;
        }

        private void Update()
        {
            if (_enabler.Update(Time.deltaTime))
                UpdateTransform();
            else
                enabled = false;
        }

        private void UpdateTransform()
        {
            transform.localPosition = _initialPosition + Vector3.Lerp(_hideShift, Vector3.zero, _enabler.Value);
        }

        public WaitAnimationHandle SetEnabled(bool isEnabled, bool instant = false)
        {
            _enabler.SetEnabled(isEnabled, instant);
            enabled = true;
            if (instant)
            {
                Init();
                UpdateTransform();
            }
            return WaitAnimationHandle;
        }
    }
}
