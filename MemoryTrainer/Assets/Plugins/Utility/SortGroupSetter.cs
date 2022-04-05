using UnityEngine;
using UnityEngine.Rendering;
using Utility.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utility
{
    public class SortGroupSetter : MonoBehaviour
    {

#if UNITY_EDITOR

        [CustomEditor(typeof(SortGroupSetter))]
        public class SortGroupSetterEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                if (GUILayout.Button("Apply"))
                {
                    var sortGroupSetter = target as SortGroupSetter;
                    if (sortGroupSetter._useCanvas)
                    {
                        if (sortGroupSetter.TryGetComponent(out Canvas canvas))
                            Undo.RecordObject(canvas, "Applying sortGroup");
                        else
                            canvas = Undo.AddComponent<Canvas>(sortGroupSetter.gameObject);
                        while (UnityEditorInternal.ComponentUtility.MoveComponentUp(canvas)) { }
                    }
                    else
                    {
                        if (sortGroupSetter.TryGetComponent(out SortingGroup sortGroup))
                            Undo.RecordObject(sortGroup, "Applying sortGroup");
                        else
                            sortGroup = Undo.AddComponent<SortingGroup>(sortGroupSetter.gameObject);
                        while (UnityEditorInternal.ComponentUtility.MoveComponentUp(sortGroup)) { }
                    }

                    sortGroupSetter.Sort();
                }
            }
        }

#endif

        [SerializeField, Require] SortGroup _sortGroup;
        [Range(-32768, 32767)] public int Offset;
        [SerializeField] private bool _useCanvas;

        private void Awake()
        {
            Sort();
            Destroy(this);
        }

        private void Sort()
        {
            if (_useCanvas)
            {
                if (!TryGetComponent(out Canvas canvas))
                    gameObject.AddComponent<Canvas>();

                canvas.overrideSorting = true;
                canvas.sortingLayerID = _sortGroup.SortingLayer;
                canvas.sortingOrder = _sortGroup.SortOrder + Offset;
            }
            else
            {
                if (!TryGetComponent(out SortingGroup sortingGroup))
                    gameObject.AddComponent<SortingGroup>();

                sortingGroup.sortingLayerID = _sortGroup.SortingLayer;
                sortingGroup.sortingOrder = _sortGroup.SortOrder + Offset;
            }
        }
    }
}
