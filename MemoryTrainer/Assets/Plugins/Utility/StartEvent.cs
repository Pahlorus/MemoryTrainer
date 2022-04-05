using UnityEngine;
using UnityEngine.Events;

namespace Utility
{
    public class StartEvent : MonoBehaviour
    {
        public UnityEvent OnStart;

        private void Start()
        {
            OnStart.Invoke();
        }
    }
}
