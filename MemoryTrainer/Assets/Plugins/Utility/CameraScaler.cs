namespace Utility
{
    using UnityEngine;

    public class CameraScaler : MonoBehaviour
    {
        public const float DEFAULT_ASPECT = 16f / 9;
        private static readonly Color _clear = Color.clear;

        [SerializeField] private Camera _camera;

        private void OnPreRender()
        {
            GL.Viewport(new Rect(Vector2.zero, UtilityExtensions.GetResolution()));
            GL.Clear(true, true, _clear);
        }

        private void Update()
        {
            var screenAspect = (float)Screen.width / Screen.height;


            if (screenAspect < DEFAULT_ASPECT)
            {
                var adjust = screenAspect / DEFAULT_ASPECT;
                _camera.rect = new Rect(0, (1 - adjust) * 0.5f, 1, adjust);
            }
            else
            {
                var adjust = DEFAULT_ASPECT / screenAspect;
                _camera.rect = new Rect((1 - adjust) * 0.5f, 0, adjust, 1);
            }

        }
    }
}