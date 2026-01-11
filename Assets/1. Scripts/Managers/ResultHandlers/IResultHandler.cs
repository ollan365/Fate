namespace Fate.Managers
{
    /// <summary>
    /// Interface for result handlers that process specific result types
    /// </summary>
    public interface IResultHandler
    {
        /// <summary>
        /// Checks if this handler can process the given result ID
        /// </summary>
        bool CanHandle(string resultID);

        /// <summary>
        /// Executes the result handler
        /// </summary>
        void Execute(string resultID);
    }
}
