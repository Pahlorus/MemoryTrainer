namespace Utility
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TextureGrabber))]
    public class TextureGrabberEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Grab"))
            {
                var grabber = target as TextureGrabber;
                Texture texture;
                switch (grabber.Renderer)
                {
                    case SpriteRenderer spriteRenderer: texture = spriteRenderer.sprite.texture; break;
                    default: texture =  grabber.Renderer.sharedMaterial.GetTexture(grabber.TextureName); break;
                }
                grabber.TextureSetterEvent.Invoke(texture);
            }
        }
    }
#endif

    public class TextureGrabber : MonoBehaviour
    {
        public Renderer Renderer;
        public string TextureName = "_MainTex";

        public TextureSetter TextureSetterEvent;

        [Serializable]
        public class TextureSetter : UnityEvent<Texture> { }
    }
}