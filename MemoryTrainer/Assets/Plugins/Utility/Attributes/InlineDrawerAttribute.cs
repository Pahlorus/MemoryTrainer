#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Utility.Attributes
{
    public class InlineDrawerAttribute : MultiPropertyAttribute
    {
        public readonly GUIContent[] LabelOverrides;

        public InlineDrawerAttribute(params string[] labelOverrides)
        {
            var length = labelOverrides?.Length ?? 0;
            LabelOverrides = new GUIContent[length];
            for (int i = 0; i < length; i++)
                LabelOverrides[i] = new GUIContent(labelOverrides[i]);
        }


#if UNITY_EDITOR
        internal override float? GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (!EditorGUIUtility.wideMode)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.NextVisible(true);
            EditorGUI.MultiPropertyField(position, LabelOverrides, property, label);
        }
#endif
    }
}
