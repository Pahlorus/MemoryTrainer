#if UNITY_EDITOR
using UnityEditor;
#endif

using System;

using UnityEngine;

namespace Utility.Animations
{

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(Enabler))]
    public class EnablerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enabledProp = property.FindPropertyRelative(nameof(Enabler.Enabled));
            EditorGUI.PropertyField(position, enabledProp, label);
        }
    }

#endif


    [Serializable]
    public class Enabler : CustomYieldInstruction, IWaitableAnimation
    {
        public bool Enabled;
        [NonSerialized] public float Value;
        [NonSerialized] public float Speed = 1;
        private bool _forceUpdate;
        public override bool keepWaiting =>  _forceUpdate || Enabled && Value < 1 || !Enabled && Value > 0;

        public void Init(bool enabled)
        {
            Enabled = enabled;
            Value = enabled ? 1f : 0f;
        }

        public void Init(bool enabled, float speed)
        {
            Enabled = enabled;
            Value = enabled ? 1f : 0f;
            Speed = speed;
        }

        public void Init(bool enabled, float value, float speed)
        {
            Enabled = enabled;
            Value = value;
            Speed = speed;
        }

        public bool Update(float dt)
        {
            var speed = (Enabled ? 1 : -1) * dt * Speed;
            Value = Mathf.Clamp01(Value + speed);

            var result = keepWaiting;
            _forceUpdate = false;
            return result;
        }

        public Enabler SetEnabled(bool isEnabled, bool instant = false)
        {
            _forceUpdate = true;
            Enabled = isEnabled;
            if (instant)
                Value = isEnabled ? 1 : 0;
            return this;
        }
    }
}
