using System;

namespace Umbrella.Infrastructure.Cache
{
    /// <summary>
    /// Abstraction of a given entry of cache persistence
    /// </summary>
    public interface ICacheEntry
    {
        /// <summary>
        /// Unique Key of the given entry
        /// </summary>
        string Key { get; }
        /// <summary>
        /// value of the key. Remember that it can be null as well
        /// </summary>
        object? Value { get; }
        /// <summary>
        /// Creation date of entry inside eprsistence
        /// </summary>
        DateTime CreatedOn { get; }
        /// <summary>
        /// Cheks for expiration.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns>TRUE if entry has already expired the given minutes, FALSE otherwise</returns>
        bool HasExpired(int minutes);
        /// <summary>
        /// Updates the value of cache entry
        /// </summary>
        /// <remarks>
        /// Also cration date is set to Now
        /// </remarks>
        /// <param name="value"></param>
        void UpdateValue(object? value);
    }
}
