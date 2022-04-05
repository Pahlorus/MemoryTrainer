#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;

namespace Utility
{

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MinMaxRange))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        private static readonly GUIContent[] _LABELS = new GUIContent[] { new GUIContent("Min"), new GUIContent("Max") };
        private static readonly float[] _VALUES = new float[2];

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.wideMode ? 18 : 38;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var minProp = property.FindPropertyRelative(nameof(MinMaxRange.Min));
            var maxProp = property.FindPropertyRelative(nameof(MinMaxRange.Max));
            var values = _VALUES;
            values[0] = minProp.floatValue;
            values[1] = maxProp.floatValue;
            var color = GUI.color;
            if (values[0] > values[1])
                GUI.color = Color.red;
            EditorGUI.MultiFloatField(position, label, _LABELS, _VALUES);
            GUI.color = color;
            minProp.floatValue = values[0];
            maxProp.floatValue = values[1];
        }
    }
#endif

    [Serializable]
    public struct MinMaxRange
    {
        public float Min;
        public float Max;

        public MinMaxRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Center
        { 
            get => (Max + Min) * 0.5f; 
            set
            {
                var center = Center;
                var diff = value - center;
                Min += diff;
                Max += diff;
            }
        }

        public float Size
        {
            get => Max - Min;
            set
            {
                var size = Size;
                var diff2 = (value - Size) * 0.5f;
                Max += diff2;
                Min -= diff2;
            }
        }

        /// <summary>
        /// Range from 0 to 1
        /// </summary>
        public static readonly MinMaxRange DEFAULT_01 = new MinMaxRange(0,1);
        /// <summary>
        /// Range from MinValue to MaxValue
        /// </summary>
        public static readonly MinMaxRange ALL = new MinMaxRange(float.MinValue, float.MaxValue);
        public static MinMaxRange Create(float min, float max) => new MinMaxRange(min, max);

        #region Intersection

        /// <summary>
        /// Returns true if value inside [Min Max] range
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InsideInclusive(float value)
        {
            return Min <= value && value <= Max;
        }

        /// <summary>
        /// Returns true if value inside (Min Max) range
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InsideExclusive(float value)
        {
            return Min < value && value < Max;
        }

        public bool ContainsInclusive(MinMaxRange range)
        {
            return InsideInclusive(range.Min) && InsideInclusive(range.Max);
        }

        public bool ContainsExclusive(MinMaxRange range)
        {
            return InsideExclusive(range.Min) && InsideExclusive(range.Max);
        }

        public bool Overlaps(MinMaxRange range)
        {
            return InsideInclusive(range.Min) || InsideExclusive(range.Max);
        }

        #endregion

        public override string ToString()
        {
            return $"Min:{Min} Max:{Max}";
        }
    }
}
