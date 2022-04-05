namespace Utility
{
    using UnityEditor;
    using UnityEngine;

    using Utility.Attributes;

#if UNITY_EDITOR

    [CustomEditor(typeof(GradientAtlas))]
    public class GradientAtlasEditor : Editor
    {
        public const int Width = 512;
        public const int Height = 512;

        [MenuItem("Assets/ScriptableObjects/GradientAtlas")]
        public static void CreateAtlas()
        {
            var so = ScriptableObjectUtility.CreateAsset<GradientAtlas>();

            var texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
            texture.name = "Gradients";
            so.Texture = texture;

            AssetDatabase.AddObjectToAsset(texture, so);

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(so));
        }

        public override void OnInspectorGUI()
        {
            var grad = target as GradientAtlas;

            var valid = true;

            if (grad.Gradients.Length * 2 > grad.Texture.height)
            {
                valid = false;
                EditorGUILayout.HelpBox($"Can't support more gradients", MessageType.Error);
            }

            DrawDefaultInspector();

            if (valid)
            {
                grad.Apply();
            }
        }
    }

#endif


    public class GradientAtlas : WindowedScriptableObject
    {
        [ShowOnly] public Texture2D Texture;
        public Gradient[] Gradients;

        public void Apply()
        {
            var width = Texture.width;
            var colorBuffer = new Color32[width * 2];
            for (int i = 0; i < Gradients.Length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var value = (Color32)Gradients[i].Evaluate((float)j / (width - 1));
                    colorBuffer[j] = value;
                    colorBuffer[j + width] = value;
                }
                Texture.SetPixels32(0, i * 2, width, 2, colorBuffer);
            }
            Texture.Apply();
        }

        public float GetY(int index)
        {
            return (float)(index * 2 + 1) / Texture.height;
        }
    }
}