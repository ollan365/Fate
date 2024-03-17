using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diary : EventObject
{
    public void ClickBtn()
    {
        if(!(bool)GameManager.Instance.GetVariable("DiaryPasswordCorrect"))
            GameManager.Instance.IncrementObjectClick("DiaryClick");
    }
}
