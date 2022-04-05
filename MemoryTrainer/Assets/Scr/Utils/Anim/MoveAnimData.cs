using DG.Tweening;
using UnityEngine;
using Utility;

public struct SMoveAnimData
{
    public float durationTimeDest;
    public float durationTimeStart;
    public Vector3 startPosition;
    public Vector3 destPosition;
    public Vector3 destPositionSecond;
    public Ease easeDest;
    public Ease easeStart;
}

[CreateAssetMenu(fileName = nameof(MoveAnimData), menuName = "ScriptableObjects/Anim/" + nameof(MoveAnimData))]

public class MoveAnimData : WindowedScriptableObject
{
    [Header("Move")]
    public float durationTimeDest;
    public float durationTimeStart;
    public Vector3 startPosition;
    public Vector3 destPosition;
    public Vector3 destPositionSecond;
    public Ease easeDest;
    public Ease easeStart;
}
