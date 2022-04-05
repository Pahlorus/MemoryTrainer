using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    private const string TimerValue = "TimerValue";
    //private float maxValue = 15;
    private float defaultValue = 5;
    private CSettings _settings;

    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _closeButton;

    public void Start()
    {
        _slider.onValueChanged.AddListener(OnTimerValueChange);
        _closeButton.onClick.AddListener(() => SwitchActive(false));
        SwitchActive(false);
    }

    public void Init(CSettings settings)
    {
        _settings = settings;
        SetTimer(GetFloatValue(TimerValue));
    }

    public void SwitchActive(bool active)
    {
        gameObject.SetActive(active);
    }

    private void OnTimerValueChange(float value)
    {
        int valueInt = (int)value;
        int stepSize = 1;
        value = (value - value % stepSize);
        SetTimer(value);
        SaveFloatValue(value, TimerValue);
    }

    private void SetTimer(float value)
    {
        _slider.SetValueWithoutNotify(value);
        _settings.timeDelay = value;
        _inputField.SetTextWithoutNotify(value.ToString());
    }

    private void SaveFloatValue(float value, string keyWord)
    {
        PlayerPrefs.SetFloat(keyWord, value);
    }

    private float GetFloatValue(string keyWord)
    {
        if (PlayerPrefs.HasKey(keyWord)) return PlayerPrefs.GetFloat(keyWord);
        else
        {
            SaveFloatValue(defaultValue, keyWord);
            return defaultValue;
        }
    }

}
