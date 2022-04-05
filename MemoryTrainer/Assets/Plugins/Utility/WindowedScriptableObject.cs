#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

using System;
using UnityEngine;

namespace Utility
{
    public abstract class WindowedScriptableObject : ScriptableObject
    {
        [Serializable]
        private struct WindowData
        {
            public Rect Rect;
        }

#if UNITY_EDITOR
        private static int posOffset = 0;

        [OnOpenAsset(1000)]
        private static bool Open(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is WindowedScriptableObject scriptableObject)
            {
                Open(scriptableObject);

                return true;
            }
            else
                return false;
        }

        protected static void OpenFirst<T>() where T : WindowedScriptableObject
        {
            foreach (var scriptableObject in PropertyDrawerUtils.GetAllInstances<T>())
            {
                if (scriptableObject != null)
                    Open(scriptableObject);
            }
        }

        private static void Open(WindowedScriptableObject scriptableObject)
        {
            var window = EditorWindow.CreateWindow<Window>();
            window.Init(scriptableObject);
            window.titleContent = new GUIContent(scriptableObject.name);
            window.Show();
            window.Focus();
            window.DataKey = $"{nameof(WindowedScriptableObject)}.{scriptableObject.GetType().Name}";
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

        private class Window : EditorWindow
        {
            [SerializeField] private WindowedScriptableObject _windowedScriptableObject;
            private Editor _editor;
            public string DataKey;
            [SerializeField] private Vector2 _scroll;

            public void Init(WindowedScriptableObject scriptableObject)
            {
                _windowedScriptableObject = scriptableObject;
                InitEditor();
            }

            private void OnEnable()
            {
                InitEditor();
            }

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

            private void InitEditor()
            {
                if (_windowedScriptableObject != null)
                {
                    _editor = Editor.CreateEditor(_windowedScriptableObject);
                }
            }

            private void OnGUI()
            {
                EditorGUI.indentLevel++;
                _scroll = EditorGUILayout.BeginScrollView(_scroll);
                _editor.OnInspectorGUI();
                EditorGUILayout.EndScrollView();
                EditorGUI.indentLevel--;
            }
        }
#endif
    }
}