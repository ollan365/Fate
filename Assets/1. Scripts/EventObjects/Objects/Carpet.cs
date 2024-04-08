using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carpet : EventObject, IResultExecutable
{
    [SerializeField] private GameObject CarpetPaper;

    private void Awake()
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

    // ���ҽ� ���� ����
    [Header("ī�� �̹��� ���ҽ�")]
    [SerializeField] private Sprite carpetOpen;
    [SerializeField] private Sprite carpetClosed;

    private void CarpetOpen()
    {  // ī�� ���߱�
        transform.GetComponent<Image>().sprite= carpetOpen;
        // ���� Ŭ���� ���� ���� �Ŀ��� ī�� Ŭ�� ��Ȱ��ȭ
        GetComponent<Button>().interactable = false;

        // ī�� �� ���� ��ư Ȱ��ȭ
        CarpetPaper.SetActive(true);
    }

    private void CarpetClose()   // ī�� �ݱ�
    {  
        transform.GetComponent<Image>().sprite = carpetClosed;

        // ī�� �� ���� ��Ȱ��ȭ
        CarpetPaper.SetActive(false);
    }
}
