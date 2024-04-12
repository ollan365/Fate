using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carpet_open : EventObject, IResultExecutable
{
    [SerializeField] private GameObject CarpetPaper;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("Carpet_Open", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        if ((bool)GameManager.Instance.GetVariable("ChairMoved"))
            GameManager.Instance.IncrementVariable("CarpetClick");
    }

    public void ExecuteAction()
    {
        CarpetClose();
    }

    [Header("´ÝÈù Ä«Æê ¹öÆ°")]
    [SerializeField] private GameObject carpetClosed;

    private void CarpetClose()
    {
        gameObject.SetActive(false);
        carpetClosed.SetActive(true);

        // Ä«ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½È°ï¿½ï¿½È­
        CarpetPaper.SetActive(false);

        //// Ä«Æê ´ÝÀ¸¸é ´Ù¸¥ ¹öÆ°µé ´Ù½Ã È°¼ºÈ­
        //RoomManager.Instance.ControllEventButtons(true);
        //RoomManager.Instance.SetIsInvestigating(false);
    }
}
