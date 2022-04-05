namespace Utility
{
    using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    using UnityEngine;

    using Utility.Attributes;

    [CreateAssetMenu(fileName = nameof(SortGroup), menuName = "ScriptableObjects/Utility/" + nameof(SortGroup))]
    public sealed class SortGroup : ScriptableObject
    {
        [SortingLayer] public int SortingLayer;
        [Range(-32768, 32767)] public int SortOrder;
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SortGroup))]
    public class SortGroupEditor : Editor
    {
        public bool Small;

        private SerializedProperty _sortingLayer;
        private SerializedProperty _sortOrder;

        private void OnEnable()
        {
            _sortingLayer = serializedObject.FindProperty(nameof(SortGroup.SortingLayer));
            _sortOrder = serializedObject.FindProperty(nameof(SortGroup.SortOrder));
        }

        public override void OnInspectorGUI()
        {
            if (Small)
            {
                EditorGUILayout.PropertyField(_sortingLayer, GUIContent.none);
                EditorGUILayout.PropertyField(_sortOrder, GUIContent.none);
            }
            else
            {
                EditorGUILayout.PropertyField(_sortingLayer);
                EditorGUILayout.PropertyField(_sortOrder);
                if (GUILayout.Button("Edit groups"))
                {
                    var window = EditorWindow.GetWindow<SortGroupWindow>(true, target.name, true);
                    window.Refresh();
                    window.ShowModalUtility();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

    public class SortGroupWindow : EditorWindow
    {
        private List<SortGroup> _sortGroups = new List<SortGroup>();

        public void Refresh()
        {
            _sortGroups.Clear();
            _sortGroups.AddRange(GetAllInstances());
            _sortGroups.Sort((x, y) =>
            {
                if (x.SortingLayer == y.SortingLayer) return x.SortOrder.CompareTo(y.SortOrder);
                else return SortingLayer.GetLayerValueFromID(x.SortingLayer).CompareTo(SortingLayer.GetLayerValueFromID(y.SortingLayer));
            });
        }

        private IEnumerable<SortGroup> GetAllInstances()
        {
            var guids = AssetDatabase.FindAssets("t:" + nameof(SortGroup));
            int length = guids.Length;
            for (int i = 0; i < length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var objects = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var obj in objects)
                {
                    if (obj is SortGroup sortGroup)
                        yield return sortGroup;
                }
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Refresh"))
                Refresh();

            foreach (var group in _sortGroups)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel(group.name);

                var a = Editor.CreateEditor(group) as SortGroupEditor;
                a.Small = true;
                a.OnInspectorGUI();

                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
}