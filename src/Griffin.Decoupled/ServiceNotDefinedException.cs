using System;

namespace Griffin.Decoupled
{
    /// <summary>
    /// Exception thrown when a service is not found.
    /// </summary>
    public class ServiceNotDefinedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotDefinedException" /> class.
        /// </summary>
        /// <param name="missingService">The missing service.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ServiceNotDefinedException(Type missingService)
            : base(string.Format("Failed to find service '{0}'.", missingService.FullName))
        {
            if (missingService == null) throw new ArgumentNullException("missingService");
            MissingService = missingService;
        }

        /// <summary>
        /// Gets missing service
        /// </summary>
        public Type MissingService { get; private set; }
    }
}