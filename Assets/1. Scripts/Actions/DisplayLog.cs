using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayLog : MonoBehaviour
{
    public TextMeshProUGUI logText;
    
    public void DisplayLogOnScreen(string message)
    {
        if (logText)
        {
            logText.text = message;
        }
        else
        {
            Debug.LogError("Class DisplayLog: logText is not assigned.");
        }
    }
}
