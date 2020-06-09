using Nop.Plugin.Widgets.DailyOffers.Domain;
using Nop.Plugin.Widgets.DailyOffers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers.Factories
{
    public partial interface IOfferModelFactory
    {
        /// <summary>
        /// Prepare country model
        /// </summary>
        /// <param name="model">Country model</param>
        /// <param name="country">Country</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Country model</returns>
        OfferModel PrepareOfferModel(OfferModel model, Offer offer, bool excludeProperties = false);

        OfferSearchModel PrepareOfferSearchModel(OfferSearchModel searchModel);
        OfferListModel PrepareOfferListModel(OfferSearchModel searchModel);

        OfferProductListModel PrepareOfferProductListModel(OfferProductSearchModel searchModel, Offer offer);

        AddProductToOfferListModel PrepareAddProductToOfferListModel(AddProductToOfferSearchModel searchModel);

        OfferWidgetZoneListModel PrepareOfferWidgetZoneListModel(OfferWidgetZoneSearchModel searchModel, Offer offer);

        #region UrlRecord
        OfferUrlRecordListModel PrepareOfferUrlRecordListModel(OfferUrlRecordSearchModel searchModel, Offer revSlider);

        #endregion
    }
}
