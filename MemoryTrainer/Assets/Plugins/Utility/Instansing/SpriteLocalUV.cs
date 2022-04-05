namespace Utility.Instancing
{
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(SpriteLocalUV))]
    public class SpriteLocalUVEditor : Editor
    {
        private static readonly string _HELP =
    @"Sprite can be changed, but all sprites MUST:
1) Share same texture
2) Have FullRect mesh type
3) No Rotations and TightPacking

Also this script control MeshRenderer's scale";

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(_HELP, MessageType.Info);

            DrawDefaultInspector();
        }
    }

#endif

    public class SpriteLocalUV : MatPropBlockConsumer
    {
        private static readonly int MinMaxId = Shader.PropertyToID("_MinMax");

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private MeshRenderer _meshRenderer;

        public float ScaleOverride = 1;

        private Vector4 _minMax;
        private Vector2 _lastSize;
        private Sprite _lastSprite;

        private void LateUpdate()
        {
            UpdateParameters();
        }

        private bool Check()
        {
            if (_spriteRenderer == null || _spriteRenderer.sprite == null || _meshRenderer == null)
                return false;
            return true;
        }

        private void UpdateParameters()
        {
            if (!Check() || !Init())
                return;

            var sprite = _spriteRenderer.sprite;

            if (sprite != _lastSprite)
            {
                var texture = sprite.texture;
                var textureSize = new Vector2(texture.width, texture.height);
                var spriteRect = sprite.textureRect;
                _lastSize = spriteRect.size / sprite.pixelsPerUnit;
                _minMax = new Vector4(
                    Mathf.Min(spriteRect.xMin, spriteRect.xMax) / textureSize.x,
                    Mathf.Min(spriteRect.yMin, spriteRect.yMax) / textureSize.y,
                    Mathf.Max(spriteRect.xMin, spriteRect.xMax) / textureSize.x,
                    Mathf.Max(spriteRect.yMin, spriteRect.yMax) / textureSize.y);

                _lastSprite = sprite;
                RequestRefresh();
            }

            _meshRenderer.transform.localScale = (ScaleOverride * _lastSize).ToVector3();
        }

        protected override void OnInit()
        {
            _spriteRenderer.enabled = false;
            _meshRenderer.sharedMaterial = MaterialManager.Instance.GetSharedInstanceMaterial(_meshRenderer.sharedMaterial, _spriteRenderer.sprite.texture);
            _meshRenderer.enabled = true;
        }

        protected override void OnRefresh(MaterialPropertyBlock mpb)
        {
            mpb.SetVector(MinMaxId, _minMax);
        }
    }
}