using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.DailyOffers.Models
{
    /// <summary>
    /// Represents a offer post search model
    /// </summary>
    public partial class OfferSearchModel : BaseSearchModel
    {
        #region Ctor

        public OfferSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.List.SearchOfferName")]
        public string SearchOfferName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.List.AvailableStores")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public bool HideStoresList { get; set; }

        #endregion
    }

}
