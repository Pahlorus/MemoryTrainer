using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.EditorGUIUtility;

namespace Utility.EditorExtensions
{
    public static class GUIExtensions
    {
        public static void IEditor(Object obj)
        {
            BeginVertical();
            var editor = UnityEditor.Editor.CreateEditor(obj);
            editor.OnInspectorGUI();
            Object.Destroy(editor);
            EndVertical();
        }

        public static void IEditor(Object obj, params GUILayoutOption[] options)
        {
            BeginVertical(options);
            var editor = UnityEditor.Editor.CreateEditor(obj);
            editor.OnInspectorGUI();
            Object.Destroy(editor);
            EndVertical();
        }

        public static void Label(string name, float width = 100f)
        {
            BeginHorizontal();
            LabelField(name, GUILayout.Width(width), GUILayout.Height(singleLineHeight));
            EndHorizontal();
        }

        public static void BLabel(string name, object content)
        {
            BeginHorizontal();
            LabelField(name, GUILayout.Height(singleLineHeight));
            SelectableLabel(content.ToString(), GUILayout.Height(singleLineHeight));
            EndHorizontal();
        }

        public static void SLabel(string name, object content, float width = 100f)
        {
            BeginHorizontal();
            LabelField(name, GUILayout.Width(width), GUILayout.Height(singleLineHeight));
            SelectableLabel(content.ToString(), GUILayout.Height(singleLineHeight));
            EndHorizontal();
        }

        public static void Int(string name, int value)
        {
            IntField(name, value);
        }
    }
}
#endif