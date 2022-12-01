using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Umbrella.Infrastructure.Cache
{
    /// <summary>
    /// Concrete implementation of a given entry stored in cache persistence
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CacheEntry : ICacheEntry
    {
        /// <summary>
        /// <inheritdoc cref="ICacheEntry.Key"/>
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// <inheritdoc cref="ICacheEntry.Value"/>
        /// </summary>
        public object? Value { get; private set; }
        /// <summary>
        /// <inheritdoc cref="ICacheEntry.CreatedOn"/>
        /// </summary>
        public DateTime CreatedOn { get; private set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CacheEntry(string key, object? value)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            this.Key = key;
            this.Value = value;
            this.CreatedOn = DateTime.Now;
        }
        /// <summary>
        /// <inheritdoc cref="ICacheEntry.HasExpired(int)"/>
        /// </summary>
        public bool HasExpired(int minutes)
        {
            TimeSpan diff = DateTime.Now - this.CreatedOn;
            return diff.TotalMinutes >= minutes;
        }
        /// <summary>
        /// <inheritdoc cref="ICacheEntry.UpdateValue(object)"/>
        /// </summary>
        public void UpdateValue(object? value)
        {
            this.Value = value;
            this.CreatedOn = DateTime.Now;
        }
    }
}
