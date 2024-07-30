using UnityEngine;
using static Constants;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }
    // 커서의 종류
    private enum CursorImage { Normal, Glass, X }
    private CursorImage CurrentCursor { get; set; }

    [SerializeField] private Texture2D[] cursorTextures;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        CurrentCursor = CursorImage.Normal;
    }

    public void ChangeCursorInFollow(bool toNormal = false)
    {
        if (toNormal) ChangeCursorImage(CursorImage.Normal);
        else if (FollowManager.Instance.CanClick) ChangeCursorImage(CursorImage.Glass);
        else ChangeCursorImage(CursorImage.X);

    }

    private void ChangeCursorImage(CursorImage image)
    {
        Texture2D cursorImage = null;
        switch (image)
        {
            case CursorImage.Glass: cursorImage = cursorTextures[0]; break;
            case CursorImage.X: cursorImage = cursorTextures[1]; break;
        }

        Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.Auto);
        CurrentCursor = image;
    }
}
