namespace Utility
{
    using UnityEngine;
    using UnityEngine.Events;

    public class AnimatorCallback : MonoBehaviour
    {
        public UnityEvent[] UnityEvents;

        public void Call(int id)
        {
            if (id >= 0 && id < UnityEvents.Length)
                UnityEvents[id].Invoke();
        }
    }
}