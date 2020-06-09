﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.DailyOffers.Services;
using Nop.Services.Catalog;
using Nop.Plugin.Widgets.DailyOffers.Infrastructure.Cache;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.DailyOffers.Helpers
{
   public static class SelectedListHelper
    {
        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="categoryService">Category service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category list</returns>
        public static List<SelectListItem> GetOfferList(IOfferService offerService, bool showHidden = false)
        {
            if (offerService == null)
                throw new ArgumentNullException(nameof(offerService));


            //var cacheKey = string.Format(ModelCacheEventConsumer.OFFERS_LIST_KEY, showHidden);
            //var listItems = cacheManager.Get(cacheKey, () =>
            //{
                var offers = offerService.GetAllOffers(showHidden: showHidden);
              var listItems=   offers.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                });
          //  });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        public static List<SelectListItem> GetPublicWidgetZones()
        {

            var t = typeof(PublicWidgetZones);
            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in t.GetProperties())
            {
                    result.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.GetValue(null).ToString()
                    });

            }

            return result;
        }

    }
}
