using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingPanel : MonoBehaviour
{
    public void OnBlockingPanelClicked() {
        if (!(DialogueManager.Instance && DialogueManager.Instance.isDialogueActive))
            if (HapticManager.Instance) HapticManager.Instance.PlayImpact();
        if (DialogueManager.Instance && DialogueManager.Instance.isDialogueActive) {
            DialogueManager.Instance.OnDialoguePanelClick();
            return;
        }
        
        if (MemoManager.Instance && MemoManager.Instance.isMemoOpen) {
            MemoManager.Instance.OnExit();
            return;
        }

        switch (GameSceneManager.Instance.GetActiveScene()) {
            case Constants.SceneType.START:
                if (GameManager.Instance.isPrologueInProgress == false)
                    UIManager.Instance.OnExitButtonClick();
                break;
            case Constants.SceneType.ROOM_1:
            case Constants.SceneType.ROOM_2:
                RoomManager.Instance.OnExitButtonClick();
                break;
        }
    }
}
