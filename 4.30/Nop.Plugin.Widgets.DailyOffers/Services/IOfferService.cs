using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Seo;
using Nop.Plugin.Widgets.DailyOffers.Domain;
using Nop.Plugin.Widgets.DailyOffers.Models;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Services
{
    public partial interface IOfferService
    {
        #region OfferUrlRecord
        /// <summary>
        /// Delete OfferWidgetZone
        /// </summary>
        /// <param name="OfferWidgetZone">OfferWidgetZone</param>
        void DeleteOfferUrlRecord(OfferUrlRecord OfferUrlRecord);
        OfferUrlRecord GetOfferUrlRecordById(int OfferUrlRecordId);
        List<UrlRecord> GetOfferUrlRecords();

        void InsertOfferUrlRecord(OfferUrlRecord OfferUrlRecord);

        void UpdateOfferUrlRecord(OfferUrlRecord OfferUrlRecord);

        IPagedList<OfferModel.OfferUrlRecordModel> GetOfferUrlRecordsByOfferId(int offerId,
           int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        #endregion
        #region "OfferWidgetZone"
        /// <summary>
        /// Delete OfferWidgetZone
        /// </summary>
        /// <param name="OfferWidgetZone">OfferWidgetZone</param>
        void DeleteOfferWidgetZone(OfferWidgetZone OfferWidgetZone);



        OfferWidgetZone GetOfferWidgetZoneById(int OfferWidgetZoneId);

        List<string> GetOfferWidgetZones();

        void InsertOfferWidgetZone(OfferWidgetZone OfferWidgetZone);

        void UpdateOfferWidgetZone(OfferWidgetZone OfferWidgetZone);

        #endregion
        /// <summary>
        /// Gets product category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        IPagedList<ProductOffer> GetProductOffersByOfferId(int offerId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        Offer GetOfferById(int offerId);
        ProductOffer GetProductOfferById(int productOfferId);
         void UpdateProductOffer(ProductOffer productOffer);
        void DeleteProductOffer(ProductOffer productOffer);

        IPagedList<Offer> GetAllOffers(string offerName = "",
            int storeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false);
        IList<Offer> GetOffersByWidgetZoneName(string WidgetName = "");
        void InsertProductOffer(ProductOffer productOffer);


        void InsertOffer(Offer Offer);

        void UpdateOffer(Offer Offer);
        void DeleteOffer(Offer offer);

        IPagedList<OfferWidgetZone> GetOfferWidgetZonesByOfferId(int offerId,
           int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
      //  List<ProductDetailsModel> GetProductDetailsByOfferId(int offerId);
        IList<ProductPresenationModel> PrepareProductPresentationModels(int offerId);


    }
}
