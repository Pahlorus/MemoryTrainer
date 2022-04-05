using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public class CanvasGroupAnim : BaseAnimModule
{
    private Sequence _alphaSeq;

    public CanvasGroup canvasGroup;

    #region Fade
    public async UniTask Fade(float alphaFin, float alphaStart, float durationTime, CancellationToken ct, bool isTimeScaleIgnore = false, Ease ease = Ease.Linear)
    {
        if (canvasGroup == null) return;
        if (ct.IsCancellationRequested) return;
        canvasGroup.alpha = alphaStart;
        await canvasGroup.DOFade(alphaFin, durationTime).SetUpdate(isTimeScaleIgnore).SetEase(ease).AsyncWaitForCompletion();
    }

    public async UniTask Fade(SFadeAnimData data, float durationTime, CancellationToken ct, bool isTimeScaleIgnore = false, Ease ease = Ease.Linear)
    {
        await Fade(data.finAlpha, data.startAlpha, durationTime, ct, isTimeScaleIgnore, ease);
    }

    public async UniTask Fade(FadeAnimData data, float durationTime, CancellationToken ct, bool isTimeScaleIgnore = false, Ease ease = Ease.Linear)
    {
        await Fade(data.finAlpha, data.startAlpha, durationTime, ct, isTimeScaleIgnore, ease);
    }

    public async UniTask Fade(bool up, FadeAnimData data, CancellationToken ct, bool isTimeScaleIgnore = false)
    {
        var finAlpha = up ? data.finAlpha : data.startAlpha;
        var startAlpha = up ? data.startAlpha : data.finAlpha;
        var duration = up ? data.alphaUpDuration : data.alphaDownDuration;
        var ease = up ? data.upEase : data.downEase;
        await Fade(finAlpha, startAlpha, duration, ct, isTimeScaleIgnore, ease);
    }

    public void SetInstantFade(float finAlpha)
    {
        FadeAnimReset();
        if (canvasGroup != null) canvasGroup.alpha = finAlpha;
    }

    public void SetInstantFade(bool up, FadeAnimData data)
    {
        var finAlpha = up ? data.finAlpha : data.startAlpha;
        SetInstantFade(finAlpha);
    }

    #endregion

    public async UniTask Blink(FadeAnimData data, CancellationToken ct)
    {
        await Blink(data.finAlpha, data.startAlpha, data.alphaUpDuration, data.alphaDownDuration, ct, data.upEase, data.downEase);
    }

    public async UniTask Blink(float finAlpha, float startAlpha, float alphaUpDuration, float alphaDownDuration, CancellationToken ct, Ease upEase = Ease.Linear, Ease downEase = Ease.Linear)
    {
        if (ct.IsCancellationRequested) return;
        if (_alphaSeq != default) return;
        canvasGroup.alpha = startAlpha;
        var tweenerAlphaUp = canvasGroup.DOFade(finAlpha, alphaUpDuration).SetEase(upEase);
        var tweenerAlphaDown = canvasGroup.DOFade(startAlpha, alphaDownDuration).SetEase(downEase);
        _alphaSeq = DOTween.Sequence();
        _alphaSeq.Append(tweenerAlphaUp);
        _alphaSeq.Append(tweenerAlphaDown);
        await _alphaSeq.AsyncWaitForCompletion();
        _alphaSeq = default;
    }

    private void FadeAnimReset()
    {
        canvasGroup?.DOKill();
        if (_alphaSeq != null) { _alphaSeq.Kill(); _alphaSeq = default; }
    }


    public override void AnimReset()
    {
        FadeAnimReset();
        base.AnimReset();
    }
}
