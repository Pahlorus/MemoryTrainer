
using DG.Tweening;
using UnityEngine;
using Utility;

public struct SFadeAnimData
{
    public float startAlpha;
    public float finAlpha;
    public float alphaUpDuration;
    public float alphaDownDuration;
    public Ease upEase;
    public Ease downEase;
}

[CreateAssetMenu(fileName = nameof(FadeAnimData), menuName = "ScriptableObjects/Anim/" + nameof(FadeAnimData))]
public class FadeAnimData : WindowedScriptableObject
{
    [Header("Fade")]
    public float startAlpha;
    public float finAlpha;
    public float alphaUpDuration;
    public float alphaDownDuration;
    public Ease upEase;
    public Ease downEase;
}
