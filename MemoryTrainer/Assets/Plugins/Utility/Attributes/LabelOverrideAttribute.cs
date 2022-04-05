#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Utility.Attributes
{
    public class LabelOverrideAttribute : MultiPropertyAttribute
    {
        public readonly string Override;


        public LabelOverrideAttribute(string nameOverride)
        {
            Override = nameOverride;
        }

#if UNITY_EDITOR
        private static readonly GUIContent _GUI_CONTENT = new GUIContent();
        internal override void OnPreGUI(ref Rect position, SerializedProperty property, ref GUIContent label)
        {
            var tempGUIContent = _GUI_CONTENT;

            tempGUIContent.text = Override;
            tempGUIContent.tooltip = label.tooltip;
            tempGUIContent.image = label.image;

            label = tempGUIContent;
        }
#endif
    }
}
