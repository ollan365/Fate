using UnityEngine;
using Fate.Managers;


namespace Fate.Events
{
    public class LobbyPanel : MonoBehaviour
    {
        private void OnEnable() {
            UIManager.Instance?.SetUI(eUIGameObjectName.BlurImage, true, true);
        }
    
        private void OnDisable() {
            if (!Application.isPlaying)
                return;
            if (DialogueManager.Instance?.isDialogueActive == false)
                UIManager.Instance?.SetUI(eUIGameObjectName.BlurImage, false, true);
        }
    }
}
