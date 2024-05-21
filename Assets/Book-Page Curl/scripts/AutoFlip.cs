using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

[RequireComponent(typeof(PageFlip))]
public class AutoFlip : MonoBehaviour {
    public FlipMode Mode;
    public float PageFlipTime = 1;
    public float TimeBetweenPages = 1;
    public float DelayBeforeStarting = 0;
    public bool AutoStartFlip=true;
    [FormerlySerializedAs("ControledBook")] public PageFlip controledPageFlip;
    public int AnimationFramesCount = 40;
    bool isFlipping = false;
    // Use this for initialization
    void Start () {
        if (!controledPageFlip)
            controledPageFlip = GetComponent<PageFlip>();
        if (AutoStartFlip)
            StartFlipping();
        controledPageFlip.onFlip.AddListener(new UnityEngine.Events.UnityAction(PageFlipped));
	}
    void PageFlipped()
    {
        isFlipping = false;
    }
	public void StartFlipping()
    {
        StartCoroutine(FlipToEnd());
    }
    public void FlipRightPage()
    {
        if (isFlipping) return;
        if (controledPageFlip.currentPage >= controledPageFlip.TotalPageCount) return;
        isFlipping = true;
        float frameTime = PageFlipTime / AnimationFramesCount;
        float xc = (controledPageFlip.EndBottomRight.x + controledPageFlip.EndBottomLeft.x) / 2;
        float xl = ((controledPageFlip.EndBottomRight.x - controledPageFlip.EndBottomLeft.x) / 2) * 0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(controledPageFlip.EndBottomRight.y) * 0.9f;
        float dx = (xl)*2 / AnimationFramesCount;
        StartCoroutine(FlipRTL(xc, xl, h, frameTime, dx));
    }
    public void FlipLeftPage()
    {
        if (isFlipping) return;
        if (controledPageFlip.currentPage <= 0) return;
        isFlipping = true;
        float frameTime = PageFlipTime / AnimationFramesCount;
        float xc = (controledPageFlip.EndBottomRight.x + controledPageFlip.EndBottomLeft.x) / 2;
        float xl = ((controledPageFlip.EndBottomRight.x - controledPageFlip.EndBottomLeft.x) / 2) * 0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(controledPageFlip.EndBottomRight.y) * 0.9f;
        float dx = (xl) * 2 / AnimationFramesCount;
        StartCoroutine(FlipLTR(xc, xl, h, frameTime, dx));
    }
    IEnumerator FlipToEnd()
    {
        yield return new WaitForSeconds(DelayBeforeStarting);
        float frameTime = PageFlipTime / AnimationFramesCount;
        float xc = (controledPageFlip.EndBottomRight.x + controledPageFlip.EndBottomLeft.x) / 2;
        float xl = ((controledPageFlip.EndBottomRight.x - controledPageFlip.EndBottomLeft.x) / 2)*0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(controledPageFlip.EndBottomRight.y)*0.9f;
        //y=-(h/(xl)^2)*(x-xc)^2          
        //               y         
        //               |          
        //               |          
        //               |          
        //_______________|_________________x         
        //              o|o             |
        //           o   |   o          |
        //         o     |     o        | h
        //        o      |      o       |
        //       o------xc-------o      -
        //               |<--xl-->
        //               |
        //               |
        float dx = (xl)*2 / AnimationFramesCount;
        switch (Mode)
        {
            case FlipMode.RightToLeft:
                while (controledPageFlip.currentPage < controledPageFlip.TotalPageCount)
                {
                    StartCoroutine(FlipRTL(xc, xl, h, frameTime, dx));
                    yield return new WaitForSeconds(TimeBetweenPages);
                }
                break;
            case FlipMode.LeftToRight:
                while (controledPageFlip.currentPage > 0)
                {
                    StartCoroutine(FlipLTR(xc, xl, h, frameTime, dx));
                    yield return new WaitForSeconds(TimeBetweenPages);
                }
                break;
        }
    }
    IEnumerator FlipRTL(float xc, float xl, float h, float frameTime, float dx)
    {
        float x = xc + xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);

        controledPageFlip.DragRightPageToPoint(new Vector3(x, y, 0));
        for (int i = 0; i < AnimationFramesCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            controledPageFlip.UpdateBookRtlToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x -= dx;
        }
        controledPageFlip.ReleasePage();
    }
    IEnumerator FlipLTR(float xc, float xl, float h, float frameTime, float dx)
    {
        float x = xc - xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);
        controledPageFlip.DragLeftPageToPoint(new Vector3(x, y, 0));
        for (int i = 0; i < AnimationFramesCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            controledPageFlip.UpdateBookLtrToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x += dx;
        }
        controledPageFlip.ReleasePage();
    }
}
