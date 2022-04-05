using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public class SpriteRendererAnim : BaseAnimModule
{
    private Sequence _alphaSeq;
    public SpriteRenderer spriteRendedrer;

    private void SetAlpha(float alpha)
    {
        var color = spriteRendedrer.color;
        color.a = alpha;
        spriteRendedrer.color = color;
    }

    public async UniTask Fade(float alphaFin, float alphaStart, float durationTime, CancellationToken ct, bool isTimeScaleIgnore = false, Ease ease = Ease.Linear)
    {
        if (spriteRendedrer == null) return;
        if (ct.IsCancellationRequested) return;
        SetAlpha(alphaStart);
        await spriteRendedrer.DOFade(alphaFin, durationTime).SetUpdate(isTimeScaleIgnore).SetEase(ease).AsyncWaitForCompletion();
    }

    public async UniTask Fade(FadeAnimData data, float durationTime, CancellationToken ct, bool isTimeScaleIgnore = false, Ease ease = Ease.Linear)
    {
        await Fade( data.finAlpha, data.startAlpha, durationTime, ct, isTimeScaleIgnore, ease);
    }

    public async UniTask Fade(SFadeAnimData data, float durationTime, CancellationToken ct, bool isTimeScaleIgnore = false, Ease ease = Ease.Linear)
    {
        await Fade(data.finAlpha, data.startAlpha, durationTime, ct, isTimeScaleIgnore, ease);
    }

    public async UniTask Fade(bool up, FadeAnimData data, CancellationToken ct, bool isTimeScaleIgnore = false)
    {
        float finAlha = up ? data.finAlpha : data.startAlpha;
        float startAlha = up ? data.startAlpha : data.finAlpha;
        float duration = up ? data.alphaUpDuration : data.alphaDownDuration;
        Ease ease = up ? data.upEase : data.upEase;

        await Fade(finAlha, startAlha, duration, ct, isTimeScaleIgnore, ease);
    }

    public async UniTask Fade(bool up, SFadeAnimData data, CancellationToken ct, bool isTimeScaleIgnore = false)
    {
        float finAlha = up ? data.finAlpha : data.startAlpha;
        float startAlha = up ? data.startAlpha : data.finAlpha;
        float duration = up ? data.alphaUpDuration : data.alphaDownDuration;
        Ease ease = up ? data.upEase : data.upEase;

        await Fade(finAlha, startAlha, duration, ct, isTimeScaleIgnore, ease);
    }

    public void SetInstantFade(float finAlpha)
    {
        FadeAnimReset();
        if (spriteRendedrer != null) SetAlpha(finAlpha);
    }

    public async UniTask Blink(FadeAnimData data, CancellationToken ct)
    {
        await Blink(data.finAlpha, data.startAlpha, data.alphaUpDuration, data.alphaDownDuration, ct, data.upEase, data.downEase);
    }

    public async UniTask Blink(float finAlpha, float startAlpha, float alphaUpDuration, float alphaDownDuration, CancellationToken ct, Ease upEase = Ease.Linear, Ease downEase = Ease.Linear)
    {
        if (ct.IsCancellationRequested) return;
        if (_alphaSeq != default) return;
        SetAlpha(startAlpha);
        var tweenerAlphaUp = spriteRendedrer.DOFade(finAlpha, alphaUpDuration).SetEase(upEase);
        var tweenerAlphaDown = spriteRendedrer.DOFade(startAlpha, alphaDownDuration).SetEase(downEase);
        _alphaSeq = DOTween.Sequence();
        _alphaSeq.Append(tweenerAlphaUp);
        _alphaSeq.Append(tweenerAlphaDown);
        await _alphaSeq.AsyncWaitForCompletion();
        _alphaSeq = default;
    }

    private void FadeAnimReset()
    {
        spriteRendedrer?.DOKill();
        if (_alphaSeq != null) { _alphaSeq.Kill(); _alphaSeq = default; }
    }


    public override void AnimReset()
    {
        FadeAnimReset();
        base.AnimReset();
    }

}
