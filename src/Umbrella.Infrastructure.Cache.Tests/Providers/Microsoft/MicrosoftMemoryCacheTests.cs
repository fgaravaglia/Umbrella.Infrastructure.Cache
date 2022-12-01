using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbrella.Infrastructure.Cache.Providers.Microsoft;
using MsftMemory = Microsoft.Extensions.Caching.Memory;
using System;
using Umbrella.Infrastructure.Cache.Settings;
using Moq;


namespace Umbrella.Infrastructure.Cache.Tests.Providers.Microsoft
{
    public class MicrosoftMemoryCacheTests : BaseCacheProviderTests
    {
       
        public override void Setup()
        {
            base.Setup();

            InstanceCache(new UmbrellaCacheSettings()
            {
                AdmitNullValues = false,
                MinutesLifeTimeDuration = 1
            });
        }

        protected override void InstanceCache(UmbrellaCacheSettings settings)
        {
            var logger = new Mock<ILogger>();

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var msftCache = services.BuildServiceProvider().GetRequiredService<MsftMemory.IMemoryCache>();
            this._Cache = new MicrosoftMemoryCache( logger.Object, msftCache, settings.MinutesLifeTimeDuration, settings.AdmitNullValues);
        }

    }
}
