using Nop.Plugin.Widgets.DailyOffers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers.Extensions
{
  public static  class OfferExtension
    {
        public static OfferModel.OfferUrlRecordModel FindOfferUrlRecord(this IList<OfferModel.OfferUrlRecordModel> source,
  int UrlRecordId, int offerId)
        {
            foreach (var item in source)
                if (item.UrlRecordId == UrlRecordId && item.OfferId == offerId)
                    return item;

            return null;
        }

    }
}
