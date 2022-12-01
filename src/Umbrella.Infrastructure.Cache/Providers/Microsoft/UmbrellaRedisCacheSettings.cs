using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Umbrella.Infrastructure.Cache.Settings;

namespace Umbrella.Infrastructure.Cache.Providers.Microsoft
{
    [ExcludeFromCodeCoverage]
    public class UmbrellaRedisCacheSettings : UmbrellaCacheSettings
    {
        /// <summary>
        /// Connection string to Redis Instance or Service
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Name of redis Instance
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public UmbrellaRedisCacheSettings() : base()
        {
            this.ConnectionString = "";
            this.InstanceName = "";
        }
        /// <summary>
        /// Trhows an exception if settings are wrong
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public new void EnsureConfigurationIsValid()
        {
            base.EnsureConfigurationIsValid();

            if (String.IsNullOrEmpty(ConnectionString))
                throw new InvalidOperationException($"Wrong Configuration: {nameof(ConnectionString)} is empty");
            if (String.IsNullOrEmpty(InstanceName))
                throw new InvalidOperationException($"Wrong Configuration: {nameof(InstanceName)} is empty");
        }
    }
}
