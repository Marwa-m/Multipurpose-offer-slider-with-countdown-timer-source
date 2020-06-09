using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers.Models
{
   public partial class OfferUrlRecordModel : BaseNopEntityModel
    {
        public OfferUrlRecordModel()
        {
            UrlRecords = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.UrlRecords.Fields.UrlRecord")]
        public string UrlRecord { get; set; }
        public IList<SelectListItem> UrlRecords { get; set; }
    }
}
