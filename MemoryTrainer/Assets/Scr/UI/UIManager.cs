using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private bool _isUIBusy;
    private IMediator _uiMediator;
    private SettingsPanel _settingsPanel;
    private CPool<Tile> _tilePool;
    private CPool<TileNumber> _tileSortingPool;
    private List<Tile> _tiles;
    private List<TileNumber> _sortingTiles;

    [SerializeField] private Button _startButton;
    [SerializeField] private Button _checkButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Transform _tilesContainerTr;
    [SerializeField] private Transform _sortTilesContainerTr;
    [SerializeField] private Messages _messages;

    [Header("Prefs")]
    [SerializeField] private Tile _tilePref;
    [SerializeField] private TileNumber _tileNumberPref;

    public bool IsUIBusy => _isUIBusy;

    private void Awake()
    {
        _sortingTiles = new List<TileNumber>();
        _tiles = new List<Tile>();
        _tilePool = new CPool<Tile>();
        _tileSortingPool = new CPool<TileNumber>();
    }
    private void Start()
    {
        _startButton.onClick.AddListener(_uiMediator.Start);
        _checkButton.onClick.AddListener(Check);
        _settingsButton.onClick.AddListener(SettingsButtonHandler);
        FillBoard();
        _messages.SetText(MessagesStates.Start);
    }

    public void Init(IMediator uiMediator, SettingsPanel settingsPanel)
    {
        _uiMediator = uiMediator;
        _settingsPanel = settingsPanel;
        _uiMediator.OnReset += ResetHandler;
        _uiMediator.OnSetWorkingList += SetWorkingListHandler;
        _uiMediator.OnSetSortingList += SetSortingListHandler;
        _uiMediator.OnTimeDelayEnd += TimeDelayEndHandler;
        _uiMediator.OnResultReady += ResultReadyHandler;
    }

    private void ResetHandler()
    {
        if (_tiles != default)
            foreach (var item in _tiles)
            {
                item.SetState(ETileState.Default);
            }
    }

    private void Check()
    {
        if (_uiMediator.IsGame && _uiMediator.IsCheck)
        {
            _uiMediator.Check(GetUserList());
        }
    }

    private void ResultReadyHandler(List<bool> list)
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i].SetState(list[i] ? ETileState.Correct : ETileState.Fault);
        }

        for (int i = 0; i < _sortingTiles.Count; i++)
        {
            _sortingTiles[i].SetActive(false);
        }
        _messages.SetText(IsWin() ? MessagesStates.Win : MessagesStates.Defeat);
        bool IsWin()
        {
            foreach (var res in list) if (!res) return false;
            return true;
        }

    }

    private void TimeDelayEndHandler()
    {
        RotateImitationAfterDelay().Forget();
    }

    private void SetSortingListHandler(List<int> list)
    {
        if (_sortingTiles.Count == 0 || list.Count == _sortingTiles.Count)
        {
            for (int i = 0; i < _sortingTiles.Count; i++)
            {
                var tile = _sortingTiles[i];
                tile.SetTile(list[i]);
                tile.SetActive(true);
            }
        }
        else throw new System.ArgumentOutOfRangeException();
    }

    private void SetWorkingListHandler(List<int> list)
    {
        if (_tiles.Count == 0 || list.Count == _tiles.Count)
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                _tiles[i].SetTile(list[i]);
            }
            _messages.SetText(MessagesStates.Remember);
        }
        else throw new System.ArgumentOutOfRangeException();
    }

    private List<int> GetUserList()
    {
        var list = new List<int>();
        for (int i = 0; i < _tiles.Count; i++) list.Add(_tiles[i].Number);
        return list;
    }

    public void FillBoard()
    {
        var settings = CSettings.Instance;
        var count = settings.GetTileCount();
        for (int i = 0; i < count; i++)
        {
            var tile = TileCreate(_tilesContainerTr);
            var numberTile = TileNumberCreate(_sortTilesContainerTr);
            _tiles.Add(tile);
            _sortingTiles.Add(numberTile);
        }
    }

    //pool is redundant for this implementation
    private Tile TileCreate(Transform parent)
    {
        return TileBaseCreate(parent, _tilePref, _tilePool);
    }

    private TileNumber TileNumberCreate(Transform parent)
    {
        return TileBaseCreate(parent, _tileNumberPref, _tileSortingPool);
    }

    public void TileReturnPool<T>(T tile)
    {
        switch (tile)
        {
            case TileNumber sortTile: _tileSortingPool.Enqueue(sortTile); break;
            case Tile mainTail: _tilePool.Enqueue(mainTail); break;
            case null:
            default: return;
        }
    }

    private T TileBaseCreate<T>(Transform parent, T pref, CPool<T> pool) where T : MonoBehaviour
    {
        T tile = default;
        var exist = pool.TryGetInstance(out tile);
        if (!exist)
        {
            tile = Instantiate(pref, parent);
            var itile = (ITile)tile;
            itile.Init(this);
        }
        else { }
        return tile;
    }

    private void SettingsButtonHandler()
    {
        if (_uiMediator.IsGame) return;
        _settingsPanel?.SwitchActive(true);
    }

    private async UniTask RotateImitationAfterDelay()
    {
        _uiMediator.SetUIBusy(true);
        var taskLst = new List<UniTask>();
        foreach (var item in _tiles)
        {
            var task = item.RotateImitationAnim();
            taskLst.Add(task);
        }
        await UniTask.WhenAll(taskLst);
        _uiMediator.SetUIBusy(false);
        _messages.SetText(MessagesStates.Check);

    }
}
