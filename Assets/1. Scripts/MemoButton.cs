using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoButton : MonoBehaviour
{
    public bool fade = true;
    
    public void OnMouseDown()
    {
        MemoManager.Instance.SetMemoContents(true, fade);
        MemoManager.Instance.SetMemoButtons(false, true);
        
        if (FollowManager.Instance)
            FollowManager.Instance.ClickObject();
    }
}
