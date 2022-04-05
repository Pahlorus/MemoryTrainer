
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PrefabsList), menuName = "ScriptableObjects/Data/" + nameof(PrefabsList))]
public class PrefabsList : ScriptableObject
{
    public Core corePref;
    public UIManager uiManagerPref;
    public SettingsPanel settingsPanelPref;
}
