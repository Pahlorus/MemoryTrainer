namespace Utility
{
    using UnityEngine;
    using UnityEngine.Events;

#if UNITY_EDITOR
    using UnityEditor;


    [CustomEditor(typeof(EditorButton))]
    [CanEditMultipleObjects]
    public class EditorButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Invoke"))
                foreach (EditorButton button in targets)
                    button.OnClick.Invoke();
        }
    }

#endif

    public class EditorButton : MonoBehaviour
    {
        public UnityEvent OnClick;
    }
}