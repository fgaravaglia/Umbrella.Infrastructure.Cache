using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.Infrastructure.Cache.Providers
{
    /// <summary>
    /// Cache implement by in-memory Dictionary
    /// </summary>
    /// <remarks>
    /// Consider this implementation just for POC purposes. Do not use it on production Evironments
    /// </remarks>
    internal class DictionaryMemoryCache : CacheProvider
    {
        #region Fields
        readonly Dictionary<string, ICacheEntry> _Memory;
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="admitNullValues"></param>
        /// <param name="lifeTimeDurationInMinutes"></param>
        public DictionaryMemoryCache(ILogger logger, int lifeTimeDurationInMinutes, bool admitNullValues = false) 
            : base(logger, lifeTimeDurationInMinutes, admitNullValues)
        {
            this._Memory = new Dictionary<string, ICacheEntry>();
        }

        #region Protected methods

        protected override bool ExistKey(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return this._Memory.ContainsKey(key);
        }

        protected override void AddEntry(string key, object? value)
        {
            this._Memory.Add(key, new CacheEntry(key, value));
        }

        protected override void UpdateEntry(string key, object? value)
        {
            // update the value in cache, refreshing its creation date for lifetime
            this._Memory[key].UpdateValue(value);
        }

        protected override ICacheEntry? GetEntry(string key)
        {
            if (this._Memory.ContainsKey(key))
                return this._Memory[key];

            return null;
        }

        protected override void RemoveEntry(string key)
        {
            // expired: delete from memory
            this._Memory.Remove(key);
        }
        #endregion

        /// <summary>
        /// <inheritdoc cref="IMemoryCache.ClearCache"/>
        /// </summary>
        public override void ClearCache()
        {
            try
            {
                this._Memory.Clear();
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex, "Unexpected error during clearing entire cache");
            }
            
        }
    }
}
