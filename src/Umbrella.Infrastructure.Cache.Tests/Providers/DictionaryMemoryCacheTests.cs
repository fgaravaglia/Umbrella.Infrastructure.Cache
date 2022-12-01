using Microsoft.Extensions.Logging;
using Umbrella.Infrastructure.Cache.Providers;
using Umbrella.Infrastructure.Cache.Settings;
using Moq;

namespace Umbrella.Infrastructure.Cache.Tests.Providers
{
    public class DictionaryMemoryCacheTests : BaseCacheProviderTests
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
            this._Cache = new DictionaryMemoryCache(logger.Object, settings.MinutesLifeTimeDuration, settings.AdmitNullValues);
        }
    }
}
