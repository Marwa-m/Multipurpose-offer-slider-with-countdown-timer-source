using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Discounts;
using Nop.Core.Events;
using Nop.Plugin.Widgets.DailyOffers.Domain;
using Nop.Plugin.Widgets.DailyOffers.Services;
using Nop.Services.Events;

namespace Nop.Plugin.Widgets.DailyOffers.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        IConsumer<EntityInsertedEvent<Offer>>,
        IConsumer<EntityUpdatedEvent<Offer>>,
        IConsumer<EntityDeletedEvent<Offer>>,
        IConsumer<EntityInsertedEvent<OfferUrlRecord>>,
        IConsumer<EntityUpdatedEvent<OfferUrlRecord>>,
        IConsumer<EntityDeletedEvent<OfferUrlRecord>>,
        IConsumer<EntityInsertedEvent<OfferWidgetZone>>,
        IConsumer<EntityUpdatedEvent<OfferWidgetZone>>,
        IConsumer<EntityDeletedEvent<OfferWidgetZone>>,
        IConsumer<EntityInsertedEvent<ProductOffer>>,
        IConsumer<EntityUpdatedEvent<ProductOffer>>,
        IConsumer<EntityDeletedEvent<ProductOffer>>,
        IConsumer<EntityUpdatedEvent<Discount>>,
        IConsumer<EntityDeletedEvent<Discount>>,
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>
    {
        
        private readonly IStaticCacheManager _cacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<Offer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);


        }
        public void HandleEvent(EntityUpdatedEvent<Offer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Offer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Discount> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);

        }

        public void HandleEvent(EntityDeletedEvent<Discount> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);

        }

        public void HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);

        }

        public void HandleEvent(EntityInsertedEvent<OfferUrlRecord> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<OfferUrlRecord> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<OfferUrlRecord> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }

        public void HandleEvent(EntityInsertedEvent<ProductOffer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductOffer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<ProductOffer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }

        public void HandleEvent(EntityInsertedEvent<OfferWidgetZone> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<OfferWidgetZone> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<OfferWidgetZone> eventMessage)
        {
            _cacheManager.RemoveByPrefix(NopOfferDefaults.OfferProductsByOfferIDPrefixCacheKey);
        }

    }
}