using UnityEngine;

namespace Fate.Managers
{
    /// <summary>
    /// Base class for handlers that match results by prefix
    /// </summary>
    public abstract class PrefixResultHandler : IResultHandler
    {
        protected readonly string prefix;

        protected PrefixResultHandler(string prefix)
        {
            this.prefix = prefix;
        }

        public virtual bool CanHandle(string resultID)
        {
            return !string.IsNullOrEmpty(resultID) && resultID.StartsWith(prefix);
        }

        public abstract void Execute(string resultID);

        protected string ExtractVariableName(string resultID)
        {
            if (string.IsNullOrEmpty(resultID) || resultID.Length <= prefix.Length)
                return string.Empty;
            return resultID.Substring(prefix.Length);
        }
    }
}
