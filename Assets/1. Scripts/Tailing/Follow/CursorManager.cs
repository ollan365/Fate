using UnityEngine;
using static Constants;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }
    public CursorImage CurrentCursor { get; private set; }

    [SerializeField] private Texture2D[] cursorTextures;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        CurrentCursor = CursorImage.Normal;
    }

    public void ChangeCursorImage(CursorImage image)
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
