using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Discounts;
using Nop.Core.Events;
using Nop.Plugin.Widgets.DailyOffers.Domain.Offers;
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
        IConsumer<EntityUpdatedEvent<Discount>>,
        IConsumer<EntityDeletedEvent<Discount>>,
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>
    {
        /// <summary>
        /// Key for offers caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string OFFERS_LIST_KEY = "Nop.pres.admin.offers.list-{0}";
        public const string OFFERS_LIST_PATTERN_KEY = "Nop.pres.admin.offers.list";

        public const string PRODUCTOFFER_KEY = "Nop.pres.productoffer.allbyofferid-{0}-{1}-{2}";
        public const string PRODUCTOFFER_PATTERN_KEY = "Nop.pres.productoffer.allbyofferid";

        public const string OFFERS_BY_WIDGET_ZONE_KEY = "Nop.pres.offers.bywidgetzone-{0}-{1}-{2}";
        public const string OFFERS_BY_WIDGET_ZONE_PATTERN_KEY = "Nop.pres.offers.bywidgetzone";
        
        private readonly IStaticCacheManager _cacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<Offer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OFFERS_LIST_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERS_BY_WIDGET_ZONE_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFER_PATTERN_KEY);

        }
        public void HandleEvent(EntityUpdatedEvent<Offer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OFFERS_LIST_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERS_BY_WIDGET_ZONE_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFER_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Offer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OFFERS_LIST_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERS_BY_WIDGET_ZONE_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFER_PATTERN_KEY);
        }

       

        public void HandleEvent(EntityUpdatedEvent<Discount> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OFFERS_BY_WIDGET_ZONE_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<Discount> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OFFERS_BY_WIDGET_ZONE_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OFFERS_LIST_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERS_BY_WIDGET_ZONE_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFER_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OFFERS_LIST_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERS_BY_WIDGET_ZONE_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFER_PATTERN_KEY);
        }
    }
}