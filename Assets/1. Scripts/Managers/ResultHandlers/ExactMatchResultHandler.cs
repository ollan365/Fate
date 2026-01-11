namespace Fate.Managers
{
    /// <summary>
    /// Base class for handlers that match results by exact string match
    /// </summary>
    public abstract class ExactMatchResultHandler : IResultHandler
    {
        protected readonly string exactMatch;

        protected ExactMatchResultHandler(string exactMatch)
        {
            this.exactMatch = exactMatch;
        }

        public virtual bool CanHandle(string resultID)
        {
            return resultID == exactMatch;
        }

        public abstract void Execute(string resultID);
    }
}
