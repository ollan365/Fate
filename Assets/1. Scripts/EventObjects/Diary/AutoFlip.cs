using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

[RequireComponent(typeof(PageFlip))]
public class AutoFlip : MonoBehaviour {
    public float PageFlipTime = 1; // Time it takes to flip one page
    public PageFlip ControlledBook; // Reference to the PageFlip script
    public int AnimationFramesCount = 40; // Number of frames for the flip animation
    bool isFlipping = false; // Flag to control flip status

    void Start () {
        if (!ControlledBook)
            ControlledBook = GetComponent<PageFlip>();
    }

    public void FlipRightPage() {
        if (isFlipping || ControlledBook.currentPage >= ControlledBook.TotalPageCount - 1) return;
        StartCoroutine(FlipPage(FlipMode.RightToLeft));
    }

    public void FlipLeftPage() {
        if (isFlipping || ControlledBook.currentPage <= 0) return;
        StartCoroutine(FlipPage(FlipMode.LeftToRight));
    }

    IEnumerator FlipPage(FlipMode flipMode) {
        isFlipping = true;
        ControlledBook.mode = flipMode;

        float frameTime = PageFlipTime / AnimationFramesCount;
        float xc = (ControlledBook.EdgeBottomRight.x + ControlledBook.EdgeBottomLeft.x) / 2;
        float xl = ((ControlledBook.EdgeBottomRight.x - ControlledBook.EdgeBottomLeft.x) / 2) * 0.9f;
        float h = Mathf.Abs(ControlledBook.EdgeBottomRight.y) * 0.9f;
        float dx = (xl) * 2 / AnimationFramesCount;
        float x = (flipMode == FlipMode.RightToLeft) ? xc + xl : xc - xl;
        float sign = (flipMode == FlipMode.RightToLeft) ? -1 : 1;

        for (int i = 0; i < AnimationFramesCount; i++) {
            float y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            Vector3 point = new Vector3(x, y, 0);
            if (flipMode == FlipMode.RightToLeft) {
                ControlledBook.DragRightPageToPoint(point);
                ControlledBook.UpdateBookRtlToPoint(point);
            } else {
                ControlledBook.DragLeftPageToPoint(point);
                ControlledBook.UpdateBookLtrToPoint(point);
            }
            yield return new WaitForSeconds(frameTime);
            x += sign * dx;
        }
        ControlledBook.ReleasePage();
        isFlipping = false;
    }
}
