using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.Infrastructure.Cache.Providers
{
    /// <summary>
    /// base implementation for CACHE
    /// </summary>
    internal abstract class CacheProvider : IMemoryCache
    {
        #region Fields
        protected readonly bool _AdmitNullValues;
        protected readonly int _LifeTimeDurationInMinutes;
        protected readonly ILogger _Logger;
        #endregion

        #region Properties
        public int LifeTimeDurationInMinutes => this._LifeTimeDurationInMinutes;
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="admitNullValues"></param>
        /// <param name="lifeTimeDurationInMinutes"></param>
        protected CacheProvider(ILogger logger, int lifeTimeDurationInMinutes, bool admitNullValues = false)
        {
            this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));   
            this._AdmitNullValues = admitNullValues;
            this._LifeTimeDurationInMinutes = lifeTimeDurationInMinutes;
        }

        #region Protected methods
        protected abstract bool ExistKey(string key);
        protected abstract void AddEntry(string key, object? value);
        protected abstract void UpdateEntry(string key, object? value);
        protected abstract ICacheEntry? GetEntry(string key);
        protected abstract void RemoveEntry(string key);
        #endregion

        /// <summary>
        /// <inheritdoc cref="IMemoryCache.AddOrUpdateEntry(string, object?)"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entryValue"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddOrUpdateEntry(string key, object? entryValue)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (entryValue is null && !_AdmitNullValues)
                throw new ArgumentNullException(nameof(entryValue));

            if (this.Exists(key))
            {
                // update the value in cache, refreshing its creation date for lifetime
                this._Logger.LogDebug("Updating existing key on cache [{cacheKey}]", key);
                UpdateEntry(key, entryValue);
            }
            else
            {
                // add new entry
                this._Logger.LogDebug("Create new entry on cache [{cacheKey}]", key);
                AddEntry(key, entryValue);
            }
        }
        /// <summary>
        /// <inheritdoc cref="IMemoryCache.AddOrUpdateObject{T}(string, T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="objectValue"></param>
        public void AddOrUpdateObject<T>(string key, T objectValue) where T : new()
        {
            this.AddOrUpdateEntry(key, objectValue);
        }
        /// <summary>
        /// <inheritdoc cref="IMemoryCache.Exists(string)"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Exists(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (!ExistKey(key))
                return false;

            return !GetEntry(key).HasExpired(this._LifeTimeDurationInMinutes);
        }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        /// <summary>
        /// <inheritdoc cref="IMemoryCache.TryGetEntry(string, out object)"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entryValue"></param>
        /// <returns></returns>
        public bool TryGetEntry(string key, out object? entryValue)
        {
            entryValue = null;
            if (!Exists(key))
                return false;
            try
            {
                //check lifetime
                var entry = GetEntry(key);
                if (entry != null && entry.HasExpired(this._LifeTimeDurationInMinutes))
                {
                    // expired: delete from memory
                    this._Logger.LogDebug("removing expired entry on cache [{cacheKey}]", key);
                    RemoveEntry(key);
                    return false;
                }
                else
                {
                    // get value
                    entryValue = entry != null ? entry.Value : null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex, "Unable to Get Entry for {cacheKey}", key);
                return false;
            }
        }
        /// <summary>
        /// <inheritdoc cref="IMemoryCache.TryGetObject{T}(string, out T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        public bool TryGetObject<T>(string key, out T objectValue)
        {
            objectValue = default(T);
            object output;
            bool exists = TryGetEntry(key, out output);
            if (exists)
            {
                objectValue = (T)output;

            }

            return exists;
        }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).

        /// <summary>
        /// <inheritdoc cref="IMemoryCache.ForceExpireEntry(string)"/>
        /// </summary>
        /// <param name="key"></param>
        public void ForceExpireEntry(string key)
        {
            if (!Exists(key))
                return;
            try
            {
                RemoveEntry(key);
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex, "Unexpected Error during removing key from cache [{cacheKey}]", key);
            }
        }
        /// <summary>
        /// <inheritdoc cref="IMemoryCache.ClearCache"/>
        /// </summary>
        public abstract void ClearCache();
    }
}
