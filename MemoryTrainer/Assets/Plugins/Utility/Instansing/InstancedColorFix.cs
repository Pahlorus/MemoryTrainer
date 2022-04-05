namespace Utility.Instancing
{
    using UnityEngine;

    public class InstancedColorFix : MatPropBlockConsumer
    {
        private static readonly int ColorFixId = Shader.PropertyToID("_ColorFix");
        [Range(-1, 1)]
        public float Contrast = 0;
        [Range(-1, 1)]
        public float Brightness = 0;
        [Range(-1, 1)]
        public float Saturation = 0;
        [Range(0, 1)]
        public float Alpha = 1;

        private Vector4 _lastColorFix;

        private void Update()
        {
            UpdateParameters();
        }

        private void UpdateParameters()
        {
            if (!Init())
                return;

            var colorFix = new Vector4(Contrast, Brightness, Saturation, 0);
            colorFix += Vector4.one;
            colorFix *= 0.5f;
            colorFix.w = Alpha;

            if (_lastColorFix == colorFix)
                return;
            _lastColorFix = colorFix;

            RequestRefresh();
        }

        protected override void OnRefresh(MaterialPropertyBlock mpb)
        {
            mpb.SetVector(ColorFixId, _lastColorFix);
        }
    }
}