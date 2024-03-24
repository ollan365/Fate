using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carpet : EventObject, IResultExecutable
{
    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Carpet", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("CarpetClick");
    }

    public void ExecuteAction()
    {
        if ((int)GameManager.Instance.GetVariable("PaperClick")>0) // 종이 클릭했다면
        {
            // 카펫 원위치
            CarpetClose();
        }
        else // 종이 클릭 안 함. 처음 클릭이면
        {
            CarpetOpen();
        }
    }

    private void CarpetOpen(){  // 카펫 들추기
        //amulet.SetActive(true);
        //RoomManager.Instance.AddScreenObjects(amulet);
        //RoomManager.Instance.isResearch = true;
        transform.GetComponent<Image>().sprite= Resources.Load<Sprite>("PrototypeImage/Carpet Open");
    }

    private void CarpetClose()   // 카펫 닫기
    {  
        transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("PrototypeImage/Carpet Closed");
    }
}
