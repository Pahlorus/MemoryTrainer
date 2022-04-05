using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private Transform _canvasTr;
    [SerializeField] private PrefabsList _prefabs;

    private void Awake()
    {
        var settings = new CSettings();
        var uiMediator = new CUIMediator();

        var core = Instantiate(_prefabs.corePref);
        var uiManager = Instantiate(_prefabs.uiManagerPref, _canvasTr);
        var settingsPanel = Instantiate(_prefabs.settingsPanelPref, _canvasTr);

        settingsPanel.Init(settings);
        core.Init(uiMediator, settings);
        uiManager.Init(uiMediator, settingsPanel);
    }
}
