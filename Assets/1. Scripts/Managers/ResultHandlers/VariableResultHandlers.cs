using UnityEngine;
using static Fate.Utilities.Constants;

namespace Fate.Managers
{
    // Variable manipulation handlers
    public class RevealMemoHandler : PrefixResultHandler
    {
        public RevealMemoHandler() : base("Result_RevealMemo") { }

        public override void Execute(string resultID)
        {
            string variableName = ExtractVariableName(resultID);
            if (MemoManager.Instance != null)
                MemoManager.Instance.RevealMemo(variableName);
            else
                Debug.LogWarning($"ResultManager: MemoManager.Instance is null, cannot reveal memo '{variableName}'");
        }
    }

    public class StartDialogueHandler : PrefixResultHandler
    {
        public StartDialogueHandler() : base("Result_StartDialogue") { }

        public override void Execute(string resultID)
        {
            string variableName = ExtractVariableName(resultID);
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.StartDialogue(variableName);
            else
                Debug.LogWarning($"ResultManager: DialogueManager.Instance is null, cannot start dialogue '{variableName}'");
        }
    }

    public class IncrementVariableHandler : PrefixResultHandler
    {
        public IncrementVariableHandler() : base("Result_Increment") { }

        public override void Execute(string resultID)
        {
            string variableName = ExtractVariableName(resultID);
            if (string.IsNullOrEmpty(variableName))
            {
                Debug.LogWarning($"ResultManager: Cannot extract variable name from resultID '{resultID}'");
                return;
            }
            
            if (GameManager.Instance != null)
                GameManager.Instance.IncrementVariable(variableName);
            else
                Debug.LogWarning($"ResultManager: GameManager.Instance is null, cannot increment variable '{variableName}'");
        }
    }

    public class DecrementVariableHandler : PrefixResultHandler
    {
        public DecrementVariableHandler() : base("Result_Decrement") { }

        public override void Execute(string resultID)
        {
            string variableName = ExtractVariableName(resultID);
            if (GameManager.Instance != null)
                GameManager.Instance.DecrementVariable(variableName);
            else
                Debug.LogWarning($"ResultManager: GameManager.Instance is null, cannot decrement variable '{variableName}'");
        }
    }

    public class InverseVariableHandler : PrefixResultHandler
    {
        public InverseVariableHandler() : base("Result_Inverse") { }

        public override void Execute(string resultID)
        {
            string variableName = ExtractVariableName(resultID);
            if (GameManager.Instance != null)
                GameManager.Instance.InverseVariable(variableName);
            else
                Debug.LogWarning($"ResultManager: GameManager.Instance is null, cannot inverse variable '{variableName}'");
        }
    }

    public class SetEventFinishedHandler : PrefixResultHandler
    {
        public SetEventFinishedHandler() : base("Result_IsFinished") { }

        public override void Execute(string resultID)
        {
            string variableName = ExtractVariableName(resultID);
            if (GameManager.Instance != null)
                GameManager.Instance.SetEventFinished(variableName);
            else
                Debug.LogWarning($"ResultManager: GameManager.Instance is null, cannot set event finished '{variableName}'");
        }
    }

    public class SetEventUnFinishedHandler : PrefixResultHandler
    {
        public SetEventUnFinishedHandler() : base("Result_IsUnFinished") { }

        public override void Execute(string resultID)
        {
            string variableName = ExtractVariableName(resultID);
            if (GameManager.Instance != null)
                GameManager.Instance.SetEventUnFinished(variableName);
            else
                Debug.LogWarning($"ResultManager: GameManager.Instance is null, cannot set event unfinished '{variableName}'");
        }
    }
}
