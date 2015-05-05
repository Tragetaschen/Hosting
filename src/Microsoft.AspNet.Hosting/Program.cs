// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

            var context = new HostingContext()
            {
                Configuration = config,
                ServerFactoryLocation = config.Get("server"),
                ApplicationName = config.Get("app")
            };

            var engine = new HostingEngine(_serviceProvider);

            using (engine.Start(context))
            {
                var appShutdownService = context.ApplicationServices.GetRequiredService<IApplicationShutdown>();
                var hostingKeepAlives = context.ApplicationServices.GetRequiredServices<IHostingKeepAlive>();

                foreach (var hostingKeepAlive in hostingKeepAlives)
                    hostingKeepAlive.Setup(appShutdownService);

                appShutdownService.ShutdownRequested.WaitHandle.WaitOne();
            }
        }
    }
}
