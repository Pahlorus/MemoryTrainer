#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Utility.Attributes
{
    public class ChangedAttribute : MultiPropertyAttribute
    {
        private Color _color;

#if UNITY_EDITOR
        internal override void OnPreGUI(ref Rect position, SerializedProperty property, ref GUIContent label)
        {
            var isChanged = false;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer: isChanged = property.intValue != default; break;
                case SerializedPropertyType.Boolean: isChanged = property.boolValue != default; break;
                case SerializedPropertyType.Float: isChanged = property.floatValue != default; break;
                case SerializedPropertyType.String: isChanged = property.stringValue != default; break;
                case SerializedPropertyType.Color: isChanged = property.colorValue != default; break;
                case SerializedPropertyType.ObjectReference: isChanged = property.objectReferenceValue != default; break;
                case SerializedPropertyType.LayerMask: isChanged = property.intValue != default; break;
                case SerializedPropertyType.Enum: isChanged = property.enumValueIndex != default; break;
                case SerializedPropertyType.Vector2: isChanged = property.vector2Value != default; break;
                case SerializedPropertyType.Vector3: isChanged = property.vector3Value != default; break;
                case SerializedPropertyType.Vector4: isChanged = property.vector4Value != default; break;
                case SerializedPropertyType.Rect: isChanged = property.rectValue != default; break;
                case SerializedPropertyType.ArraySize: isChanged = property.arraySize != default; break;
                case SerializedPropertyType.Character: isChanged = property.intValue != default; break;
                case SerializedPropertyType.Bounds: isChanged = property.boundsValue != default; break;
                case SerializedPropertyType.Quaternion: isChanged = property.quaternionValue != default; break;
                case SerializedPropertyType.ExposedReference: isChanged = property.exposedReferenceValue != default; break;
                case SerializedPropertyType.FixedBufferSize: isChanged = property.fixedBufferSize != default; break;
                case SerializedPropertyType.Vector2Int: isChanged = property.vector2IntValue != default; break;
                case SerializedPropertyType.Vector3Int: isChanged = property.vector3IntValue != default; break;
                case SerializedPropertyType.RectInt: isChanged = property.rectIntValue.position != default || property.rectIntValue.size != default; break;
                case SerializedPropertyType.BoundsInt: isChanged = property.boundsIntValue != default; break;
            }
            _color = GUI.color;
            if (isChanged)
                GUI.color = Color.green;
        }

        internal override void OnPostGUI(Rect position, SerializedProperty property)
        {
            GUI.color = _color;
        }
#endif
    }
}
