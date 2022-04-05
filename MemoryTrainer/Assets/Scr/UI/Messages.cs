using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum MessagesStates { Start, Remember, Check, Defeat, Win }
public class Messages : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _text;

    const string startMessage = "Press start button and memorize the position of the numbers";
    const string rememberMessage = "Remember!";
    const string checkMessage = "Drag and drop the numbers in the correct order. Next press check button, for result";
    const string defeatMessage = "You lose! To repeat press start button";
    const string winMessage = "You win! To repeat press start button";

    public void SetText(MessagesStates state)
    {
        switch (state)
        {
            case MessagesStates.Start: _text.text = startMessage; break;
            case MessagesStates.Remember: _text.text = rememberMessage; break;
            case MessagesStates.Check: _text.text = checkMessage; break;
            case MessagesStates.Defeat: _text.text = defeatMessage; break;
            case MessagesStates.Win: _text.text = winMessage; break;
            default: _text.text = string.Empty; break;
        }
    }
}
