using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.DailyOffers.Models
{
    public partial class AddProductToOfferModel : BaseNopModel
    {
        #region Ctor

        public AddProductToOfferModel()
        {
            SelectedProductIds = new List<int>();
        }
        #endregion

        #region Properties

        public int OfferId { get; set; }

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }

}
