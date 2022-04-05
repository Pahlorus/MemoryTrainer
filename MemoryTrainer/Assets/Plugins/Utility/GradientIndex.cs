namespace Utility
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;

    [CustomPropertyDrawer(typeof(GradientIndex))]
    public class GradientIndexDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var halfHeight = position.height * 0.5f;

            var indexRect = new Rect(position.x, position.y, position.width, halfHeight);
            var gradientRect = new Rect(position.x, position.y + halfHeight, position.width, halfHeight);

            var indexProp = property.FindPropertyRelative(nameof(GradientIndex.Index));

            EditorGUI.PropertyField(indexRect, indexProp, label);

            if (property.serializedObject.targetObject is IGradienAtlasHolder gradienAtlasHolder)
            {
                var attributes = fieldInfo.GetCustomAttributes(typeof(GradientRefAttribute), true);
                if (attributes == null || attributes.Length == 0)
                    return;

                var atlasIndex = (attributes[0] as GradientRefAttribute).AtlasRefIndex;

                var gradientIndex = indexProp.intValue;

                var atlas = gradienAtlasHolder.GetAtlas(atlasIndex);
                if (atlas == null || atlas.Gradients == null || gradientIndex < 0 || gradientIndex >= atlas.Gradients.Length)
                    return;

                EditorGUI.indentLevel += 1;
                atlas.Gradients[gradientIndex] = EditorGUI.GradientField(gradientRect, "UndoNotWork", atlas.Gradients[gradientIndex]);
                EditorGUI.indentLevel -= 1;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }

#endif

    [Serializable]
    public struct GradientIndex
    {
        public int Index;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class GradientRefAttribute : PropertyAttribute
    {
        public int AtlasRefIndex;
        public GradientRefAttribute(int atlasRefIndex = 0)
        {
            AtlasRefIndex = atlasRefIndex;
        }
    }

    public interface IGradienAtlasHolder
    {
        GradientAtlas GetAtlas(int index);
    }
}