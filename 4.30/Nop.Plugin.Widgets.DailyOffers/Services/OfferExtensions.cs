using Nop.Plugin.Widgets.DailyOffers.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Services
{
    public static class OfferExtensions
    {
        public static ProductOffer FindProductOffer(this IList<ProductOffer> source,
           int productId, int offerId)
        {
            foreach (var productOffer in source)
                if (productOffer.ProductId == productId && productOffer.OfferId == offerId)
                    return productOffer;

            return null;
        }

        public static OfferWidgetZone FindWidgetZoneOffer(this IList<OfferWidgetZone> source,
          string widgetzoneName, int offerId)
        {
            foreach (var widgetzoneOffer in source)
                if (widgetzoneOffer.WidgetZone == widgetzoneName && widgetzoneOffer.OfferId == offerId)
                    return widgetzoneOffer;

            return null;
        }
    }
}
