using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PageFlip))]
public class AutoFlip : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Time spent 'dragging' across before release.")]
    public float pageFlipTime = 0.4f;

    [Tooltip("Additional tiny pause after releasing, to avoid UI flickers.")]
    public float settleDelay = 0.01f;

    [Header("Motion")]
    [Tooltip("Ease for the progress across the page (0→1).")]
    public AnimationCurve progressCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Vertical lift profile (0 at edges, 1 at mid). Example: (0,0) (0.5,1) (1,0)")]
    public AnimationCurve arcCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 0f),
        new Keyframe(0.5f, 1f, 0f, 0f),
        new Keyframe(1f, 0f, 0f, 0f)
    );

    [Range(0.1f, 1.0f), Tooltip("How far inward from each edge we start/finish (as a fraction of half-width).")]
    public float edgeInsetRatio = 0.2f;

    [Range(0.1f, 1.2f), Tooltip("How high the page is lifted at peak (relative to page height).")]
    public float liftHeightRatio = 0.1f;

    [Header("References")]
    public PageFlip controlledBook;

    private bool isFlipping;

    private void Start()
    {
        if (!controlledBook)
            controlledBook = GetComponent<PageFlip>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isFlipping = false;
        if (controlledBook != null)
            controlledBook.CancelFlip();
    }

    public void FlipRightPage()
    {
        if (isFlipping || controlledBook.currentPage >= controlledBook.totalPageCount - 1)
            return;

        StartCoroutine(FlipPage(FlipMode.RightToLeft));
    }

    public void FlipLeftPage()
    {
        if (isFlipping || controlledBook.currentPage <= 0)
            return;

        StartCoroutine(FlipPage(FlipMode.LeftToRight));
    }

    public void FlipToPage(int pageNum)
    {
        // Right page indices are even in this setup.
        if (pageNum % 2 != 0)
        {
            Debug.LogWarning("FlipToPage: target page must be even (right page index).");
            return;
        }
        if (isFlipping || pageNum < 0 || pageNum >= controlledBook.totalPageCount)
            return;

        StartCoroutine(FlipToPageCoroutine(pageNum));
    }
    public IEnumerator FlipToPageInEndingAlbum(int pageNum)
    {
        // Right page indices are even in this setup.
        if (pageNum % 2 != 0)
        {
            Debug.LogWarning("FlipToPage: target page must be even (right page index).");
            yield break;
        }
        if (isFlipping || pageNum < 0 || pageNum >= controlledBook.totalPageCount)
            yield break;

        yield return StartCoroutine(FlipToPageCoroutine(pageNum));
        UIManager.Instance.SetUI(eUIGameObjectName.ExitButton, false);
    }

    private IEnumerator FlipToPageCoroutine(int targetPage)
    {
        // Use the book’s authoritative currentPage (don’t track a shadow copy).
        while (controlledBook.currentPage != targetPage)
        {
            if (controlledBook.currentPage < targetPage)
                yield return StartCoroutine(FlipPage(FlipMode.RightToLeft));
            else
                yield return StartCoroutine(FlipPage(FlipMode.LeftToRight));
        }
    }

    private IEnumerator FlipPage(FlipMode flipMode)
    {
        isFlipping = true;

        // Disable UI interactions while flipping
        if (controlledBook.pageContentsManager)
        {
            controlledBook.pageContentsManager.flipLeftButton.SetActive(false);
            controlledBook.pageContentsManager.flipRightButton.SetActive(false);
        }
        UIManager.Instance.SetUI(eUIGameObjectName.ExitButton, false);
        UIManager.Instance.GetUI(eUIGameObjectName.BlurImage).GetComponent<Button>().interactable = false;

        // Tell PageFlip not to chase the mouse while we animate.
        controlledBook.BeginAutoFlip();

        // Geometry snapshot
        float xc = (controlledBook.EdgeBottomRight.x + controlledBook.EdgeBottomLeft.x) * 0.5f;
        float halfSpan = (controlledBook.EdgeBottomRight.x - controlledBook.EdgeBottomLeft.x) * 0.5f;
        float xl = halfSpan * edgeInsetRatio;
        float pageHeight = Mathf.Abs(controlledBook.EdgeBottomRight.y) * 2f;
        float lift = pageHeight * liftHeightRatio * 0.5f; // relative to half-height

        float startX = (flipMode == FlipMode.RightToLeft) ? xc + xl : xc - xl;
        float endX   = (flipMode == FlipMode.RightToLeft) ? xc - xl : xc + xl;

        // Small base Y above the bottom avoids shadow pops at exactly y=0
        const float baseY = 0.001f;

        // First frame: use Drag* to set up pivots/sprites/clipping, then drive with UpdateBook*.
        Vector3 startPt = new Vector3(startX, baseY, 0f);
        if (flipMode == FlipMode.RightToLeft)
            controlledBook.DragRightPageToPoint(startPt);
        else
            controlledBook.DragLeftPageToPoint(startPt);

        // Animate a smooth, human-like arc across the spread
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, pageFlipTime);
            float p = Mathf.Clamp01(progressCurve.Evaluate(t));
            float x = Mathf.Lerp(startX, endX, p);
            float y = baseY + arcCurve.Evaluate(p) * lift;
            Vector3 follow = new Vector3(x, y, 0f);

            if (flipMode == FlipMode.RightToLeft)
                controlledBook.UpdateBookRtlToPoint(follow);
            else
                controlledBook.UpdateBookLtrToPoint(follow);

            yield return null;
        }

        // Release to the book's own forward tween (no manual page index mutation here).
        controlledBook.ReleasePage();

        // Wait until PageFlip finishes (both working page images become inactive).
        yield return new WaitUntil(() =>
            controlledBook.left != null && controlledBook.right != null &&
            !controlledBook.left.gameObject.activeSelf && !controlledBook.right.gameObject.activeSelf
        );

        // Tiny settle delay helps avoid end-of-flip flashes on some GPUs
        if (settleDelay > 0f) yield return new WaitForSeconds(settleDelay);

        controlledBook.EndAutoFlip();

        // Re-enable UI and update flip button visibility based on current page
        if (controlledBook.pageContentsManager)
            controlledBook.pageContentsManager.DisplayPagesStatic(controlledBook.currentPage);
        UIManager.Instance.SetUI(eUIGameObjectName.ExitButton, true);
        UIManager.Instance.GetUI(eUIGameObjectName.BlurImage).GetComponent<Button>().interactable = true;

        isFlipping = false;
    }
}
