#if UNITY_EDITOR
using UnityEditor;
#endif

using System;

using UnityEngine;

using Utility.Attributes;

using UObject = UnityEngine.Object;

namespace Utility
{
    [Serializable]
    public abstract class PrefabInstance
    {
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(PrefabInstance), true)]
        public class PrefabInstanceDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var prefabProp = property.FindPropertyRelative("_prefab");
                EditorGUI.PropertyField(position, prefabProp, label);
            }
        }
#endif
    }

    [Serializable]
    public abstract class PrefabInstance<T> : PrefabInstance where T : UObject
    {
        [SerializeField, Require] private T _prefab;
        [NonSerialized] private T _instance;

        public T Get()
        {
            if (_instance == null)
                _instance = UObject.Instantiate(_prefab);
            return _instance;
        }
    }
}