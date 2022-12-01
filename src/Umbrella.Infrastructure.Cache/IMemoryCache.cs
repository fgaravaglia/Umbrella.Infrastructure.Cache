using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Umbrella.Infrastructure.Cache.Tests")]

namespace Umbrella.Infrastructure.Cache
{
    /// <summary>
    /// Abstraction for Caching
    /// </summary>
    public interface IMemoryCache
    {
        /// <summary>
        /// Duration of lifetime expressed in minutes
        /// </summary>
        int LifeTimeDurationInMinutes { get; }
        /// <summary>
        /// It Creates or Updates the entry in memeory
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entryValue"></param>
        void AddOrUpdateEntry(string key, object entryValue);
        /// <summary>
        ///  It Creates or Updates the entry in memeory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="objectValue"></param>
        void AddOrUpdateObject<T>(string key, T objectValue) where T : new();
        /// <summary>
        /// Checks if entry exists or not
        /// </summary>
        /// <param name="key"></param>
        /// <returns>TRUE if exists, FALSE otherwise</returns>
        bool Exists(string key);
        /// <summary>
        /// Gets the value from cache if it exists
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entryValue">contains the cache value, if there. NULL otherwise</param>
        /// <returns>TRUE if item exists; FALSE otherwise</returns>
        bool TryGetEntry(string key, out object entryValue);
        /// <summary>
        /// Gets the value from cache if it exists
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objectValue">contains the cache value, if there. NULL otherwise</param>
        /// <returns>TRUE if item exists; FALSE otherwise</returns>
        bool TryGetObject<T>(string key, out T objectValue);
        /// <summary>
        /// Force the entry to be expired
        /// </summary>
        /// <param name="key"></param>
        void ForceExpireEntry(string key);
        /// <summary>
        /// Clear all entries
        /// </summary>
        void ClearCache();
    }
}
