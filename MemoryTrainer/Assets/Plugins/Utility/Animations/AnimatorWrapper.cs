#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;

using UnityEngine;

using Utility.Attributes;

namespace Utility.Animations
{
    public class AnimatorWrapper : MonoBehaviour, IWaitableAnimation
    {
        [Serializable]
        private struct StateHash
        {
            [SerializeField] private string _state;
            private int _hash;

            public int Hash
            {
                get
                {
                    if (_hash == 0)
                        _hash = Animator.StringToHash(_state);
                    return _hash;
                }
            }


#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(StateHash))]
            public class StateHashDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    var prop = property.FindPropertyRelative(nameof(_state));
                    EditorGUI.PropertyField(position, prop, label);
                }
            }
#endif
        }

        [SerializeField, Require] private Animator _animator;

        private WaitAnimationHandle _waitAnimationHandle;

        [Header("Don't change in runtime")]
        [SerializeField] private List<StateHash> _states;
        private int _lastHash;

        bool IWaitableAnimation.keepWaiting 
        {
            get
            {
                var _currentState = _animator.GetCurrentAnimatorStateInfo(0);
                return
                    _currentState.shortNameHash != _lastHash ||
                    _currentState.normalizedTime < 1;          
            }
}

        private void Awake()
        {
            _waitAnimationHandle = new WaitAnimationHandle(this);
        }

        public WaitAnimationHandle PlayHash(int hash, bool instant = false)
        {
            _lastHash = hash;
            _animator.Play(_lastHash, -1, instant ? 1 : 0);
            return _waitAnimationHandle;
        }

        public WaitAnimationHandle PlayIndex(int index, bool instant = false)
        {
            _lastHash = _states[index].Hash;
            _animator.Play(_lastHash, -1, instant ? 1 : 0);
            return _waitAnimationHandle;
        }
    }
}
