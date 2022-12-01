using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using MsftMemory = Microsoft.Extensions.Caching.Distributed;

namespace Umbrella.Infrastructure.Cache.Providers.Microsoft
{
    /// <summary>
    /// Cache implementation using MSFT provider for Distributed Approach
    /// </summary>
    /// <remarks>
    /// to use this implementation you need also Microsoft.Extensions.Caching.StackExchangeRedis package
    /// for more information see <seealso cref="https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-7.0"/>
    /// </remarks>
    internal class MicrosoftDistributedMemoryCache : CacheProvider
    {
        readonly MsftMemory.IDistributedCache _MsftCache;
        readonly List<string> _Keys;
        readonly object _Locker = new object();

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cache"></param>
        /// <param name="admitNullValues"></param>
        /// <param name="lifeTimeDurationInMinutes"></param>
        public MicrosoftDistributedMemoryCache(ILogger logger, MsftMemory.IDistributedCache cache, int lifeTimeDurationInMinutes, bool admitNullValues = false)
            : base(logger, lifeTimeDurationInMinutes, admitNullValues)
        {
            this._MsftCache = cache ?? throw new ArgumentNullException(nameof(cache));
            this._Keys = new List<string>();
        }

        static byte[] ConvertToByte(object? value)
        {
            if (value == null)
                return Array.Empty<byte>();
            else
                return Array.Empty<byte>();
        }

        static object? ConvertToObject(byte[]? value)
        {
            if (value == null)
                return null;
            else
                return new object();
        }

        #region Protected methods
        protected override bool ExistKey(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return this._MsftCache.Get(key) != null;
        }

        protected override void AddEntry(string key, object? value)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromMinutes((double)this.LifeTimeDurationInMinutes));
            lock (_Locker)
            {
                this._MsftCache.Set(key, ConvertToByte(value), cacheEntryOptions);
            }

            this._Keys.Add(key);
        }

        protected override void UpdateEntry(string key, object? value)
        {
            if (this.ExistKey(key))
            {
                this._Logger.LogDebug("Removing old entry to refresh it [{cacheKey}]", key);
                RemoveEntry(key);
            }

            // update the value in cache, refreshing its creation date for lifetime
            AddEntry(key, value);
        }

        protected override ICacheEntry? GetEntry(string key)
        {
            lock (_Locker)
            {
                var valueBytes = this._MsftCache.Get(key);
                return valueBytes != null ? new CacheEntry(key, ConvertToObject(valueBytes)) : null;
            }
        }

        protected override void RemoveEntry(string key)
        {
            lock (_Locker)
            {
                // expired: delete from memory
                this._MsftCache.Remove(key);
                this._Keys.Remove(key);
            }
        }
        #endregion

        /// <summary>
        /// <inheritdoc cref="IMemoryCache.ClearCache"/>
        /// </summary>
        public override void ClearCache()
        {
            this._Keys.Distinct().ToList().ForEach(x =>
            {
                try
                {
                    if (this.GetEntry(x) != null)
                        this._MsftCache.Remove(x);
                }
                catch (Exception ex)
                {
                    this._Logger.LogError(ex, "Unexpected error during clearing entire cache at element {cacheKey}", x);
                }
            });

        }
    }
}
