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
        if ((int)GameManager.Instance.GetVariable("PaperClick")>0) // ���� Ŭ���ߴٸ�
        {
            // ī�� ����ġ
            CarpetClose();
        }
        else // ���� Ŭ�� �� ��. ó�� Ŭ���̸�
        {
            CarpetOpen();
        }
    }

    private void CarpetOpen(){  // ī�� ���߱�
        //amulet.SetActive(true);
        //RoomManager.Instance.AddScreenObjects(amulet);
        //RoomManager.Instance.isResearch = true;
        transform.GetComponent<Image>().sprite= Resources.Load<Sprite>("PrototypeImage/Carpet Open");
    }

    private void CarpetClose()   // ī�� �ݱ�
    {  
        transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("PrototypeImage/Carpet Closed");
    }
}
