using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

[RequireComponent(typeof(PageFlip))]
public class AutoFlip : MonoBehaviour
{
    public float pageFlipTime = 0.2f; // Time it takes to flip one page
    public PageFlip controlledBook; // Reference to the PageFlip script
    public int animationFramesCount = 80; // Number of frames for the flip animation
    bool isFlipping = false; // Flag to control flip status

    private int currentPage;

    private void Start()
    {
        if (!controlledBook) controlledBook = GetComponent<PageFlip>();
    }

    public void FlipRightPage()
    {
        // Debug.Log("flip right page");

        if (isFlipping || controlledBook.currentPage >= controlledBook.totalPageCount - 1) return;
        StartCoroutine(FlipPage(FlipMode.RightToLeft));
    }

    public void FlipLeftPage()
    {
        // Debug.Log("flip left page");

        if (isFlipping || controlledBook.currentPage <= 0) return;
        StartCoroutine(FlipPage(FlipMode.LeftToRight));
    }

    public void FlipToPage(int pageNum)
    {
        if (pageNum % 2 != 0)
        {
            Debug.LogWarning("pageNum should be even!.");
            return;
        }

        // Debug.Log($"flip to page {pageNum}");

        if (isFlipping || pageNum < 0 || pageNum >= controlledBook.totalPageCount) return;
        StartCoroutine(FlipToPageCoroutine(pageNum));
    }

    private IEnumerator FlipToPageCoroutine(int pageNum)
    {
        currentPage = controlledBook.currentPage;
        while (currentPage != pageNum)
        {
            // Debug.Log($"current page: {currentPage}, target page: {pageNum}");

            if (currentPage < pageNum)
            {
                yield return StartCoroutine(FlipPage(FlipMode.RightToLeft));
            }
            else if (currentPage > pageNum)
            {
                yield return StartCoroutine(FlipPage(FlipMode.LeftToRight));
            }
        }
    }

    private IEnumerator FlipPage(FlipMode flipMode)
    {
        isFlipping = true;
        controlledBook.mode = flipMode;

        controlledBook.pageContentsManager.flipLeftButton.SetActive(false);
        controlledBook.pageContentsManager.flipRightButton.SetActive(false);
        UIManager.Instance.SetUI("ExitButton", false);

        float elapsedTime = 0;
        float xc = (controlledBook.EdgeBottomRight.x + controlledBook.EdgeBottomLeft.x) / 2;
        float xl = ((controlledBook.EdgeBottomRight.x - controlledBook.EdgeBottomLeft.x) / 2) * 0.9f;
        float h = Mathf.Abs(controlledBook.EdgeBottomRight.y) * 0.9f;

        Vector3 startPoint = new Vector3(
            flipMode == FlipMode.RightToLeft ? xc + xl : xc - xl,
            0,
            0
        );
        Vector3 endPoint = new Vector3(
            flipMode == FlipMode.RightToLeft ? xc - xl : xc + xl,
            0,
            0
        );

        while (elapsedTime < pageFlipTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / pageFlipTime;

            // Use smooth step for more natural movement
            t = Mathf.SmoothStep(0, 1, t);

            // Calculate current position
            float x = Mathf.Lerp(startPoint.x, endPoint.x, t);
            float y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            Vector3 point = new Vector3(x, y, 0);

            // Only call DragPageToPoint, not both
            if (flipMode == FlipMode.RightToLeft)
            {
                controlledBook.DragRightPageToPoint(point);
            }
            else
            {
                controlledBook.DragLeftPageToPoint(point);
            }

            yield return null; // Wait for next frame instead of fixed time
        }

        controlledBook.ReleasePage();
        isFlipping = false;

        if (flipMode == FlipMode.RightToLeft) currentPage += 2;
        else currentPage -= 2;

        UIManager.Instance.SetUI("ExitButton", true);
    }
}