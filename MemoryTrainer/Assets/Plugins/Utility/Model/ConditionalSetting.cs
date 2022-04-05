#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

using System;
using UnityEngine;
using Utility.Model;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConditionalSettingUsageAttribute : MultiPropertyAttribute
    {
        /// <summary>
        /// Override condition display name
        /// </summary>
        public string ConditionName = null;
        /// <summary>
        /// Override value display name
        /// </summary>
        public string ValueName = null;
        /// <summary>
        /// Set mask for expanding value based on FLAGS condition
        /// </summary>
        public int ExpandMask = -1;

        /// <summary>
        /// Set spectific values to expand
        /// </summary>
        public readonly int[] ExpandEnums;

        public ConditionalSettingUsageAttribute() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expandOn">Set spectific values to expand</param>
        public ConditionalSettingUsageAttribute(params int[] expandOn)
        {
            ExpandEnums = expandOn;
        }

#if UNITY_EDITOR
        public bool Comapare(int value)
        {
            if ((value & ExpandMask) == 0)
                return false;

            if (ExpandEnums == null)
                return true;

            return ExpandEnums.Contains(value);
        }

        private static GUIContent _conditionLabel = new GUIContent();
        private static GUIContent _valueLabel = new GUIContent();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var conditionProp = property.FindPropertyRelative(nameof(ConditionalSetting<Enum, int>.Condition));
            var valueProp = property.FindPropertyRelative(nameof(ConditionalSetting<Enum, int>.Value));

            _conditionLabel.text = ConditionName;
            _valueLabel.text = ValueName;

            EditorGUI.BeginProperty(position, label, property);
            {
                var rect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                rect.width /= 2;

                var labelStyle = EditorStyles.label;
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                EditorGUIUtility.labelWidth = labelStyle.CalcSize(_conditionLabel).x;
                EditorGUI.PropertyField(rect, conditionProp, _conditionLabel);
                if (Comapare(conditionProp.intValue))
                {
                    rect.x += rect.width;
                    EditorGUIUtility.labelWidth = labelStyle.CalcSize(_valueLabel).x;
                    EditorGUI.PropertyField(rect, valueProp, _valueLabel);
                }
                EditorGUI.indentLevel = indent;
            }
            EditorGUI.EndProperty();
        }
#endif
    }
}

namespace Utility.Model
{
    /// <summary>
    /// Value can be hidden based on Condition.
    /// That behaviour contolled by ConditionalSettingUsageAttribute.
    /// </summary>
    /// <typeparam name="E">Enum type for first setting</typeparam>
    /// <typeparam name="V">Value type for second setting</typeparam>
    [Serializable]
    public class ConditionalSetting<E, V> : ConditionalSetting where E : Enum
    {
        public E Condition;
        public V Value;
    }

    [Serializable]
    public class ConditionalSetting
    {
    }
}
