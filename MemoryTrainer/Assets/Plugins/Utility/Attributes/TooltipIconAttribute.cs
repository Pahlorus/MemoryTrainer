#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Utility.Attributes
{
    public class TooltipIconAttribute : MultiPropertyAttribute
    {
        private static GUIContent _guiContent;
        private readonly string _toolTip;

        public TooltipIconAttribute(string toolTip)
        {
            _toolTip = toolTip;
        }

#if UNITY_EDITOR

        internal override void OnPreGUI(ref Rect position, SerializedProperty property, ref GUIContent label)
        {
            if (_guiContent == null)
                _guiContent = new GUIContent(EditorGUIUtility.IconContent("_Help"));

            _guiContent.text = label.text;
            _guiContent.tooltip = _toolTip;

            var labelPos = new Rect(position);
            labelPos.x -= 2;
            var width = EditorGUIUtility.labelWidth;
            GUI.Label(labelPos, _guiContent);

            position.x += width;
            position.width -= width;
            label = GUIContent.none;
        }
#endif
    }
}
