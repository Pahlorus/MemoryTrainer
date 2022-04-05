
using DG.Tweening;
using UnityEngine;
using Utility;

public struct SScaleAnimData
{
    public float scaleUpDuration;
    public float scaleDownDuration;
    public Vector3 startScale;
    public Vector3 finScale;
    public Ease upEase;
    public Ease downEase;
}

[CreateAssetMenu(fileName = nameof(ScaleAnimData), menuName = "ScriptableObjects/Anim/" + nameof(ScaleAnimData))]
public class ScaleAnimData : WindowedScriptableObject
{
    [Header("Scale")]
    public float scaleUpDuration;
    public float scaleDownDuration;
    public Vector3 auxScale;
    public Vector3 startScale;
    public Vector3 finScale;
    public Ease upEase;
    public Ease downEase;
}
