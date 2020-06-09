using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Models
{
    public partial class OfferWidgetZoneModel: BaseNopEntityModel
    {
        public OfferWidgetZoneModel()
        {
            WidgetZones = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.WidgetZone.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.WidgetZone.Fields.WidgetZone")]
        public string WidgetZone { get; set; }
        public IList<SelectListItem> WidgetZones { get; set; }
    }
}
