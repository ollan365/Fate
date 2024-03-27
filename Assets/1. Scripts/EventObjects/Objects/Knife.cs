using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : EventObject, IResultExecutable
{

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Knife", this);
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

    // Ä¿ÅÍÄ® »ç¶óÁü
    private void getKnife()
    {
        this.gameObject.SetActive(false);
    }
}