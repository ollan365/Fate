using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum PageType
{
    Left,
    Right,
    Back,
    Front
}

abstract public class PageContentsManager : MonoBehaviour
{
    [SerializeField] public GameObject flipLeftButton;
    [SerializeField] public GameObject flipRightButton;
    
    protected Dictionary<string, string> PagesDictionary = new Dictionary<string, string>();
    [SerializeField] protected TextMeshProUGUI leftPage;
    [SerializeField] protected TextMeshProUGUI rightPage;
    [SerializeField] protected TextMeshProUGUI backPage;
    [SerializeField] protected TextMeshProUGUI frontPage;
    
    public abstract void ParsePageContents();

    public abstract void DisplayPage(PageType pageType, int pageNum);

    public abstract void DisplayPagesDynamic(int currentPage);

    public abstract void DisplayPagesStatic(int currentPage);
}
