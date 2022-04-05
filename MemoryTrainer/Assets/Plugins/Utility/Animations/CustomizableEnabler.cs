using System;

using UnityEngine;
using UnityEngine.Events;

namespace Utility.Animations
{
    public class CustomizableEnabler : MonoBehaviour
    {
        [Serializable]
        private class FloatUpdate : UnityEvent<float> { }
        [Serializable]
        private class Vector3Update : UnityEvent<Vector3> { }

        [SerializeField] private Enabler _enabler;

        public float AnimationTime;


        [SerializeField] private FloatUpdate _onUpdateFloat;
        [SerializeField] private Vector3Update _onUpdateVector3;

        public bool UpdateEnabler(float dt)
        {
            _enabler.Speed = AnimationTime <= 0 ? float.PositiveInfinity : 1 / AnimationTime;
            var result = _enabler.Update(dt);
            var value = _enabler.Value;
            _onUpdateFloat?.Invoke(value);
            _onUpdateVector3?.Invoke(new Vector3(value, value, value));
            return result;
        }

        public Enabler SetEnabled(bool isEnabled, bool instant = false)
        {
            return _enabler.SetEnabled(isEnabled, instant);
        }
    }
}
