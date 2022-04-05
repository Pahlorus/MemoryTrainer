namespace Utility
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    public class CameraSetter : MonoBehaviour
    {
        public CameraSetEvent onStart;

        void Start()
        {
            var camera = Camera.main;

            onStart.Invoke(camera);
        }
    }

    [Serializable]
    public class CameraSetEvent : UnityEvent<Camera> { }
}