using System.Collections;
using UnityEngine;
using Fate.Utilities;
using static Fate.Utilities.Constants;

namespace Fate.Managers
{
    // Game state and UI handlers
    public class SetGenderGirlHandler : ExactMatchResultHandler
    {
        public SetGenderGirlHandler() : base("Result_girl") { }

        public override void Execute(string resultID)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetVariable("AccidyGender", 0);
            else
                Debug.LogWarning("ResultManager: GameManager.Instance is null, cannot set AccidyGender");
        }
    }

    public class SetGenderBoyHandler : ExactMatchResultHandler
    {
        public SetGenderBoyHandler() : base("Result_boy") { }

        public override void Execute(string resultID)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetVariable("AccidyGender", 1);
            else
                Debug.LogWarning("ResultManager: GameManager.Instance is null, cannot set AccidyGender");
        }
    }

    public class CloseEyesHandler : ExactMatchResultHandler
    {
        public CloseEyesHandler() : base("ResultCloseEyes") { }

        public override void Execute(string resultID)
        {
            if (UIManager.Instance != null)
                ResultManager.Instance.StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, 1, true, 0.5f));
            else
                Debug.LogWarning("ResultManager: UIManager.Instance is null, cannot fade");
        }
    }

    public class FadeOutHandler : ExactMatchResultHandler
    {
        public FadeOutHandler() : base("Result_FadeOut") { }

        public override void Execute(string resultID)
        {
            float fadeOutTime = 3f;
            if (UIManager.Instance != null)
                ResultManager.Instance.StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, fadeOutTime));
            else
                Debug.LogWarning("ResultManager: UIManager.Instance is null, cannot fade out");
        }
    }

    public class FadeInHandler : ExactMatchResultHandler
    {
        public FadeInHandler() : base("Result_FadeIn") { }

        public override void Execute(string resultID)
        {
            float fadeInTime = 3f;
            if (UIManager.Instance != null)
                ResultManager.Instance.StartCoroutine(UIManager.Instance.OnFade(null, 1, 0, fadeInTime));
            else
                Debug.LogWarning("ResultManager: UIManager.Instance is null, cannot fade in");
        }
    }

    public class PrologueLimitHandler : ExactMatchResultHandler
    {
        public PrologueLimitHandler() : base("ResultPrologueLimit") { }

        public override void Execute(string resultID)
        {
            if (DialogueManager.Instance != null)
                ResultManager.Instance.StartCoroutine(DialogueManager.Instance.StartDialogue("Prologue_000", 3));
            else
                Debug.LogWarning("ResultManager: DialogueManager.Instance is null, cannot start dialogue");
        }
    }

    public class CommonPrologueAHandler : ExactMatchResultHandler
    {
        public CommonPrologueAHandler() : base("ResultCommonPrologueA") { }

        public override void Execute(string resultID)
        {
            if (LobbyManager.Instance != null)
            {
                if (LobbyManager.Instance.backgroundImage != null && LobbyManager.Instance.room1Side1BackgroundSprite != null)
                    LobbyManager.Instance.backgroundImage.sprite = LobbyManager.Instance.room1Side1BackgroundSprite;
                if (DialogueManager.Instance != null)
                    ResultManager.Instance.StartCoroutine(DialogueManager.Instance.StartDialogue("Prologue_002", 3));
                else
                    Debug.LogWarning("ResultManager: DialogueManager.Instance is null, cannot start dialogue");
            }
            else
                Debug.LogWarning("ResultManager: LobbyManager.Instance is null");
        }
    }

    public class NamePanelHandler : ExactMatchResultHandler
    {
        public NamePanelHandler() : base("ResultName") { }

        public override void Execute(string resultID)
        {
            if (LobbyManager.Instance != null)
                LobbyManager.Instance.OpenNamePanel();
            else
                Debug.LogWarning("ResultManager: LobbyManager.Instance is null, cannot open name panel");
        }
    }

    public class BirthPanelHandler : ExactMatchResultHandler
    {
        public BirthPanelHandler() : base("ResultBirth") { }

        public override void Execute(string resultID)
        {
            if (LobbyManager.Instance != null)
                LobbyManager.Instance.OpenBirthPanel();
            else
                Debug.LogWarning("ResultManager: LobbyManager.Instance is null, cannot open birth panel");
        }
    }

    public class PrologueEndHandler : ExactMatchResultHandler
    {
        public PrologueEndHandler() : base("ResultPrologueEnd") { }

        public override void Execute(string resultID)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.isPrologueInProgress = false;
            if (GameSceneManager.Instance != null)
                GameSceneManager.Instance.LoadScene(SceneType.ROOM_1);
            else
                Debug.LogWarning("ResultManager: GameSceneManager.Instance is null, cannot load scene");
        }
    }

    public class TimePassHandler : ExactMatchResultHandler
    {
        public TimePassHandler() : base("ResultTimePass") { }

        public override void Execute(string resultID)
        {
            if (SoundPlayer.Instance != null)
                SoundPlayer.Instance.UISoundPlay(Sound_HeartPop);
            if (RoomManager.Instance != null && RoomManager.Instance.actionPointManager != null)
                RoomManager.Instance.actionPointManager.DecrementActionPoint();
            else
                Debug.LogWarning("ResultManager: RoomManager.Instance or actionPointManager is null, cannot decrement action point");
        }
    }
}
