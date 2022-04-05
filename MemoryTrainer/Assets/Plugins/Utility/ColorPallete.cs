#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = nameof(ColorPallete), menuName = "ScriptableObjects/Utility/" + nameof(ColorPallete))]
    public class ColorPallete : WindowedScriptableObject
    {
        [SerializeField] ColorPalleteDictionary _pallete;

        public bool TryGetColor(string key, out ColorInfo colorInfo)
        {
            return _pallete.TryGetValue(key, out colorInfo);
        }

        [Serializable] private class ColorPalleteDictionary : SerializableDictionaryBase<string, ColorInfo> { }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(ColorInfo))]
        public class ColorInfoDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var colorProp = property.FindPropertyRelative(nameof(ColorInfo.Color32));
                var hexProp = property.FindPropertyRelative(nameof(ColorInfo.Hex));

                EditorGUI.PropertyField(position, colorProp, label);
                hexProp.stringValue = ColorUtility.ToHtmlStringRGBA(colorProp.colorValue);
            }
        }
#endif
        [Serializable]
        public struct ColorInfo
        {
            public Color32 Color32;
            public string Hex;

            public override string ToString()
            {
                return Hex;
            }
        }
    }
}