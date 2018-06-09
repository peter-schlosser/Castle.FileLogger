// Copyright (c) Peter Schlosser. All rights reserved.  Licensed under the MIT license. See LICENSE.txt in the project root for license information.
using System;
using Microsoft.Extensions.DependencyInjection;
using Castle.Extensions.Logging.FileLogger;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extensions for adding the <see cref="CastleFileLoggerProvider" /> to the <see cref="ILoggingBuilder" />
    /// </summary>
    public static class LoggerFactoryExtensions
    {
        /// <summary>
        /// Adds the FileLoggerProvider action to the ILoggingBuilder interface.
        /// </summary>
        /// <example>
        /// public static IWebHost BuildWebHost(string[] args) =>
        ///     WebHost.CreateDefaultBuilder(args)
        ///         .ConfigureLogging(logging => logging.AddFile())
        ///         .UseStartup<Startup>()
        ///         .Build();
        /// results in log file named: ~/Logs/log-YYYYMMDD.txt
        /// </example>
        /// <param name="builder">The extension method argument</param>
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, CastleFileLoggerProvider>();
            return builder;
        }

        /// <summary>
        /// Adds the FileLoggerProvider action to the ILoggingBuilder interface setting the <see cref="CastleFileLoggerOptions.FileName"/> property.
        /// </summary>
        /// <param name="builder">The extension method argument</param>
        /// <param name="filename">Sets the filename prefix of log filenames.</param>
        /// <example>
        /// public static IWebHost BuildWebHost(string[] args) =>
        ///     WebHost.CreateDefaultBuilder(args)
        ///         .ConfigureLogging(logging => logging.AddFile("myapp-"))
        ///         .UseStartup<Startup>()
        ///         .Build();
        /// results in log file named: ~/Logs/myapp-YYYYMMDD.txt
        /// </example>
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException(nameof(filename));
            }
            builder.AddFile(options => options.FileName = filename);
            return builder;
        }

        /// <summary>
        /// Adds the FileLoggerProvider action to the ILoggingBuilder interface setting <see cref="CastleFileLoggerOptions"/>.
        /// </summary>
        /// <param name="builder">The extension method argument</param>
        /// <param name="configure">Configures the logging options using <see cref="CastleFileLoggerOptions"/>.</param>
        /// <example>
        /// public static IWebHost BuildWebHost(string[] args) =>
        ///     WebHost.CreateDefaultBuilder(args)
        ///         .ConfigureLogging(logging =>
        ///             logging.AddFile(options =>
        ///             {
        ///                 options.LogDirectory = "Logs";
        ///                 options.FileName = "myapp-";
        ///                 options.FlushPeriod = TimeSpan.FromSeconds(10);
        ///             }))
        ///         .UseStartup<Startup>()
        ///         .Build();
        /// results in log file named: ~/Logs/myapp-YYYYMMDD.txt
        /// </example>
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<CastleFileLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            builder.AddFile();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
