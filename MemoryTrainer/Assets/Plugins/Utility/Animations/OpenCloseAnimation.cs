#if UNITY_EDITOR
using UnityEditor;
#endif

using System;

using UnityEngine;
using UnityEngine.Events;
using Utility.Attributes;

namespace Utility.Animations
{

#if UNITY_EDITOR

    [CustomEditor(typeof(OpenCloseAnimation), true)]
    [CanEditMultipleObjects]
    public class OpenCloseAnimationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Open"))
                {
                    SetOpened(true);
                }
                else if (GUILayout.Button("Close"))
                {
                    SetOpened(false);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void SetOpened(bool opened)
        {
            foreach (OpenCloseAnimation animation in targets)
                if (Application.IsPlaying(animation))
                    animation.SetOpened(opened);
        }
    }

#endif

    /// <summary>
    /// False - Closed; True - Opened
    /// </summary>
    [Serializable] public class OpenCloseEvent : UnityEvent<bool> { }

    public class OpenCloseAnimation : MonoBehaviour, IWaitableAnimation
    {
        private static readonly int _speedHash = Animator.StringToHash("Speed");
        [Require] public Animator Animator;
        public float Speed = 1;

        public OpenCloseEvent OpenCloseEvent;

        public bool keepWaiting { get; private set; }
        public WaitAnimationHandle WaitAnimationHandle { get; private set; }

        private bool _isOpened;

        protected virtual void Awake()
        {
            WaitAnimationHandle = new WaitAnimationHandle(this);
        }

        public WaitAnimationHandle SetOpened(bool opened, bool instant = false)
        {
            _isOpened = opened;
            Animator.SetFloat(_speedHash, (opened ? 1 : -1) * Speed);
            if (instant)
            {
                keepWaiting = false;
                Animator.Play(0, -1, opened ? 1 : 0);
                OpenCloseEvent.Invoke(opened);
            }
            else
            {
                keepWaiting = true;
                Animator.Play(0, -1, Mathf.Clamp01(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
            }

            return WaitAnimationHandle;
        }

        protected virtual void Animator_Opened()
        {
            if (_isOpened)
                keepWaiting = false;
            OpenCloseEvent.Invoke(true);
        }

        protected virtual void Animator_Closed()
        {
            if (!_isOpened)
                keepWaiting = false;
            OpenCloseEvent.Invoke(false);
        }
    }
}