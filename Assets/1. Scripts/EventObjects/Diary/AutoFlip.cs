using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

[RequireComponent(typeof(PageFlip))]
public class AutoFlip : MonoBehaviour {
    public float pageFlipTime = 0.2f; // Time it takes to flip one page
    public PageFlip controlledBook; // Reference to the PageFlip script
    public int animationFramesCount = 80; // Number of frames for the flip animation
    bool isFlipping = false; // Flag to control flip status

    void Start () {
        if (!controlledBook) controlledBook = GetComponent<PageFlip>();
    }

    public void FlipRightPage() {
        Debug.Log("flip right page");
        
        if (isFlipping || controlledBook.currentPage >= controlledBook.totalPageCount - 1) return;
        StartCoroutine(FlipPage(FlipMode.RightToLeft));
    }

    public void FlipLeftPage() {
        Debug.Log("flip left page");
        
        if (isFlipping || controlledBook.currentPage <= 0) return;
        StartCoroutine(FlipPage(FlipMode.LeftToRight));
    }

    public void FlipToPage(int pageNum)
    {
        Debug.Log($"flip to page {pageNum}");
        
        if (isFlipping || pageNum < 0 || pageNum >= controlledBook.totalPageCount) return;
        while (controlledBook.currentPage != pageNum)
        {
            if (controlledBook.currentPage < pageNum) FlipRightPage();
            else FlipLeftPage();
        }
    }

    private IEnumerator FlipPage(FlipMode flipMode) {
        isFlipping = true;
        controlledBook.mode = flipMode;

        float frameTime = pageFlipTime / animationFramesCount;
        float xc = (controlledBook.EdgeBottomRight.x + controlledBook.EdgeBottomLeft.x) / 2;
        float xl = ((controlledBook.EdgeBottomRight.x - controlledBook.EdgeBottomLeft.x) / 2) * 0.9f;
        float h = Mathf.Abs(controlledBook.EdgeBottomRight.y) * 0.9f;
        float dx = (xl) * 2 / animationFramesCount;
        float x = (flipMode == FlipMode.RightToLeft) ? xc + xl : xc - xl;
        float sign = (flipMode == FlipMode.RightToLeft) ? -1 : 1;

        for (int i = 0; i < animationFramesCount; i++) {
            float y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            Vector3 point = new Vector3(x, y, 0);
            if (flipMode == FlipMode.RightToLeft) {
                controlledBook.DragRightPageToPoint(point);
                controlledBook.UpdateBookRtlToPoint(point);
            } else {
                controlledBook.DragLeftPageToPoint(point);
                controlledBook.UpdateBookLtrToPoint(point);
            }
            yield return new WaitForSeconds(frameTime);
            x += sign * dx;
        }
        controlledBook.ReleasePage();
        isFlipping = false;
    }
}
