using Cysharp.Threading.Tasks;

using System.Threading;

using UnityEngine;
using UnityEngine.UI;


public enum ETileState {Default, Fault, Correct }

public class Tile : TileBase
{
    private int _id;
    private CancellationTokenSource _source;
    [SerializeField] private Image _image;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _faultColor;
    [SerializeField] private Color _correctColor;

    [Header("Anim")]
    [SerializeField] private BaseAnimModule _animModule;
    [SerializeField] private ScaleAnimData _scaleDate;

    public void ClearText()
    {
        _text.text = string.Empty;
    }

    public void SetState(ETileState state)
    {
        switch(state)
        {
            case ETileState.Correct: _image.color = _correctColor; break;
            case ETileState.Fault: _image.color = _faultColor; break;
            default:
            case ETileState.Default:
                {
                    Number = -1;
                    Text.text = string.Empty;
                    _image.color = _defaultColor; 
                    break;
                }
        }
    }

    public void AnimReset()
    {
        _animModule.AnimReset();
        _animModule.SetInstantScale(to: false, _scaleDate);
        if (_source != null) _source.Cancel();
    }

    public async UniTask RotateImitationAnim()
    {
        _animModule.AnimReset();
        _animModule.SetInstantScale(to: false, _scaleDate);
        _source = new CancellationTokenSource();
        var ct = CancellationTokenSource.CreateLinkedTokenSource(transform.GetCancellationTokenOnDestroy(), _source.Token).Token;

        if (ct.IsCancellationRequested) return;
        await _animModule.Scale(up: true, _scaleDate, ct);
        SetState(ETileState.Default);
        if (ct.IsCancellationRequested) return;
        await _animModule.Scale(up: false, _scaleDate, ct);
    }
}
