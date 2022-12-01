using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Infrastructure.Cache.Settings;

namespace Umbrella.Infrastructure.Cache.Tests.Providers
{
    public abstract class BaseCacheProviderTests : IMemoryCacheTests
    {
        protected IMemoryCache _Cache;

        [SetUp]
        public virtual void Setup()
        {
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (this._Cache != null)
                this._Cache.ClearCache();
        }


        #region Tests on AddOrUpdateEntry
        [Test]
        public virtual void AddOrUpdateEntry_ThrowEx_IfKeyIsNull()
        {
            //********* GIVEN
            string key = "";
            object value = 33;

            //********* WHEN
            TestDelegate testCode = () => this._Cache.AddOrUpdateEntry(key, value);

            //********* ASSERT
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("key"));
            Assert.Pass();
        }

        protected abstract void InstanceCache(UmbrellaCacheSettings settings);


        [Test]
        public virtual void AddOrUpdateEntry_ThrowEx_IfValueIsNull_AdnAllowNullIsFalse()
        {
            //********* GIVEN
            string key = "x";
            object value = null;
            InstanceCache(new UmbrellaCacheSettings()
            {
                AdmitNullValues = false
            });

            //********* WHEN
            TestDelegate testCode = () => this._Cache.AddOrUpdateEntry(key, value);

            //********* ASSERT
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("entryValue"));
            Assert.Pass();
        }

        [Test]
        public virtual void AddOrUpdateEntry_DoesNotThrowEx_IfValueIsNull_AndAllowNullIsTrue()
        {
            //********* GIVEN
            string key = "x";
            object value = null;
            InstanceCache(new UmbrellaCacheSettings()
            {
                AdmitNullValues = true
            });

            //********* WHEN
            this._Cache.AddOrUpdateEntry(key, value);

            //********* ASSERT
            Assert.Pass();
        }

        [Test]
        public virtual void AddOrUpdateEntry_AddsKeyIfDoesNotExists()
        {
            //********* GIVEN
            string key = "notExist";
            object value = 333;

            //********* WHEN
            this._Cache.AddOrUpdateEntry(key, value);

            //********* ASSERT
            Assert.True(this._Cache.Exists(key));
            Assert.Pass();
        }

        [Test]
        public virtual void AddOrUpdateEntry_UpdatesKey()
        {
            //********* GIVEN
            string key = "x";
            int value = 333;
            this._Cache.AddOrUpdateEntry(key, 22);

            //********* WHEN
            this._Cache.AddOrUpdateEntry(key, value);

            //********* ASSERT
            int cacheValue;
            bool exists = this._Cache.TryGetObject<int>(key, out cacheValue);
            Assert.True(exists);
            Assert.That(cacheValue, Is.EqualTo(value));
            Assert.Pass();
        }
        #endregion


        #region Tests on Exists
        [Test]
        public virtual void Exists_ThrowEx_IfKeyIsNull()
        {
            //********* GIVEN
            string key = "";

            //********* WHEN
            TestDelegate testCode = () => this._Cache.Exists(key);

            //********* ASSERT
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("key"));
            Assert.Pass();
        }

        [Test]
        public virtual void Exists_ReturnsTrue_IfKeyIsInCache()
        {
            //********* GIVEN
            string key = "x";
            this._Cache.AddOrUpdateEntry(key, 22);

            //********* WHEN
            var exists = this._Cache.Exists(key);

            //********* ASSERT
            Assert.True(exists);
            Assert.Pass();
        }

        [Test]
        public virtual void Exists_ReturnsFalse_IfKeyIsNotInCache()
        {
            //********* GIVEN
            string key = "NotExisting";

            //********* WHEN
            var exists = this._Cache.Exists(key);

            //********* ASSERT
            Assert.False(exists);
            Assert.Pass();
        }

        [Test]
        public virtual void Exists_ReturnsFalse_IfKeyIsExpired()
        {
            //********* GIVEN
            string key = "x";
            this._Cache.AddOrUpdateEntry(key, 22);
            Thread.Sleep(60 * 2 * 1000);

            //********* WHEN
            var exists = this._Cache.Exists(key);

            //********* ASSERT
            Assert.False(exists);
            Assert.Pass();
        }
        #endregion

        #region Tests on TryGetEntry

        [Test]
        public virtual void TryGetEntry_ThrowEx_IfKeyIsNull()
        {
            //********* GIVEN
            string key = "";

            //********* WHEN
            object cachedValue;
            TestDelegate testCode = () => this._Cache.TryGetEntry(key, out cachedValue);

            //********* ASSERT
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("key"));
            Assert.Pass();
        }

        [Test]
        public virtual void TryGetEntry_ReturnsFalse_IfKeyDoesNotExist()
        {
            //********* GIVEN
            string key = "NotExisting";

            //********* WHEN
            object cachedValue;
            var exists = this._Cache.TryGetEntry(key, out cachedValue);

            //********* ASSERT
            Assert.False(exists);
            Assert.Pass();
        }

        [Test]
        public virtual void TryGetEntry_ReturnsFalse_IfKeyIsExpired_and_KeyIsDeleted()
        {
            //********* GIVEN
            string key = "x";
            this._Cache.AddOrUpdateEntry(key, 22);
            Thread.Sleep(60 * 2 * 1000);

            //********* WHEN
            object cachedValue;
            var exists = this._Cache.TryGetEntry(key, out cachedValue);

            //********* ASSERT
            Assert.False(exists);
            Assert.False(this._Cache.Exists(key));
            Assert.Pass();
        }

        [Test]
        public virtual void TryGetEntry_ReturnsCachedValue()
        {
            //********* GIVEN
            string key = "x";
            this._Cache.AddOrUpdateEntry(key, 22);

            //********* WHEN
            object cachedValue;
            var exists = this._Cache.TryGetEntry(key, out cachedValue);

            //********* ASSERT
            Assert.True(exists);
            Assert.That(cachedValue, Is.EqualTo(22));
            Assert.Pass();
        }
        #endregion

        #region Tests on ForceExpireKey

        [Test]
        public virtual void ForceExpireEntry_ThrowEx_IfKeyIsNull()
        {
            //********* GIVEN
            string key = "";

            //********* WHEN
            TestDelegate testCode = () => this._Cache.ForceExpireEntry(key);

            //********* ASSERT
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("key"));
            Assert.Pass();
        }

        [Test]
        public virtual void ForceExpireEntry_Removes_Entry()
        {
            //********* GIVEN
            string key = "x";
            this._Cache.AddOrUpdateEntry(key, 22);

            //********* WHEN
            this._Cache.ForceExpireEntry(key);

            //********* ASSERT
            Assert.False(this._Cache.Exists(key));
            Assert.Pass();
        }

        #endregion
    }
}
