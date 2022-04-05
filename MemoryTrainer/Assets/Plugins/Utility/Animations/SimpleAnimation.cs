using System;
using UnityEngine;

namespace Utility.Animations
{
    [Serializable]
    public abstract class SimpleAnimation<T> : ISimpleAnimation, IWaitableAnimation
    {

        public T CurrentValue => mCurrentValue;
        [SerializeField] private T mCurrentValue;
        public T Diff => mDiff;
        [SerializeField] private T mDiff;
        public T TargetValue => mTargetValue;
        [SerializeField] private T mTargetValue;

        public T BeginValue => Minus(mTargetValue, mDiff);
        [SerializeField] private T mStartValue;

        public float Progress => mProgress;
        [Range(0, 1)]
        [SerializeField] private float mProgress;

        public float InterpolationSpeed = 1f;
        public float Tolerance = 0.01f;

        public bool Completed => _Completed;
        [SerializeField] private bool _Completed;
        protected abstract T Lerp(T from, T to, float speed);
        protected abstract float InverseLerp(T from, T to, T value);
        protected abstract T Plus(T value, T add);
        protected abstract T Minus(T value, T sub);
        protected abstract T Zero { get; }
        bool IWaitableAnimation.keepWaiting => !_Completed;

        private WaitAnimationHandle _waitAnimationHandle;

        protected abstract bool Equals(T x, T y);

        public SimpleAnimation()
        {
            _waitAnimationHandle = new WaitAnimationHandle(this);
        }

        private void Init(T value)
        {
            mCurrentValue = value;
            mDiff = Zero;
            mStartValue = value;
            mTargetValue = value;
            mProgress = 1;
        }

        public void SetTarget(T newTargetValue, bool instant = false)
        {

            if (instant)
            {
                Init(newTargetValue);
                return;
            }

            var oldTargetValue = mTargetValue;
            mTargetValue = newTargetValue;
            mStartValue = mCurrentValue;
            mDiff = Plus(mDiff, Minus(newTargetValue, oldTargetValue));
            _Completed = false;
        }

        public void Update(float dt)
        {
            mCurrentValue = Lerp(mCurrentValue, mTargetValue, InterpolationSpeed * dt);
            if (Equals(mStartValue, mTargetValue))
                mProgress = 1;
            else
                mProgress = InverseLerp(mStartValue, mTargetValue, mCurrentValue);
            if (Mathf.Abs(1 - mProgress) < Tolerance)
                Finish();
        }

        private void Finish()
        {
            mProgress = 1;
            mCurrentValue = mTargetValue;
            mStartValue = mTargetValue;
            mDiff = Zero;
            _Completed = true;
        }

        public void OnValidate()
        {
            mCurrentValue = Lerp(mStartValue, mTargetValue, mProgress);
        }

        public WaitAnimationHandle Wait() => _waitAnimationHandle;
    }

    public interface ISimpleAnimation
    {
        bool Completed { get; }
        void Update(float dt);
    }
}