using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum FlipMode
{
    RightToLeft,
    LeftToRight
}

public class PageFlip : MonoBehaviour
{
    [Header("PageTextManager")]
    public PageContentsManager pageContentsManager;
    
    [Header("Canvas and Panel")]
    public Canvas canvas;
    [SerializeField] private RectTransform bookPanel;
    
    [Header("Page Sprites")]
    public Sprite backPage;
    public Sprite leftPage;
    public Sprite rightPage;
    
    public bool interactable = true;
    private bool pageDragging;
    private bool autoFlipping = false;
    private bool enableShadowEffect = true;
    private bool pendingReset = false;

    [Header("Current and Total Page Count")]
    // represent the index of the sprite shown in the right page
    public int currentPage;
    public int totalPageCount;

    [Header("Essential GameObjects - Do not change")]
    public Image clippingPlane;
    public Image nextPageClip;
    public Image shadow;
    public Image shadowLtr;
    public Image left;
    public Image leftNext;
    public Image right;
    public Image rightNext;
    private UnityEvent onFlip;

    private float radius1, radius2;

    //Spine Bottom
    private Vector3 spineBottom;

    //Spine Top
    private Vector3 spineTop;

    //corner of the page
    private Vector3 corner;

    //follow point 
    private Vector3 f;

    //current flip mode
    public FlipMode mode;

    public Vector3 EdgeBottomLeft { get; private set; }

    public Vector3 EdgeBottomRight { get; private set; }
    
    private void Start()
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
        shadow.rectTransform.pivot = new Vector2(1, pageWidth / 2 / shadowPageHeight);

        shadowLtr.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        shadowLtr.rectTransform.pivot = new Vector2(0, pageWidth / 2 / shadowPageHeight);

