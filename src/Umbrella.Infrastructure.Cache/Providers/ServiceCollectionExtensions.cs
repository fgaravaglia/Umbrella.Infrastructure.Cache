using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.Infrastructure.Cache.Providers;
using Umbrella.Infrastructure.Cache.Settings;

namespace Umbrella.Infrastructure.Cache.Providers
{
    /// <summary>
    /// Extensions to manage DI for Cache components
    /// </summary>
    public static  class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the cache manager to DI, using default settings and In-Memory approach.
        /// It cannot works for distributed processes / components
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddCache(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var settings = config.GetCacheSettings();
            services.AddCache(settings);
        }

        /// <summary>
        /// Adds the cache manager to DI, using default settings and In-Memory approach.
        /// It cannot works for distributed processes / components
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void AddCache(this IServiceCollection services, UmbrellaCacheSettings settings)
        { 
            if(services == null)
                throw new ArgumentNullException(nameof(services));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            services.AddSingleton<IMemoryCache>(x =>
            {
                var msftLogger = x.GetRequiredService<ILogger>();
                return new DictionaryMemoryCache(msftLogger, settings.MinutesLifeTimeDuration, settings.AdmitNullValues);
            });
        }
    }
}
