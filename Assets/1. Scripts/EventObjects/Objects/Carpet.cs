using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carpet : EventObject, IResultExecutable
{
    [SerializeField] private GameObject CarpetPaper;

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

    // 리소스 관련 변수
    [Header("카펫 이미지 리소스")]
    [SerializeField] private Sprite carpetOpen;
    [SerializeField] private Sprite carpetClosed;

    private void CarpetOpen()
    {  // 카펫 들추기
        transform.GetComponent<Image>().sprite= carpetOpen;
        // 종이 클릭을 위해 들춘 후에는 카펫 클릭 비활성화
        GetComponent<Button>().interactable = false;

        // 카펫 밑 종이 버튼 활성화
        CarpetPaper.SetActive(true);
    }

    private void CarpetClose()   // 카펫 닫기
    {  
        transform.GetComponent<Image>().sprite = carpetClosed;

        // 카펫 밑 종이 비활성화
        CarpetPaper.SetActive(false);
    }
}
