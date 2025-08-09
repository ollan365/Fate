using UnityEngine;

public class MemoButton : MonoBehaviour
{
    public void OnMouseDown()
    {
        MemoManager.Instance.SetMemoContents(true);
        MemoManager.Instance.SetMemoButtons(false, true);
        
        if (FollowManager.Instance)
            FollowManager.Instance.ClickObject();
    }
}
