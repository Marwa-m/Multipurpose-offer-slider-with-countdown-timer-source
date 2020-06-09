using Nop.Core.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers.Services
{
   public static partial class NopOfferDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// {2} : show hidden records?
        /// </remarks>
        //public static CacheKey OffersAllCacheKey => new CacheKey("Nop.offer.all-{0}-{1}-{2}", OffersAllPrefixCacheKey);

        ///// <summary>
        ///// Gets a key pattern to clear cache
        ///// </summary>
        //public static string OffersAllPrefixCacheKey => "Nop.offer.all";

        /////{0}: WidgetZoneName
        /////{1}: UrlRecordId
        ///// {2} : current store ID
        ///// {3} : roles of the current user
        //public static CacheKey GetOffersByWidgetZoneNameAndUrlRecordId => new CacheKey("Nop.pres.offers.bywidgetzoneurl-{0}-{1}-{2}-{3}", OffersByWidgetZoneNameAndUrlRecordIdPrefixCacheKey);
        //public static string OffersByWidgetZoneNameAndUrlRecordIdPrefixCacheKey => "Nop.pres.offers.bywidgetzoneurl";

        /////{0}: OfferId
        ///// {1} : current store ID
        ///// {2} : roles of the current user
        //public static CacheKey GetOfferWidgetZonesByOfferIDKey => new CacheKey("Nop.pres.offerwidgetzones.byofferid-{0}-{1}-{2}", OfferWidgetZonesByOfferIDPrefixCacheKey);
        //public static string OfferWidgetZonesByOfferIDPrefixCacheKey => "Nop.pres.offerwidgetzones.byofferid-{0}-";


        ///{0}: OfferId
        /// {1} : current store ID
        /// {2} : roles of the current user
        public static CacheKey GetOfferProductsByOfferID => new CacheKey("Nop.pres.offerproducts.byofferid-{0}-{1}-{2}", OfferProductsByOfferIDPrefixCacheKey);
        public static string OfferProductsByOfferIDPrefixCacheKey => "Nop.pres.offerproducts.byofferid";

        ///{0}: OfferId
        /// {1} : current store ID
        /// {2} : roles of the current user
        //public static CacheKey GetOfferUrlsByOfferID => new CacheKey("Nop.pres.offerurls.byofferid-{0}-{1}-{2}", OfferUrlsByOfferIDPrefixCacheKey);
        //public static string OfferUrlsByOfferIDPrefixCacheKey => "Nop.pres.offerurls.byofferid";

    }
}
