// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.Hosting
{
    /// <summary>
    /// Define a method to shut down the running application.
    /// </summary>
    public interface IShutdownHandler
    {
        /// <summary>
        /// Setup a way to signal application shutdown.
        /// </summary>
        /// <param name="applicationShutdown">The application shutdown.</param>
        void Setup(IApplicationShutdown applicationShutdown);
    }
}
