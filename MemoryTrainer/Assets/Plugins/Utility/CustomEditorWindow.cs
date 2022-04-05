#if UNITY_EDITOR
using UnityEditor;
#endif

using System;

using UnityEngine;

namespace Utility
{

#if UNITY_EDITOR
    public abstract class CustomEditorWindow : EditorWindow { }

    public abstract class CustomEditorWindow<T> : CustomEditorWindow where T : CustomEditorWindow<T>
    {
        [Serializable]
        private struct WindowData
        {
            public Rect Rect;
        }

        private static int posOffset = 0;

        public static void Open(string name)
        {
            var window = CreateWindow<T>();
            window.titleContent = new GUIContent(name);
            window.Show();
            window.Focus();
            window.DataKey = $"{nameof(CustomEditorWindow)}.{name}";
            var posData = EditorPrefs.GetString(window.DataKey, null);
            if (!string.IsNullOrEmpty(posData))
            {
                var data = JsonUtility.FromJson<WindowData>(posData);
                window.position = data.Rect;
            }
            var rect = window.position;
            rect.position += Vector2.one * 20 * (5 + posOffset);
            window.position = rect;
            posOffset = (posOffset + 1) % 5;
        }

        public string DataKey;
        [SerializeField] private Vector2 _scroll;

        private void OnDisable()
        {
            if (!string.IsNullOrEmpty(DataKey))
            {
                var windowData = new WindowData
                {
                    Rect = position,
                };
                var data = JsonUtility.ToJson(windowData);
                EditorPrefs.SetString(DataKey, data);
            }
        }

        private void OnGUI()
        {
            EditorGUI.indentLevel++;
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            OnGUI_Internal();
            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel--;
        }

        protected abstract void OnGUI_Internal();
    }
#endif
}
