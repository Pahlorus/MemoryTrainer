#if UNITY_EDITOR
using UnityEditor;
#endif  

using System;

using UnityEngine;

namespace Utility.Animations
{
    [CreateAssetMenu(fileName = nameof(FlyingTextData), menuName = "ScriptableObjects/Utility/" + nameof(FlyingTextData))]
    public class FlyingTextData : WindowedScriptableObject
    {

#if UNITY_EDITOR

        [CustomEditor(typeof(FlyingTextData))]
        [CanEditMultipleObjects]
        public class FlyingTextDataEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                if (Application.isPlaying)
                {
                    EditorGUILayout.Space();
                    LeftRight = EditorGUILayout.Vector2Field(nameof(LeftRight), LeftRight);
                    BotTop = EditorGUILayout.Vector2Field(nameof(BotTop), BotTop);
                }
            }
        }

#endif

        public float Time;

        public Gradient Color;
        public AnimationCurve Distance;
        public AnimationCurve Scale;

        public float RandomDistance;
        public float RandomAngle;

        [Header("MoveLimits")]
        public float leftBoardOffset;
        public float rightBoardOffset;
        public float topBoardOffset;
        public float bottomBoardOffset;

        public string Prefix;

        public static Vector2 LeftRight;
        public static Vector2 BotTop;

        public Vector3 Clamp(Vector3 value)
        {
            return new Vector3(
                Mathf.Clamp(value.x, LeftRight.x + leftBoardOffset, LeftRight.y - rightBoardOffset),
                Mathf.Clamp(value.y, BotTop.x + bottomBoardOffset, BotTop.y - topBoardOffset),
                value.z);
        }
    }
}
