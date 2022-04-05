using UnityEngine;

using Utility.Attributes;

namespace Utility.Animations
{
    public class OpenCloseAnimationTransform : OpenCloseAnimation
    {
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private Vector3 _initialScale;

        [SerializeField, Require] Transform _transform;

        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;


        protected override void Awake()
        {
            base.Awake();

            _initialPosition = _transform.localPosition;
            _initialRotation = _transform.localRotation;
            _initialScale = _transform.localScale;

            UpdateTransform();
        }

        private void OnAnimatorMove()
        {
            if (keepWaiting)
            {
                UpdateTransform();
            }
        }

        protected override void Animator_Opened()
        {
            base.Animator_Opened();
            UpdateTransform();
        }

        protected override void Animator_Closed()
        {
            base.Animator_Closed();
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            _transform.localPosition = _initialPosition + Position;
            _transform.localRotation = _initialRotation * Rotation;
            _transform.localScale = _initialScale.Mult(Scale);
        }
    }
}
