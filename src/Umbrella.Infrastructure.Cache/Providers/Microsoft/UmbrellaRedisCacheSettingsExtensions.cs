using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Umbrella.Infrastructure.Cache.Providers.Microsoft
{
    /// <summary>
    /// Extensions to amnage with settings for cache
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class UmbrellaRedisCacheSettingsExtensions
    {
        internal const string SECTION_NAME = "UmbrellaCache";


        /// <summary>
        /// Reads the appSettings section for CACHE configuration
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static UmbrellaRedisCacheSettings GetCacheSettings(this IConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // read the configuration
            UmbrellaRedisCacheSettings settings = new UmbrellaRedisCacheSettings();
            var section = config.GetSection(SECTION_NAME);
            if (section == null)
                throw new InvalidOperationException($"Wrong Configuration: section '{SECTION_NAME}' is missing");

            // read and validate settings
            section.Bind(settings);
            settings.EnsureConfigurationIsValid();

            return settings;
        }
    }
}
