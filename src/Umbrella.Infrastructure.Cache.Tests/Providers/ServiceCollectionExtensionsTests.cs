using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using Umbrella.Infrastructure.Cache.Providers;
using Umbrella.Infrastructure.Cache.Settings;


namespace Umbrella.Infrastructure.Cache.Tests.Providers
{
    public class ServiceCollectionExtensionsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddCache_ThrowEx_IfServicesIsNull()
        {
            //********* GIVEN
            IServiceCollection services = null;
            UmbrellaCacheSettings settings = new UmbrellaCacheSettings();

            //********* WHEN
            TestDelegate testCode = () => services.AddCache(settings);

            //********* ASSERT
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("services"));
            Assert.Pass();
        }

        [Test]
        public void AddCache_ThrowEx_IfSettingsIsNull()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            UmbrellaCacheSettings settings = null;

            //********* WHEN
            TestDelegate testCode = () => services.AddCache(settings);

            //********* ASSERT
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("settings"));
            Assert.Pass();
        }

        [Test]
        public void AddCache_ThrowEx_IfConfigIsNull()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            IConfiguration config = null;

            //********* WHEN
            TestDelegate testCode = () => services.AddCache(config);

            //********* ASSERT
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("config"));
            Assert.Pass();
        }

        [Test]
        public void AddCache_ThrowEx_IfReadSettingsAreInvalid()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            var path = Path.Combine(Environment.CurrentDirectory, "appSettings.Test.Invalid.json");
            IConfiguration config = new ConfigurationManager().InitConfigurationFromFile(path);

            //********* WHEN
            TestDelegate testCode = () => services.AddCache(config);

            //********* ASSERT
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(testCode);
            Assert.That(ex.Message, Is.EqualTo("Wrong Configuration: MinutesLifeTimeDuration is wrong"));
            Assert.Pass();
        }

        [Test]
        public void AddCache_Injects_IMemoryCache()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            services.AddTransient(x =>
            {
                var logger = new Mock<ILogger>();
                return logger.Object;
            });
            UmbrellaCacheSettings settings = new UmbrellaCacheSettings();

            //********* WHEN
            services.AddCache(settings);

            //********* ASSERT
            var provider = services.BuildServiceProvider();
            var cache = provider.GetService<IMemoryCache>();    
            Assert.IsNotNull(cache);
            Assert.IsInstanceOf<DictionaryMemoryCache>(cache);
            Assert.Pass();
        }
    }
}
