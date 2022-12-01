using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using MsftMemory = Microsoft.Extensions.Caching.Memory;

namespace Umbrella.Infrastructure.Cache.Providers.Microsoft
{
    /// <summary>
    /// Cache impleemtnation using MSFT provider for in-Memory appraoch
    /// </summary>
    internal class MicrosoftMemoryCache : CacheProvider
    {
        readonly MsftMemory.IMemoryCache _MsftCache;
        readonly List<string> _Keys;
        readonly object _Locker = new object();

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cache"></param>
        /// <param name="admitNullValues"></param>
        /// <param name="lifeTimeDurationInMinutes"></param>
        public MicrosoftMemoryCache(ILogger logger, MsftMemory.IMemoryCache cache, int lifeTimeDurationInMinutes, bool admitNullValues = false)
            : base(logger, lifeTimeDurationInMinutes, admitNullValues)
        {
            this._MsftCache = cache ?? throw new ArgumentNullException(nameof(cache));
            this._Keys = new List<string>();
        }

        #region Protected methods
        protected override bool ExistKey(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            object cacheEntryValue;
            return this._MsftCache.TryGetValue(key, out cacheEntryValue);
        }

        protected override void AddEntry(string key, object? value)
        {
            var cacheEntryOptions = new MsftMemory.MemoryCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromMinutes((double)this.LifeTimeDurationInMinutes));
            lock (_Locker)
            {
                this._MsftCache.Set(key, value, cacheEntryOptions);
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
                object cacheEntryValue;
                if (this._MsftCache.TryGetValue(key, out cacheEntryValue))
                    return new CacheEntry(key, cacheEntryValue);
                else
                    return null;
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
