namespace Utility.Instancing
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class MatPropBlockHolder : MonoBehaviour
    {
#if UNITY_EDITOR

        [CustomEditor(typeof(MatPropBlockHolder))]
        public class MatPropBlockHolderEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                var holder = target as MatPropBlockHolder;
                if (holder != null && holder.UseGetPropertyBlock && holder._Renderer is SpriteRenderer)
                {
                    EditorGUILayout.HelpBox("GetPropertyBlock on SpriRenderer breaks batching, but needed for texture assignment", MessageType.Warning);
                }

                DrawDefaultInspector();

                if (Application.IsPlaying(holder) && GUILayout.Button("Refresh"))
                {
                    holder.RequestRefresh();
                }
            }
        }


#endif


        [SerializeField] private Renderer _Renderer;
        public bool UseGetPropertyBlock;

        [NonSerialized] private bool _Inited = false;
        [NonSerialized] private MaterialPropertyBlock _MatPropBlock;

        public event Action<MaterialPropertyBlock> RefreshEvent;

        private bool _refreshNeeded;

        private void LateUpdate()
        {
            if (!Init() || !_refreshNeeded)
                return;

            _refreshNeeded = false;

            if (UseGetPropertyBlock)
                _Renderer.GetPropertyBlock(_MatPropBlock);
            RefreshEvent?.Invoke(_MatPropBlock);
            _Renderer.SetPropertyBlock(_MatPropBlock);
        }

        public bool Init()
        {
            if (_Inited)
                return true;

            if (_Renderer == null)
                return false;

            if (_MatPropBlock == null)
                _MatPropBlock = new MaterialPropertyBlock();

            _Inited = true;
            return true;
        }

        public void RequestRefresh()
        {
            _refreshNeeded = true;
        }
    }
}