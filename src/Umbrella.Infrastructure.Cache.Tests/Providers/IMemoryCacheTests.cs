using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Infrastructure.Cache.Tests.Providers
{
    internal interface IMemoryCacheTests
    {
        #region Tests on AddOrUpdateEntry

        void AddOrUpdateEntry_ThrowEx_IfKeyIsNull();
        void AddOrUpdateEntry_ThrowEx_IfValueIsNull_AdnAllowNullIsFalse();
        void AddOrUpdateEntry_DoesNotThrowEx_IfValueIsNull_AndAllowNullIsTrue();
        void AddOrUpdateEntry_AddsKeyIfDoesNotExists();
        void AddOrUpdateEntry_UpdatesKey();
        #endregion

        #region Tests on Exists
        
        void Exists_ThrowEx_IfKeyIsNull();
        void Exists_ReturnsTrue_IfKeyIsInCache();
        void Exists_ReturnsFalse_IfKeyIsNotInCache();
        void Exists_ReturnsFalse_IfKeyIsExpired();

        #endregion

        #region Tests on TryGetEntry

        void TryGetEntry_ThrowEx_IfKeyIsNull();
        void TryGetEntry_ReturnsFalse_IfKeyDoesNotExist();
        void TryGetEntry_ReturnsFalse_IfKeyIsExpired_and_KeyIsDeleted();
        void TryGetEntry_ReturnsCachedValue();

        #endregion

        #region Tests on ForceExpireKey

        void ForceExpireEntry_ThrowEx_IfKeyIsNull();
        void ForceExpireEntry_Removes_Entry();

        #endregion
    }
}
