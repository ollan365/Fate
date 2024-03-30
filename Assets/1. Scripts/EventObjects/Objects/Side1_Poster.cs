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
        //Debug.Log("현재 포스터상태 : "+ (bool)GameManager.Instance.GetVariable("PosterCorrect"));

        if ((int)GameManager.Instance.GetVariable("KnifeClick") > 0 && (int)GameManager.Instance.GetVariable("PosterClick") >= 0
            && !(bool)GameManager.Instance.GetVariable("PosterCorrect"))
        {
            Debug.Log("포스터 칼로 뒷면 확인함");
            GameManager.Instance.SetVariable("PosterCorrect", true);
        }

        //Debug.Log("현재 포스터상태 : " + (bool)GameManager.Instance.GetVariable("PosterCorrect"));
    }
}