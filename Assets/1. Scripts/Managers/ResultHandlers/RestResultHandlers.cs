using System.Collections;
using UnityEngine;
using Fate.Utilities;
using static Fate.Utilities.Constants;

namespace Fate.Managers
{
    public class RestButtonHandler : ExactMatchResultHandler
    {
        public RestButtonHandler() : base("Result_restButton") { }

        public override void Execute(string resultID)
        {
            // No action needed
        }
    }

    public class RestYesHandler : ExactMatchResultHandler
    {
        public RestYesHandler() : base("Result_restYes") { }

        public override void Execute(string resultID)
        {
            if (SoundPlayer.Instance != null)
                SoundPlayer.Instance.UISoundPlay(Sound_HeartPop);
            if (RoomManager.Instance != null && RoomManager.Instance.actionPointManager != null)
                ResultManager.Instance.StartCoroutine(RoomManager.Instance.actionPointManager.TakeRest());
            else
                Debug.LogWarning("ResultManager: RoomManager or actionPointManager is null");
        }
    }

    public class RestNoHandler : ExactMatchResultHandler
    {
        public RestNoHandler() : base("Result_restNo") { }

        public override void Execute(string resultID)
        {
            // No action needed
        }
    }

    public class StartHomecomingHandler : ExactMatchResultHandler
    {
        public StartHomecomingHandler() : base("Result_StartHomecoming") { }

        public override void Execute(string resultID)
        {
            if (RoomManager.Instance != null && RoomManager.Instance.actionPointManager != null)
                RoomManager.Instance.actionPointManager.RefillHeartsOrEndDay();
            else
                Debug.LogWarning("ResultManager: RoomManager or actionPointManager is null");
        }
    }

    public class NextMorningDayHandler : ExactMatchResultHandler
    {
        public NextMorningDayHandler() : base("Result_NextMorningDay") { }

        public override void Execute(string resultID)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetVariable("isHomeComingComplete", true);
            if (SaveManager.Instance != null)
                SaveManager.Instance.SaveGameData();
            if (RoomManager.Instance != null && RoomManager.Instance.actionPointManager != null)
                ResultManager.Instance.StartCoroutine(RoomManager.Instance.actionPointManager.nextMorningDay());
            else
                Debug.LogWarning("ResultManager: RoomManager or actionPointManager is null");
        }
    }
}
