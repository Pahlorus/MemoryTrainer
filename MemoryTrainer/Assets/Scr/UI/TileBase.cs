using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface ITile
{
    public int Number { get; set; }
    public void Init(UIManager uiManager);
    public void SetTile(int number);
}

public class TileBase : MonoBehaviour, ITile
{
    protected UIManager _uiManager;
    [SerializeField] protected TextMeshProUGUI _text;

    public int Number { get; set; }
    public TextMeshProUGUI Text => _text;
    public void SetTile(int number)
    {
        Number = number;
        Text.text = Number.ToString();
    }

    public void Init(UIManager uiManager)
    {
        Number = -1;
        _text.text = string.Empty;
        _uiManager = uiManager;
    }
}
