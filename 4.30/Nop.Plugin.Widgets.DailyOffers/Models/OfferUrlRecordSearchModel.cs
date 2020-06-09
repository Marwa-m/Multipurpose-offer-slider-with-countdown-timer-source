using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers.Models
{
   public partial class OfferUrlRecordSearchModel : BaseSearchModel
    {
        #region Properties

        public int OfferId { get; set; }

        #endregion
    }
}
