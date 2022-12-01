using NUnit.Framework;
using System;

namespace Umbrella.Infrastructure.Cache.Tests
{
    public class CacheEntryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor_ThrowEx_IfKeyIsNull()
        {
            //********* GIVEN
            string key = "";
            object value = 33;

            //********* WHEN
            TestDelegate testCode = () => new CacheEntry(key, value);

            //********* ASSERT
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("key"));
            Assert.Pass();
        }

        [Test]
        public void Constructor_SetsTheCreationDate()
        {
            //********* GIVEN
            string key = "x";
            object value = 33;

            //********* WHEN
            var entry = new CacheEntry(key, value);

            //********* ASSERT
            Assert.True(entry.CreatedOn != DateTime.MinValue);
            Assert.Pass();
        }
    }
}