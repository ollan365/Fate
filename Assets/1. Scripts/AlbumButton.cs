using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbumButton : MonoBehaviour
{
    public void OnMouseDown() {
        UIManager.Instance.SetUI(eUIGameObjectName.BlurImage, true, true);
        UIManager.Instance.SetUI(eUIGameObjectName.Album, true, true, FloatDirection.Up);
        UIManager.Instance.SetUI(eUIGameObjectName.ExitButton, true);
        gameObject.SetActive(false);
    }
}
