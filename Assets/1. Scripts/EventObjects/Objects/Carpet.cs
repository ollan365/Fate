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
        if((bool)GameManager.Instance.GetVariable("ChairMoved"))
            GameManager.Instance.IncrementVariable("CarpetClick");
    }

    public void ExecuteAction()
    {
        if ((int)GameManager.Instance.GetVariable("PaperClick")>0) // ï¿½ï¿½ï¿½ï¿½ Å¬ï¿½ï¿½ï¿½ß´Ù¸ï¿½
        {
            // Ä«ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½Ä¡
            CarpetClose();
        }
        else // ï¿½ï¿½ï¿½ï¿½ Å¬ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½. Ã³ï¿½ï¿½ Å¬ï¿½ï¿½ï¿½Ì¸ï¿½
        {
            CarpetOpen();
        }
    }

    // ï¿½ï¿½ï¿½Ò½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
    [Header("Ä«ï¿½ï¿½ ï¿½Ì¹ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½Ò½ï¿½")]
    [SerializeField] private Sprite carpetOpen;
    [SerializeField] private Sprite carpetClosed;

    private void CarpetOpen()
    {  // Ä«ï¿½ï¿½ ï¿½ï¿½ï¿½ß±ï¿½
        transform.GetComponent<Image>().sprite= carpetOpen;
        // ï¿½ï¿½ï¿½ï¿½ Å¬ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½Ä¿ï¿½ï¿½ï¿½ Ä«ï¿½ï¿½ Å¬ï¿½ï¿½ ï¿½ï¿½È°ï¿½ï¿½È­
        GetComponent<Button>().interactable = false;

        // Ä«ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½Æ° È°ï¿½ï¿½È­
        CarpetPaper.SetActive(true);

        // Ä«Æê ¹Ø Á¾ÀÌ ¹öÆ° Á¦¿ÜÇÏ°í ´Ù ¹öÆ° ¾È ´­¸®°Ô ÇÔ.
        RoomManager.Instance.ControllEventButtons(false);
        RoomManager.Instance.SetIsInvestigating(true);
    }

    private void CarpetClose()   // Ä«ï¿½ï¿½ ï¿½Ý±ï¿½
    {  
        transform.GetComponent<Image>().sprite = carpetClosed;

        // Ä«ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½È°ï¿½ï¿½È­
        CarpetPaper.SetActive(false);

        // Ä«Æê ´ÝÀ¸¸é ´Ù¸¥ ¹öÆ°µé ´Ù½Ã È°¼ºÈ­
        RoomManager.Instance.ControllEventButtons(true);
        RoomManager.Instance.SetIsInvestigating(false);
    }
}
