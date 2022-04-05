using System;
using UnityEngine;

namespace Utility.Animations
{
    [Serializable]
    public class SimpleAnimationLinearFloat : SimpleAnimation<float>
    {
        protected override float Zero => 0;

        protected override float InverseLerp(float from, float to, float value)
        {
            return Mathf.InverseLerp(from, to, value);
        }

        protected override float Lerp(float from, float to, float speed)
        {
            return Mathf.Clamp(from + (from < to ? speed : -speed), Mathf.Min(from, to), Mathf.Max(from, to));
        }

        protected override float Plus(float value, float add)
        {
            return value + add;
        }

        protected override float Minus(float value, float sub)
        {
            return value - sub;
        }

        protected override bool Equals(float x, float y)
        {
            return x == y;
        }

    }
}