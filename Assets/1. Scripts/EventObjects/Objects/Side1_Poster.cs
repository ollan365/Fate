using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Side1_Poster : EventObject, IResultExecutable
{

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Poster", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("PosterClick");
    }

    public void ExecuteAction()
    {
        openPoster();
    }

    private void openPoster()
    {
        if((int)GameManager.Instance.GetVariable("KnifeClick")==0&& (int)GameManager.Instance.GetVariable("PosterClick") >= 0)
        {
            Debug.Log("포스터 칼로 뒷면 확인함");
            GameManager.Instance.SetVariable("PosterCorrect", true);
        }
    }
}