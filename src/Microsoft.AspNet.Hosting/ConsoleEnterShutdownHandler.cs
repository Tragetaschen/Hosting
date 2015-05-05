using System;
using System.Threading.Tasks;
using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.Hosting
{
    public class ConsoleEnterShutdownHandler : IShutdownHandler
    {
        public void Setup(IApplicationShutdown applicationShutdown)
        {
            Task.Run(() =>
            {
                Console.WriteLine("Started");
                Console.ReadLine();
                applicationShutdown.RequestShutdown();
            });
        }
    }
}
