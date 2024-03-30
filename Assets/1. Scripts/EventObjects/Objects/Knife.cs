using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : EventObject, IResultExecutable
{

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Knife", this);

        // 다른 사이드에서 이미 커터칼 챙겼으면 커터칼 안 보이게 함
        if ((int)GameManager.Instance.GetVariable("KnifeClick") > 0)
        {
            getKnife();
        }
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("KnifeClick");
    }

    public void ExecuteAction()
    {
        getKnife();
    }

    // 커터칼 사라짐
    private void getKnife()
    {
        this.gameObject.SetActive(false);
    }
}