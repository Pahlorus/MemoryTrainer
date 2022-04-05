namespace Utility.Instancing
{
    using UnityEngine;

    public class InstancedColor : MatPropBlockConsumer
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        public Color Color;
        private Color _lastColor;

        private void Update()
        {
            UpdateParameters();
        }

        private void UpdateParameters()
        {
            if (!Init())
                return;

            if (_lastColor == Color)
                return;
            _lastColor = Color;
            RequestRefresh();
        }

        protected override void OnRefresh(MaterialPropertyBlock mpb)
        {
            mpb.SetColor(ColorId, Color);
        }
    }
}

