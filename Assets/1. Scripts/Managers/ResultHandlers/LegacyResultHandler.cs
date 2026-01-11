using System.Collections;
using UnityEngine;
using static Fate.Utilities.Constants;
using Random = Unity.Mathematics.Random;

namespace Fate.Managers
{
    /// <summary>
    /// Legacy handler that contains all remaining result cases.
    /// This should be gradually refactored into specific handlers.
    /// This handler is used as a fallback when no specific handler matches.
    /// </summary>
    public class LegacyResultHandler : IResultHandler
    {
        public bool CanHandle(string resultID)
        {
            // This handler can handle any result ID
            // It's used as a fallback by the registry
            return true;
        }

        public void Execute(string resultID)
        {
            // Delegate to the legacy implementation
            if (ResultManager.Instance != null)
                ResultManager.Instance.ExecuteResultLegacy(resultID);
            else
                Debug.LogError("ResultManager: Instance is null in LegacyResultHandler");
        }
    }
}
