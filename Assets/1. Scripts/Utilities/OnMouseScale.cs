using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseScale : MonoBehaviour
{
    public float originalScale = 1f;
    public float scaleMultiplier = 1.2f;
    
    public void PointerEnter()
    {
        UIManager.Instance.ChangeCursor();
        transform.localScale = new Vector2(originalScale * scaleMultiplier, originalScale * scaleMultiplier);
    }
    
    public void PointerExit()
    {
        UIManager.Instance.SetCursorAuto();
        transform.localScale = new Vector2(originalScale, originalScale);
    }
}
