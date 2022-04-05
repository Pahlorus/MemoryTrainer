namespace Utility
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    using System.Collections.Generic;
    using static Utility.PlayerPrefsExtensions;

    public static class PropertyDrawerUtils
    {
#if UNITY_EDITOR
        public static void Prefs(string key, PrefsAction action, ref bool value)
        {
            switch (action)
            {
                case PrefsAction.Get:
                    value = EditorPrefs.GetBool(key, value);
                    break;
                case PrefsAction.Set:
                    EditorPrefs.SetBool(key, value);
                    break;
            }
        }

        public static void Prefs(string key, PrefsAction action, ref float value)
        {
            switch (action)
            {
                case PrefsAction.Get:
                    value = EditorPrefs.GetFloat(key, value);
                    break;
                case PrefsAction.Set:
                    EditorPrefs.SetFloat(key, value);
                    break;
            }
        }

        public static void Prefs(string key, PrefsAction action, ref int value)
        {
            switch (action)
            {
                case PrefsAction.Get:
                    value = EditorPrefs.GetInt(key, value);
                    break;
                case PrefsAction.Set:
                    EditorPrefs.SetInt(key, value);
                    break;
            }
        }

        public static void Prefs(string key, PrefsAction action, ref string value)
        {
            switch (action)
            {
                case PrefsAction.Get:
                    value = EditorPrefs.GetString(key, value);
                    break;
                case PrefsAction.Set:
                    EditorPrefs.SetString(key, value);
                    break;
            }
        }

        public static void PrefsSORef<T>(string key, PrefsAction action, ref T value) where T : ScriptableObject
        {
            switch (action)
            {
                case PrefsAction.Get:
                    {
                        var guid = EditorPrefs.GetString(key, null);
                        if (!string.IsNullOrEmpty(guid))
                        {
                            value = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                        }
                        break;
                    }
                case PrefsAction.Set:
                    {
                        if (value == null)
                        {
                            EditorPrefs.SetString(key, string.Empty);
                            return;
                        }

                        var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                        EditorPrefs.SetString(key, guid);
                        break;
                    }
            }
        }

        public static void PrefsSORef<T>(ref string data, PrefsAction action, ref T value) where T : ScriptableObject
        {
            switch (action)
            {
                case PrefsAction.Get:
                    {
                        var guid = data;
                        if (!string.IsNullOrEmpty(guid))
                        {
                            value = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                        }
                        break;
                    }
                case PrefsAction.Set:
                    {
                        if (value == null)
                        {
                            data = string.Empty;
                            return;
                        }

                        var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                        data = guid;
                        break;
                    }
            }
        }

        public static bool DrawPropertyField(this SerializedProperty serializedProperty, Rect rect, int charCount = 3, string label = null)
        {
            if (charCount == 0)
            {
                return EditorGUI.PropertyField(rect, serializedProperty, GUIContent.none);
            }    
            var name = label ?? serializedProperty.name;
            var shortName = name.Substring(0, Mathf.Min(name.Length, charCount));
            var labelContent = EditorGUIUtility.TrTextContent(shortName, serializedProperty.name);
            var labelRect = CutRectLeft(ref rect, EditorGUIUtility.singleLineHeight * 2);
            EditorGUI.LabelField(labelRect, labelContent, GUIContent.none);
            return EditorGUI.PropertyField(rect, serializedProperty, GUIContent.none);
        }

        public static void DeleteArrayElementAtIndexExplicit(this SerializedProperty serializedProperty, int index)
        {
            var elementProperty = serializedProperty.GetArrayElementAtIndex(index);
            if (elementProperty.propertyType == SerializedPropertyType.ObjectReference && elementProperty.objectReferenceValue != null)
                elementProperty.objectReferenceValue = null;
            serializedProperty.DeleteArrayElementAtIndex(index);
        }

        public static void DrawLabel(string name, float value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(name, value.ToString("N2"));
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawLabel(string name, float value, float baseValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(name, value.ToString("N2"));
            EditorGUILayout.TextField((value / baseValue).ToString("P2"));
            EditorGUILayout.EndHorizontal();
        }


        public static void SplitVericalLayout()
        {
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(2));
            EditorGUI.DrawRect(rect, Color.white);
        }

        public static void SplitVericalLayout(GUILayoutOption option)
        {
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(2), option);
            EditorGUI.DrawRect(rect, Color.white);
        }



        [MenuItem("Assets/Tools/Set Dirty")]
        private static void SetDirty()
        {
            foreach (Object o in Selection.objects)
            {
                EditorUtility.SetDirty(o);
            }
        }

        public static IEnumerable<T> GetAllInstances<T>() where T : Object
        {
            var type = typeof(T);
            var guids = AssetDatabase.FindAssets("t:" + type.Name);
            int length = guids.Length;
            for (int i = 0; i < length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var objects = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var obj in objects)
                {
                    if (obj != null && obj.GetType() == type)
                        yield return obj as T;
                }
            }
        }

        public static T GetFirst<T>() where T : WindowedScriptableObject
        {
            foreach (var scriptableObject in GetAllInstances<T>())
            {
                if (scriptableObject != null)
                {
                    return scriptableObject;
                }
            }
            return null;
        }

        public static void AdditionalInfoAboutPlayMode()
        {
            EditorGUILayout.HelpBox("Additional info available in play mode", MessageType.Info);
        }
#endif

        public static void SplitRectVertical(this Rect rect, out Rect top, out Rect bottom, float proportion = 0.5f)
        {
            var height = rect.height * proportion;

            top = new Rect(rect.x, rect.y, rect.width, height);
            bottom = new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }
        public static void SplitRecHorisontal(this Rect rect, out Rect left, out Rect right, float proportion = 0.5f)
        {
            var width = rect.width * proportion;

            left = new Rect(rect.x, rect.y, width, rect.height);
            right = new Rect(rect.x + width, rect.y, rect.width - width, rect.height);
        }
        public static Rect CutRectLeft(ref Rect rect, float size)
        {
            var r = new Rect(rect.x, rect.y, size, rect.height);
            rect.x += size;
            rect.width -= size;
            return r;
        }
        public static Rect CutRectRight(ref Rect rect, float size)
        {
            var r = new Rect(rect.x + rect.width - size, rect.y, size, rect.height);
            rect.width -= size;
            return r;
        }
        public static Rect CutRectTop(ref Rect rect, float size)
        {
            var r = new Rect(rect.x, rect.y, rect.width, size);
            rect.y -= size;
            rect.height -= size;
            return r;
        }
        public static Rect CutRectBottom(ref Rect rect, float size)
        {
            var r = new Rect(rect.x, rect.y + rect.height + size, rect.width, size);
            rect.width -= size;
            return r;
        }
    }
}