using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    private bool _isGame;
    private bool _isCheck;

    private IMediator _uiMediator;
    private CSettings _settings;
    private List<int> modifiedList;
    public bool IsGame => _isGame;
    public bool IsCheck => _isCheck;

    public void Init(CUIMediator mediator, CSettings settings)
    {
        mediator.Init(this);
        _uiMediator = mediator;
        _settings = settings;
        _uiMediator.OnStart += StartHandler;
        _uiMediator.OnCheck += CheckHandler;
    }

    private void StartHandler()
    {
        if (_isGame) return;
        GameCycle().Forget();
    }

    private void CheckHandler(List<int> list)
    {
        modifiedList = new List<int>(list);
        _isCheck = false;
    }

    private List<int> GetRandomNumberList(int count)
    {
        var resultList = new List<int>();
        for (int i = 0; i < count; i++)
        {
            int rndValue = UnityEngine.Random.Range(0, 100);
            while (resultList.Contains(rndValue))
            {
                rndValue = UnityEngine.Random.Range(0, 100);
            }
            resultList.Add(rndValue);
        }
        return resultList;
    }

    public List<int> FillWorkingList()
    {
        var count = _settings.GetTileCount();
        var list = GetRandomNumberList(count);
        _uiMediator.SetWorkingList(list);
        return list;
    }

    public void FillSortingList(List<int> list)
    {
        var sortingList = new List<int>(list);
        sortingList.Sort();
        _uiMediator.SetSortingList(sortingList);
    }

    public async UniTask GameCycle()
    {
        GameReset();
        _isGame = true;
        var list = FillWorkingList();
        await UniTask.Delay((int)_settings.timeDelay * 1000);
        FillSortingList(list);
        _uiMediator.TimeDelayEnd();
        await UniTask.WaitWhile(() => _uiMediator.IsUIBusy);
        _isCheck = true;
        await UniTask.WaitWhile(() => _isCheck);
        CheckResult(list);
        _isGame = false;
    }

    private void CheckResult(List<int> originalList)
    {
        var result = new List<bool>();
        for (int i = 0; i < originalList.Count; i++)
        {
            result.Add(originalList[i] == modifiedList[i]);
        }
        _uiMediator.SetResult(result);
    }

    private void GameReset()
    {
        _isGame = false;
        _isCheck = false;
        _uiMediator.Reset();
    }
}

