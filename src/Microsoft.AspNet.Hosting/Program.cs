// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting.Internal;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.Hosting
{
    public class Program
    {
        private const string HostingIniFile = "Microsoft.AspNet.Hosting.ini";

        private readonly IServiceProvider _serviceProvider;

        public Program(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Main(string[] args)
        {
            var config = new Configuration();
            if (File.Exists(HostingIniFile))
            {
                config.AddIniFile(HostingIniFile);
            }
            config.AddEnvironmentVariables();
            config.AddCommandLine(args);

            var host = new WebHostBuilder(_serviceProvider, config)
                .UseServices(x => x.AddSingleton<IShutdownHandler, ConsoleEnterShutdownHandler>())
                .Build();
            using (host.Start())
            {
                var appShutdownService = host.ApplicationServices.GetRequiredService<IApplicationShutdown>();
                var shutdownHandlers = host.ApplicationServices.GetRequiredServices<IShutdownHandler>();

                foreach (var shutdownHandler in shutdownHandlers)
                {
                    shutdownHandler.Setup(appShutdownService);
                }

                appShutdownService.ShutdownRequested.WaitHandle.WaitOne();
            }
        }
    }
}