        pageContentsManager.DisplayPagesStatic(currentPage);
    }

    private void Update()
    {
        if (pageDragging && interactable && !autoFlipping)
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

        spineBottom = new Vector3(0, -pageHeight / 2);
        EdgeBottomRight = new Vector3(pageWidth, -pageHeight / 2); // Bottom right corner
        EdgeBottomLeft = new Vector3(-pageWidth, -pageHeight / 2); // Bottom left corner
        spineTop = new Vector3(0, pageHeight / 2);

        radius1 = Vector2.Distance(spineBottom, EdgeBottomRight);
        radius2 = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
    }

    private Vector3 TransformPoint(Vector3 mouseScreenPos)
    {
        switch (canvas.renderMode)
        {
            case RenderMode.ScreenSpaceCamera:
            {
                var mouseWorldPos =
                    canvas.worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y,
                        canvas.planeDistance));
                Vector2 localPos = bookPanel.InverseTransformPoint(mouseWorldPos);

                return localPos;
            }
            case RenderMode.WorldSpace:
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var globalEbr = transform.TransformPoint(EdgeBottomRight);
                var globalEbl = transform.TransformPoint(EdgeBottomLeft);
                var globalSt = transform.TransformPoint(spineTop);
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

        corner = Calc_C_Position(followLocation);
        var clipAngle = CalcClipAngle(corner, EdgeBottomLeft, out var t1);
        //0 < T0_T1_Angle < 180
        clipAngle = (clipAngle + 180) % 180;

        clippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
        clippingPlane.transform.position = bookPanel.TransformPoint(t1);

        //page position and angle
        left.transform.position = bookPanel.TransformPoint(corner);
        var cT1Dy = t1.y - corner.y;
        var cT1Dx = t1.x - corner.x;
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
        corner = Calc_C_Position(followLocation);
        var clipAngle = CalcClipAngle(corner, EdgeBottomRight, out var t1);
        if (clipAngle > -90) clipAngle += 180;

        clippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
        clippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);
        clippingPlane.transform.position = bookPanel.TransformPoint(t1);

        //page position and angle
        right.transform.position = bookPanel.TransformPoint(corner);
        var cT1Dy = t1.y - corner.y;
        var cT1Dx = t1.x - corner.x;
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

    private float CalcClipAngle(Vector3 corner, Vector3 bookCorner, out Vector3 t1)
    {
        var t0 = (corner + bookCorner) / 2;
        var t0CornerDy = bookCorner.y - t0.y;
        var t0CornerDx = bookCorner.x - t0.x;
        var t0CornerAngle = Mathf.Atan2(t0CornerDy, t0CornerDx);

        var t1X = t0.x - t0CornerDy * Mathf.Tan(t0CornerAngle);
        t1X = NormalizeT1X(t1X, bookCorner, spineBottom);
        t1 = new Vector3(t1X, spineBottom.y, 0);

        //clipping plane angle=T0_T1_Angle
        var t0T1Dy = t1.y - t0.y;
        var t0T1Dx = t1.x - t0.x;
        var t0T1Angle = Mathf.Atan2(t0T1Dy, t0T1Dx) * Mathf.Rad2Deg;
        return t0T1Angle;
    }

    private static float NormalizeT1X(float t1, Vector3 corner, Vector3 spineBottom)
    {
        if (t1 > spineBottom.x && spineBottom.x > corner.x) return spineBottom.x;
        if (t1 < spineBottom.x && spineBottom.x < corner.x) return spineBottom.x;
        return t1;
    }

    private Vector3 Calc_C_Position(Vector3 followLocation)
    {
        f = followLocation;
        var fSbDy = f.y - spineBottom.y;
        var fSbDx = f.x - spineBottom.x;
        var fSbAngle = Mathf.Atan2(fSbDy, fSbDx);
        var r1 = new Vector3(radius1 * Mathf.Cos(fSbAngle), radius1 * Mathf.Sin(fSbAngle), 0) + spineBottom;

        var fSbDistance = Vector2.Distance(f, spineBottom);
        var calcCPosition = fSbDistance < radius1 ? f : r1;
        var fStDy = calcCPosition.y - spineTop.y;
        var fStDx = calcCPosition.x - spineTop.x;
        var fStAngle = Mathf.Atan2(fStDy, fStDx);
        var r2 = new Vector3(radius2 * Mathf.Cos(fStAngle), radius2 * Mathf.Sin(fStAngle), 0) + spineTop;
        var cStDistance = Vector2.Distance(calcCPosition, spineTop);
        if (cStDistance > radius2) calcCPosition = r2;
        return calcCPosition;
    }

    public void DragRightPageToPoint(Vector3 point)
    {
        if (currentPage >= totalPageCount - 1) return;
        pageDragging = true;
        mode = FlipMode.RightToLeft;
        f = point;

        nextPageClip.rectTransform.pivot = new Vector2(0, 0.12f);
        clippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);

        left.gameObject.SetActive(true);
        left.rectTransform.pivot = new Vector2(0, 0);
        var transform1 = left.transform;
        transform1.position = rightNext.transform.position;
        transform1.eulerAngles = Vector3.zero;
        left.sprite = currentPage < totalPageCount ? rightPage : backPage;
        left.transform.SetAsFirstSibling();

        right.gameObject.SetActive(true);
        var transform2 = right.transform;
        transform2.position = rightNext.transform.position;
        transform2.eulerAngles = Vector3.zero;
        right.sprite = currentPage < totalPageCount - 1 ? leftPage : backPage;

        // rightNext.sprite = currentPage < totalPageCount - 2 ? rightPage : backPage;

        leftNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) shadow.gameObject.SetActive(true);
        UpdateBookRtlToPoint(f);

        pageContentsManager.DisplayPagesDynamic(currentPage);
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
        var transform1 = right.transform;
        var position = leftNext.transform.position;
        transform1.position = position;
        right.sprite = leftPage;
        transform1.eulerAngles = Vector3.zero;
        right.transform.SetAsFirstSibling();

        left.gameObject.SetActive(true);
        left.rectTransform.pivot = new Vector2(1, 0);
        var transform2 = left.transform;
        transform2.position = position;
        transform2.eulerAngles = Vector3.zero;
        left.sprite = currentPage >= 2 ? rightPage : backPage;

        leftNext.sprite = currentPage >= 3 ? leftPage : backPage;

        rightNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) shadowLtr.gameObject.SetActive(true);
        UpdateBookLtrToPoint(f);

        pageContentsManager.DisplayPagesDynamic(currentPage - 2);
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
        var distanceToLeft = Vector2.Distance(corner, EdgeBottomLeft);
        var distanceToRight = Vector2.Distance(corner, EdgeBottomRight);
        if (distanceToRight < distanceToLeft && mode == FlipMode.RightToLeft) TweenBack();
        else if (distanceToRight > distanceToLeft && mode == FlipMode.LeftToRight) TweenBack();
        else TweenForward();
    }

    private Coroutine currentCoroutine;

    private void UpdateSprites()
    {
        leftNext.sprite = 0 < currentPage && currentPage <= totalPageCount ? leftPage : backPage;
        // rightNext.sprite = rightPage;
    }

    private void TweenForward()
    {
        currentCoroutine =
            StartCoroutine(TweenTo(mode == FlipMode.RightToLeft ? EdgeBottomLeft : EdgeBottomRight, 0.15f, Flip));
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

        pageContentsManager.DisplayPagesStatic(currentPage);
    }

    private void TweenBack()
    {
        if (mode == FlipMode.RightToLeft)
            currentCoroutine = StartCoroutine(TweenTo(EdgeBottomRight, 0.15f,
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
        else
            currentCoroutine = StartCoroutine(TweenTo(EdgeBottomLeft, 0.15f,
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

        pageContentsManager.DisplayPagesStatic(currentPage);
    }

    private IEnumerator TweenTo(Vector3 to, float duration, Action onFinish)
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

    public void BeginAutoFlip() {
        autoFlipping = true;
    }

    public void EndAutoFlip()
    {
        autoFlipping = false;
        var wasDragging = pageDragging;
        pageDragging = false;
        if (wasDragging) {
            shadow.gameObject.SetActive(false);
            shadowLtr.gameObject.SetActive(false);
        }
    }

    public void CancelFlip()
    {
        if (currentCoroutine != null) {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (pageDragging || autoFlipping)
            pendingReset = true;

        pageDragging = false;
        autoFlipping = false;
    }

    private void OnEnable()
    {
        if (!pendingReset) 
            return;
        
        pendingReset = false;

        shadow.gameObject.SetActive(false);
        shadowLtr.gameObject.SetActive(false);

        left.gameObject.SetActive(false);
        left.transform.SetParent(bookPanel, true);
        left.transform.localEulerAngles = Vector3.zero;

        right.gameObject.SetActive(false);
        right.transform.SetParent(bookPanel, true);
        right.transform.localEulerAngles = Vector3.zero;

        leftNext.transform.SetParent(bookPanel, true);
        rightNext.transform.SetParent(bookPanel, true);

        UpdateSprites();
        pageContentsManager.DisplayPagesStatic(currentPage);
    }
}