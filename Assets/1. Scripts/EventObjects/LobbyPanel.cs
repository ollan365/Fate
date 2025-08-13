using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    private void OnEnable() {
        UIManager.Instance?.SetUI(eUIGameObjectName.BlurImage, true);
    }
    
    private void OnDisable() {
        if (DialogueManager.Instance?.isDialogueActive == false)
            UIManager.Instance?.SetUI(eUIGameObjectName.BlurImage, false);
    }
}
