using System.Collections.Generic;
using UnityEngine;

namespace Fate.Managers
{
    /// <summary>
    /// Registry for result handlers. Handles registration and lookup of handlers.
    /// </summary>
    public class ResultHandlerRegistry
    {
        private readonly List<IResultHandler> handlers = new List<IResultHandler>();

        public void RegisterHandler(IResultHandler handler)
        {
            if (handler != null)
                handlers.Add(handler);
        }

        public void RegisterHandlers(IEnumerable<IResultHandler> handlerList)
        {
            foreach (var handler in handlerList)
            {
                if (handler != null)
                    handlers.Add(handler);
            }
        }

        public IResultHandler FindHandler(string resultID)
        {
            if (string.IsNullOrEmpty(resultID))
                return null;

            // Try to find a handler that can handle this result ID
            // Priority: exact match handlers first, then prefix handlers
            // Skip legacy handler in first pass
            IResultHandler legacyHandler = null;
            
            foreach (var handler in handlers)
            {
                // Skip legacy handler for now
                if (handler is LegacyResultHandler)
                {
                    legacyHandler = handler;
                    continue;
                }
                
                if (handler.CanHandle(resultID))
                    return handler;
            }

            // If no specific handler found, use legacy handler as fallback
            return legacyHandler;
        }

        public void Clear()
        {
            handlers.Clear();
        }
    }
}
