namespace Utility.Attributes
{

    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
    using System.Reflection;

    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
                EditorGUI.LabelField(position, new GUIContent(property.displayName), new GUIContent("should be an integer(the layer id)"), EditorStyles.boldLabel);
            else
                SortingLayerField(position, label, property, EditorStyles.popup, EditorStyles.label);
        }

        public static void SortingLayerField(Rect position, GUIContent label, SerializedProperty layerID, GUIStyle style, GUIStyle labelStyle)
        {
            MethodInfo methodInfo = typeof(EditorGUI).GetMethod("SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(Rect), typeof(GUIContent), typeof(SerializedProperty), typeof(GUIStyle), typeof(GUIStyle) }, null);
            if (methodInfo != null)
            {
                object[] parameters = new object[] { position, label, layerID, style, labelStyle };
                try { methodInfo.Invoke(null, parameters); }
                catch (System.Exception) { }
            }
        }
    }

#endif

    public class SortingLayerAttribute : PropertyAttribute { }
}