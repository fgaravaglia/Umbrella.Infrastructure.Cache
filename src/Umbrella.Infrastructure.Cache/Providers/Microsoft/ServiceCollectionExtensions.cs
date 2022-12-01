using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Umbrella.Infrastructure.Cache.Providers;
using Umbrella.Infrastructure.Cache.Settings;
using MsftMemory = Microsoft.Extensions.Caching.Memory;
using MsftDistributedMemory = Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Umbrella.Infrastructure.Cache.Providers.Microsoft
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
        public static void AddMicrosoftCache(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var settings = config.GetCacheSettings();
            services.AddMicrosoftCache(settings);
        }
        /// <summary>
        /// Adds the cache manager to DI, using default settings and distributed approach with redis cache
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddMicrosoftDistributedRedisCache(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            UmbrellaRedisCacheSettings settings = config.GetCacheSettings();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = settings.ConnectionString;
                options.InstanceName = settings.InstanceName;
            });
            services.AddSingleton<IMemoryCache>(x =>
            {
                var msftLogger = x.GetRequiredService<ILogger>();
                var msftCache = x.GetRequiredService<MsftDistributedMemory.IDistributedCache>();
                return new MicrosoftDistributedMemoryCache(msftLogger, msftCache, settings.MinutesLifeTimeDuration, settings.AdmitNullValues);
            });
        }
        /// <summary>
        /// Adds the cache manager to DI, using default settings and In-Memory approach.
        /// It cannot works for distributed processes / components
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void AddMicrosoftCache(this IServiceCollection services, UmbrellaCacheSettings settings)
        { 
            if(services == null)
                throw new ArgumentNullException(nameof(services));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            services.AddMemoryCache();
            services.AddSingleton<IMemoryCache>(x =>
            {
                var msftLogger = x.GetRequiredService<ILogger>();
                var msftCache = x.GetRequiredService<MsftMemory.IMemoryCache>();
                return new MicrosoftMemoryCache(msftLogger, msftCache, settings.MinutesLifeTimeDuration, settings.AdmitNullValues);
            });
        }
    }
}
