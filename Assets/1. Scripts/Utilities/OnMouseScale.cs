using UnityEngine;

public class OnMouseScale : MonoBehaviour
{
    public float originalScale = 1f;
    public float scaleMultiplier = 1.2f;
    private RectTransform rectTransform;
    [SerializeField] private bool checkMouseHover;
    private Camera uiCamera;

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

    private void Start()
    {
        if (!checkMouseHover)
            return;

        uiCamera = UIManager.Instance.uiCamera;
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            Debug.LogError("RectTransform not found on the object.");
    }

    private void Update()
    {
        if (!checkMouseHover || !rectTransform || !uiCamera)
            return;
        
        // if touching mouse pointer
        bool isPointerOverUI = RectTransformUtility.RectangleContainsScreenPoint(rectTransform,
            Input.mousePosition,
            uiCamera);
        if (isPointerOverUI)
            PointerEnter();
        else
            PointerExit();
    }
}
