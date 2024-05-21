using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public enum FlipMode
{
    RightToLeft,
    LeftToRight
}

public class PageFlip : MonoBehaviour {
    public Canvas canvas;
    [SerializeField]
    RectTransform bookPanel;
    public Sprite background;
    public Sprite[] bookPages;
    public bool interactable = true;
    public bool pageDragging = false;
    public bool enableShadowEffect = true;
    //represent the index of the sprite shown in the right page
    public int currentPage = 0;
    public int TotalPageCount => bookPages.Length;

    public Vector3 EndBottomLeft => ebl;

    public Vector3 EndBottomRight => ebr;

    public Image clippingPlane;
    public Image nextPageClip;
    public Image shadow;
    public Image shadowLtr;
    public Image left;
    public Image leftNext;
    public Image right;
    public Image rightNext;
    public UnityEvent onFlip;
    float radius1, radius2;
    //Spine Bottom
    Vector3 sb;
    //Spine Top
    Vector3 st;
    //corner of the page
    Vector3 c;
    //Edge Bottom Right
    Vector3 ebr;
    //Edge Bottom Left
    Vector3 ebl;
    //follow point 
    Vector3 f;
    //current flip mode
    FlipMode mode;

    [SerializeField] private DiaryManager diaryManager;

    void Start()
    {
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        if (!canvas) Debug.LogError("Book should be a child to canvas");

        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
        UpdateSprites();
        CalcCurlCriticalPoints();

        var rect = bookPanel.rect;
        var pageWidth = rect.width / 2.0f;
        var pageHeight = rect.height;
        nextPageClip.rectTransform.sizeDelta = new Vector2(pageWidth, pageHeight + pageHeight * 2);

        clippingPlane.rectTransform.sizeDelta = new Vector2(pageWidth * 2 + pageHeight, pageHeight + pageHeight * 2);

        //hypotenuse (diagonal) page length
        var hyp = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
        var shadowPageHeight = pageWidth / 2 + hyp;

        shadow.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        shadow.rectTransform.pivot = new Vector2(1, (pageWidth / 2) / shadowPageHeight);

        shadowLtr.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        shadowLtr.rectTransform.pivot = new Vector2(0, (pageWidth / 2) / shadowPageHeight);
        
        diaryManager.DisplayPagesStatic(currentPage);
    }

    void Update()
    {
        if (pageDragging && interactable)
        {
            f = Vector3.Lerp(f, TransformPoint(Input.mousePosition), Time.deltaTime * 10);
            if (mode == FlipMode.RightToLeft) UpdateBookRtlToPoint(f);
            else UpdateBookLtrToPoint(f);
        }
    }
    
    private void CalcCurlCriticalPoints()
    {
        var rect = bookPanel.rect;
        var pageWidth = rect.width / 2.0f;
        var pageHeight = rect.height;

        sb = new Vector3(0, -pageHeight / 2);
        ebr = new Vector3(pageWidth, -pageHeight / 2); // Bottom right corner
        ebl = new Vector3(-pageWidth, -pageHeight / 2); // Bottom left corner
        st = new Vector3(0, pageHeight / 2);

        radius1 = Vector2.Distance(sb, ebr);
        radius2 = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
    }

