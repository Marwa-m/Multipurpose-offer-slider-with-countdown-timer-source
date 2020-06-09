using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Models.Offers
{
   
    public partial class OfferListModel : BaseNopModel
    {
        public OfferListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.List.SearchOfferName")]
        public string SearchOfferName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.List.AvailableStores")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }

}
