using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Events;
using Nop.Plugin.Widgets.DailyOffers.Domain.Offers;
using Nop.Services.Events;

namespace Nop.Plugin.Widgets.DailyOffers.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        IConsumer<EntityInserted<Offer>>,
        IConsumer<EntityUpdated<Offer>>,
        IConsumer<EntityDeleted<Offer>>
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

        public void HandleEvent(EntityInserted<Offer> eventMessage)
        {
            _cacheManager.RemoveByPattern(OFFERS_LIST_PATTERN_KEY);
            _cacheManager.RemoveByPattern(OFFERS_BY_WIDGET_ZONE_KEY);
            _cacheManager.RemoveByPattern(PRODUCTOFFER_KEY);

        }
        public void HandleEvent(EntityUpdated<Offer> eventMessage)
        {
            _cacheManager.RemoveByPattern(OFFERS_LIST_PATTERN_KEY);
            _cacheManager.RemoveByPattern(OFFERS_BY_WIDGET_ZONE_KEY);
            _cacheManager.RemoveByPattern(PRODUCTOFFER_KEY);
        }
        public void HandleEvent(EntityDeleted<Offer> eventMessage)
        {
            _cacheManager.RemoveByPattern(OFFERS_LIST_PATTERN_KEY);
            _cacheManager.RemoveByPattern(OFFERS_BY_WIDGET_ZONE_KEY);
            _cacheManager.RemoveByPattern(PRODUCTOFFER_KEY);
        }
    }
}