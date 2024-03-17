using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayLog : MonoBehaviour
{
    public TextMeshProUGUI logText;
    
    public void DisplayLogOnScreen(string message)
    {
        logText.text = message;
    }
}
