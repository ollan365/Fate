using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoButton : MonoBehaviour
{
    public void OnMouseDown()
    {
        bool fade = MemoManager.Instance.fade;
        FloatDirection floatDirection = MemoManager.Instance.floatDirection;
        MemoManager.Instance.SetMemoContents(true, fade, floatDirection);
        MemoManager.Instance.SetMemoButtons(false, true);
        
        if (FollowManager.Instance)
            FollowManager.Instance.ClickObject();
    }
}
