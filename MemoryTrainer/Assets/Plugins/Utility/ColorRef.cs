#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[CreateAssetMenu(fileName = nameof(ColorRef), menuName = "ScriptableObjects/Utility/" + nameof(ColorRef))]
public class ColorRef : ScriptableObject
{
    public Color32 Color;
}