using System;
using System.Collections;
using System.Collections.Generic;

public interface IMediator
{
    public bool IsUIBusy { get; }
    public bool IsGame { get; }
    public bool IsCheck { get; }

    public event Action OnReset;
    public event Action OnTimeDelayEnd;
    public event Action<List<bool>> OnResultReady;
    public event Action<List<int>> OnSetWorkingList;
    public event Action<List<int>> OnSetSortingList;

    public event Action OnStart;
    public event Action<List<int>> OnCheck;

    public void SetWorkingList(List<int> list);
    public void SetSortingList(List<int> list);

    public void TimeDelayEnd();

    public void Reset();
    public void Start();
    public void SetUIBusy(bool state);
    public void Check(List<int> list);
    public void SetResult(List<bool> list);
}

public class CUIMediator : IMediator
{
    private bool _uiBusy;
    private Core _core;

    private List<int> _workingList;
    private List<int> _sortingList;
    private List<int> userList;

    public bool IsGame => _core.IsGame;
    public bool IsCheck => _core.IsCheck;
    public bool IsUIBusy => _uiBusy;

    #region CoreActions
    public event Action OnReset;
    public event Action OnTimeDelayEnd;
    public event Action<List<bool>> OnResultReady;
    public event Action<List<int>> OnSetWorkingList;
    public event Action<List<int>> OnSetSortingList;
    #endregion

    #region UIActions
    public event Action<List<int>> OnSetUserList;
    public event Action OnStart;
    public event Action<List<int>> OnCheck;
    #endregion

    public CUIMediator()
    {
        _workingList = new List<int>();
        _sortingList = new List<int>();
    }

    public void Init(Core core)
    {
        _core = core;
    }

    public void Reset()
    {
        OnReset?.Invoke();
    }

    public void SetWorkingList(List<int> list)
    {
        _workingList.Clear();
        _workingList.AddRange(list);
        OnSetWorkingList?.Invoke(_workingList);
    }

    public void SetSortingList(List<int> list)
    {
        _sortingList.Clear();
        _sortingList.AddRange(list);
        OnSetSortingList?.Invoke(_sortingList);
    }

    public void TimeDelayEnd()
    {
        OnTimeDelayEnd?.Invoke();
    }

    public void Start()
    {
        OnStart?.Invoke();
    }

    public void Check(List<int> list)
    {
        OnCheck?.Invoke(list);
    }

    public void SetResult(List<bool> list)
    {
        OnResultReady?.Invoke(list);
    }

    public void SetUIBusy(bool state)
    {
        _uiBusy = state;
    }
}
