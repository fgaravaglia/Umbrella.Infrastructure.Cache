using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Umbrella.Infrastructure.Cache.Settings
{
    /// <summary>
    /// class to map avaialble options to cache
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UmbrellaCacheSettings
    {
        /// <summary>
        /// TRUE if cache can store null value. Default FALSE
        /// </summary>
        public bool AdmitNullValues { get; set; }
        /// <summary>
        /// Duration of cache entry lifetime. Default is 10
        /// </summary>
        public int MinutesLifeTimeDuration { get; set; }
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public UmbrellaCacheSettings()
        { 
            this.AdmitNullValues = false;
            this.MinutesLifeTimeDuration = 10;
        }
        /// <summary>
        /// Trhows an exception if settings are wrong
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void EnsureConfigurationIsValid()
        {
            if (this.MinutesLifeTimeDuration < 0)
                throw new InvalidOperationException($"Wrong Configuration: {nameof(MinutesLifeTimeDuration)} is wrong");
        }
    }
}