    private Vector3 TransformPoint(Vector3 mouseScreenPos)
    {
        switch (canvas.renderMode)
        {
            case RenderMode.ScreenSpaceCamera:
            {
                var mouseWorldPos = canvas.worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, canvas.planeDistance));
                Vector2 localPos = bookPanel.InverseTransformPoint(mouseWorldPos);

                return localPos;
            }
            case RenderMode.WorldSpace:
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var globalEbr = transform.TransformPoint(ebr);
                var globalEbl = transform.TransformPoint(ebl);
                var globalSt = transform.TransformPoint(st);
                var p = new Plane(globalEbr, globalEbl, globalSt);
                p.Raycast(ray, out var distance);
                Vector2 localPos = bookPanel.InverseTransformPoint(ray.GetPoint(distance));
                return localPos;
            }
            default:
            {
                //Screen Space Overlay
                Vector2 localPos = bookPanel.InverseTransformPoint(mouseScreenPos);
                return localPos;
            }
        }
    }

    public void UpdateBookLtrToPoint(Vector3 followLocation)
    {
        mode = FlipMode.LeftToRight;
        f = followLocation;
        Transform transform1;
        (transform1 = shadowLtr.transform).SetParent(clippingPlane.transform, true);
        transform1.localPosition = Vector3.zero;
        transform1.localEulerAngles = Vector3.zero;
        left.transform.SetParent(clippingPlane.transform, true);

        Transform transform2;
        (transform2 = right.transform).SetParent(bookPanel.transform, true);
        transform2.localEulerAngles = Vector3.zero;
        leftNext.transform.SetParent(bookPanel.transform, true);

        c = Calc_C_Position(followLocation);
        float clipAngle = CalcClipAngle(c, ebl, out var t1);
        //0 < T0_T1_Angle < 180
        clipAngle = (clipAngle + 180) % 180;

        clippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
        clippingPlane.transform.position = bookPanel.TransformPoint(t1);

        //page position and angle
        left.transform.position = bookPanel.TransformPoint(c);
        var cT1Dy = t1.y - c.y;
        var cT1Dx = t1.x - c.x;
        var cT1Angle = Mathf.Atan2(cT1Dy, cT1Dx) * Mathf.Rad2Deg;
        left.transform.localEulerAngles = new Vector3(0, 0, cT1Angle - 90 - clipAngle);

        nextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
        Transform transform3;
        (transform3 = nextPageClip.transform).position = bookPanel.TransformPoint(t1);
        leftNext.transform.SetParent(transform3, true);
        right.transform.SetParent(clippingPlane.transform, true);
        right.transform.SetAsFirstSibling();

        shadowLtr.rectTransform.SetParent(left.rectTransform, true);
    }

    public void UpdateBookRtlToPoint(Vector3 followLocation)
    {
        mode = FlipMode.RightToLeft;
        f = followLocation;
        Transform transform1;
        (transform1 = shadow.transform).SetParent(clippingPlane.transform, true);
        transform1.localPosition = Vector3.zero;
        transform1.localEulerAngles = Vector3.zero;
        right.transform.SetParent(clippingPlane.transform, true);

        Transform transform2;
        (transform2 = left.transform).SetParent(bookPanel.transform, true);
        transform2.localEulerAngles = Vector3.zero;
        rightNext.transform.SetParent(bookPanel.transform, true);
        c = Calc_C_Position(followLocation);
        var clipAngle = CalcClipAngle(c, ebr, out var t1);
        if (clipAngle > -90) clipAngle += 180;

        clippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
        clippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);
        clippingPlane.transform.position = bookPanel.TransformPoint(t1);

        //page position and angle
        right.transform.position = bookPanel.TransformPoint(c);
        var cT1Dy = t1.y - c.y;
        var cT1Dx = t1.x - c.x;
        var cT1Angle = Mathf.Atan2(cT1Dy, cT1Dx) * Mathf.Rad2Deg;
        right.transform.localEulerAngles = new Vector3(0, 0, cT1Angle - (clipAngle + 90));

        nextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);
        Transform transform3;
        (transform3 = nextPageClip.transform).position = bookPanel.TransformPoint(t1);
        rightNext.transform.SetParent(transform3, true);
        left.transform.SetParent(clippingPlane.transform, true);
        left.transform.SetAsFirstSibling();

        shadow.rectTransform.SetParent(right.rectTransform, true);
    }

    private float CalcClipAngle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
    {
        Vector3 t0 = (c + bookCorner) / 2;
        var t0CornerDy = bookCorner.y - t0.y;
        var t0CornerDx = bookCorner.x - t0.x;
        var t0CornerAngle = Mathf.Atan2(t0CornerDy, t0CornerDx);

        var t1X = t0.x - t0CornerDy * Mathf.Tan(t0CornerAngle);
        t1X = NormalizeT1X(t1X, bookCorner, sb);
        t1 = new Vector3(t1X, sb.y, 0);

        //clipping plane angle=T0_T1_Angle
        var t0T1Dy = t1.y - t0.y;
        var t0T1Dx = t1.x - t0.x;
        var t0T1Angle = Mathf.Atan2(t0T1Dy, t0T1Dx) * Mathf.Rad2Deg;
        return t0T1Angle;
    }

    private float NormalizeT1X(float t1, Vector3 corner, Vector3 sb)
    {
        if (t1 > sb.x && sb.x > corner.x) return sb.x;
        if (t1 < sb.x && sb.x < corner.x) return sb.x;
        return t1;
    }

    private Vector3 Calc_C_Position(Vector3 followLocation)
    {
        Vector3 calcCPosition;
        f = followLocation;
        var fSbDy = f.y - sb.y;
        var fSbDx = f.x - sb.x;
        var fSbAngle = Mathf.Atan2(fSbDy, fSbDx);
        var r1 = new Vector3(radius1 * Mathf.Cos(fSbAngle), radius1 * Mathf.Sin(fSbAngle), 0) + sb;

        var fSbDistance = Vector2.Distance(f, sb);
        calcCPosition = fSbDistance < radius1 ? f : r1;
        var fStDy = calcCPosition.y - st.y;
        var fStDx = calcCPosition.x - st.x;
        var fStAngle = Mathf.Atan2(fStDy, fStDx);
        var r2 = new Vector3(radius2 * Mathf.Cos(fStAngle), radius2 * Mathf.Sin(fStAngle), 0) + st;
        var cStDistance = Vector2.Distance(calcCPosition, st);
        if (cStDistance > radius2) calcCPosition = r2;
        return calcCPosition;
    }

    public void DragRightPageToPoint(Vector3 point)
    {
        if (currentPage >= bookPages.Length) return;
        pageDragging = true;
        mode = FlipMode.RightToLeft;
        f = point;

        nextPageClip.rectTransform.pivot = new Vector2(0, 0.12f); 
        clippingPlane.rectTransform.pivot = new Vector2(1, 0.35f); 

        left.gameObject.SetActive(true);
        left.rectTransform.pivot = new Vector2(0, 0);
        left.transform.position = rightNext.transform.position;
        left.transform.eulerAngles = Vector3.zero;
        left.sprite = (currentPage < bookPages.Length) ? bookPages[currentPage] : background;
        left.transform.SetAsFirstSibling();

        right.gameObject.SetActive(true);
        right.transform.position = rightNext.transform.position;
        right.transform.eulerAngles = Vector3.zero;
        right.sprite = (currentPage < bookPages.Length - 1) ? bookPages[currentPage + 1] : background;

        rightNext.sprite = (currentPage < bookPages.Length - 2) ? bookPages[currentPage + 2] : background;

        leftNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) shadow.gameObject.SetActive(true);
        UpdateBookRtlToPoint(f);

        diaryManager.DisplayPagesDynamic(currentPage);
    }

    public void OnMouseDragRightPage()
    {
        if (interactable) DragRightPageToPoint(TransformPoint(Input.mousePosition));
    }

    public void DragLeftPageToPoint(Vector3 point)
    {
        if (currentPage <= 0) return;
        pageDragging = true;
        mode = FlipMode.LeftToRight;
        f = point;

        nextPageClip.rectTransform.pivot = new Vector2(1, 0.12f); 
        clippingPlane.rectTransform.pivot = new Vector2(0, 0.35f); 

        right.gameObject.SetActive(true);
        right.transform.position = leftNext.transform.position;
        right.sprite = bookPages[currentPage - 1];
        right.transform.eulerAngles = Vector3.zero;
        right.transform.SetAsFirstSibling();

        left.gameObject.SetActive(true);
        left.rectTransform.pivot = new Vector2(1, 0);
        left.transform.position = leftNext.transform.position;
        left.transform.eulerAngles = Vector3.zero;
        left.sprite = (currentPage >= 2) ? bookPages[currentPage - 2] : background;

        leftNext.sprite = (currentPage >= 3) ? bookPages[currentPage - 3] : background;

        rightNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) shadowLtr.gameObject.SetActive(true);
        UpdateBookLtrToPoint(f);

        diaryManager.DisplayPagesDynamic(currentPage - 2);
    }

    public void OnMouseDragLeftPage()
    {
        if (interactable) DragLeftPageToPoint(TransformPoint(Input.mousePosition));
    }

    public void OnMouseRelease()
    {
        if (interactable) ReleasePage();
    }

    public void ReleasePage()
    {
        if (!pageDragging) return;
        pageDragging = false;
        var distanceToLeft = Vector2.Distance(c, ebl);
        var distanceToRight = Vector2.Distance(c, ebr);
        if (distanceToRight < distanceToLeft && mode == FlipMode.RightToLeft) TweenBack();
        else if (distanceToRight > distanceToLeft && mode == FlipMode.LeftToRight) TweenBack();
        else TweenForward();
    }

    private Coroutine currentCoroutine;

    private void UpdateSprites()
    {
        leftNext.sprite = (currentPage > 0 && currentPage <= bookPages.Length) ? bookPages[currentPage - 1] : background;
        rightNext.sprite = (currentPage >= 0 && currentPage < bookPages.Length) ? bookPages[currentPage] : background;
    }

    private void TweenForward()
    {
        if (mode == FlipMode.RightToLeft) currentCoroutine = StartCoroutine(TweenTo(ebl, 0.15f, Flip));
        else currentCoroutine = StartCoroutine(TweenTo(ebr, 0.15f, Flip));
    }

    private void Flip()
    {
        if (mode == FlipMode.RightToLeft) currentPage += 2;
        else currentPage -= 2;
        leftNext.transform.SetParent(bookPanel.transform, true);
        left.transform.SetParent(bookPanel.transform, true);
        leftNext.transform.SetParent(bookPanel.transform, true);
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
        right.transform.SetParent(bookPanel.transform, true);
        rightNext.transform.SetParent(bookPanel.transform, true);
        UpdateSprites();
        shadow.gameObject.SetActive(false);
        shadowLtr.gameObject.SetActive(false);
        onFlip?.Invoke();
        
        diaryManager.DisplayPagesStatic(currentPage);
    }

    private void TweenBack()
    {
        if (mode == FlipMode.RightToLeft)
        {
            currentCoroutine = StartCoroutine(TweenTo(ebr, 0.15f,
                () =>
                {
                    UpdateSprites();
                    rightNext.transform.SetParent(bookPanel.transform);
                    right.transform.SetParent(bookPanel.transform);

                    left.gameObject.SetActive(false);
                    right.gameObject.SetActive(false);
                    pageDragging = false;
                }
                ));
        }
        else
        {
            currentCoroutine = StartCoroutine(TweenTo(ebl, 0.15f,
                () =>
                {
                    UpdateSprites();

                    leftNext.transform.SetParent(bookPanel.transform);
                    left.transform.SetParent(bookPanel.transform);

                    left.gameObject.SetActive(false);
                    right.gameObject.SetActive(false);
                    pageDragging = false;
                }
                ));
        }
        
        diaryManager.DisplayPagesStatic(currentPage);
    }

    private IEnumerator TweenTo(Vector3 to, float duration, System.Action onFinish)
    {
        var steps = (int)(duration / 0.025f);
        var displacement = (to - f) / steps;
        for (var i = 0; i < steps - 1; i++)
        {
            if (mode == FlipMode.RightToLeft) UpdateBookRtlToPoint(f + displacement);
            else UpdateBookLtrToPoint(f + displacement);

            yield return new WaitForSeconds(0.025f);
        }

        onFinish?.Invoke();
    }
}
