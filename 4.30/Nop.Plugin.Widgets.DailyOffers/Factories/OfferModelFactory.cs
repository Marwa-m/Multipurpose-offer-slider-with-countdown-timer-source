using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.DailyOffers.Domain;
using Nop.Plugin.Widgets.DailyOffers.Extensions;
using Nop.Plugin.Widgets.DailyOffers.Models;
using Nop.Plugin.Widgets.DailyOffers.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers.Factories
{
    public partial class OfferModelFactory:IOfferModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IOfferService _offerService;
        private readonly IProductService _productService;
        public OfferModelFactory(ILocalizationService localizationService, ILocalizedModelFactory localizedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,IOfferService offerService,IProductService productService
             )
        {
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _offerService = offerService;
            _productService = productService;
        }

        #endregion
        #region Ctor

        #endregion
        #region Utilities

        /// <summary>
        /// Prepare category product search model
        /// </summary>
        /// <param name="searchModel">Category product search model</param>
        /// <param name="category">Category</param>
        /// <returns>Category product search model</returns>
        protected virtual OfferProductSearchModel PrepareOfferProductSearchModel(OfferProductSearchModel searchModel, Offer offer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (offer == null)
                throw new ArgumentNullException(nameof(offer));

            searchModel.OfferId = offer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion
        public OfferModel PrepareOfferModel(OfferModel model, Offer offer, bool excludeProperties = false)
        {
            Action<OfferLocalizedModel, int> localizedModelConfiguration = null;

            if (offer != null)
            {
                //fill in model values from the entity
                model = model ?? offer.ToModel<OfferModel>();

                //prepare nested search model
                PrepareOfferProductSearchModel(model.OfferProductSearchModel, offer);

                
                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(offer, entity => entity.Name, languageId, false, false);
                };
            }

            //set default values for the new model
            if (offer == null)
            {
                model.Published = true;
                model.PauseOnHover = true;
                model.AutoPlay = true;
                model.AutoPlaySpeed = 2000;
                model.ButtonClasses = "u-arrow-v1 g-width-50 g-height-50 g-brd-1 g-brd-style-solid g-brd-gray-light-v1 g-brd-primary--hover g-color-gray-dark-v5 g-bg-primary--hover g-color-white--hover g-absolute-centered--y rounded-circle g-mt-minus-25";
                model.HideOnMobile = false;
                model.HideSeconds = false;
                model.PrimaryColor = "#3bb18f";
                model.BackgroundColor = "#c4d1ff33";
                model.PriceButtonBorderColor = "#fff";
                model.BackgroundNumberBox = "#a5c6bc80";
                model.CounterWordsOldPriceColor = "#777";
                model.CounterNumbersColor = "#e64b3b";
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);


            return model;
        }
        #region Method
        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category search model</returns>
        public virtual OfferSearchModel PrepareOfferSearchModel(OfferSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

          //  searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual OfferListModel PrepareOfferListModel(OfferSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get offers
            var offers =_offerService.GetAllOffers(offerName: searchModel.SearchOfferName, storeId: searchModel.SearchStoreId, showHidden: true,
              pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new OfferListModel().PrepareToGrid(searchModel, offers, () =>
            {
                return offers.Select(offer =>
                {
                    //fill in model values from the entity
                    var offerModel = offer.ToModel<OfferModel>();


                    return offerModel;
                });
            });

            return model;
        }


        public virtual OfferProductListModel PrepareOfferProductListModel(OfferProductSearchModel searchModel, Offer offer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (offer == null)
                throw new ArgumentNullException(nameof(offer));

            //get product categories
            var productOffers = _offerService.GetProductOffersByOfferId(offer.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new OfferProductListModel().PrepareToGrid(searchModel, productOffers, () =>
            {
                return productOffers.Select(productCategory =>
                {
                    //fill in model values from the entity
                    var offerProductModel = productCategory.ToModel<OfferProductModel>();

                    //fill in additional values (not existing in the entity)
                    offerProductModel.ProductName = _productService.GetProductById(productCategory.ProductId)?.Name;

                    return offerProductModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged product list model to add to the category
        /// </summary>
        /// <param name="searchModel">Product search model to add to the category</param>
        /// <returns>Product list model to add to the category</returns>
        public virtual AddProductToOfferListModel PrepareAddProductToOfferListModel(AddProductToOfferSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var products = _productService.SearchProducts(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerId: searchModel.SearchManufacturerId,
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddProductToOfferListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                  //  productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        public virtual OfferWidgetZoneListModel PrepareOfferWidgetZoneListModel(OfferWidgetZoneSearchModel searchModel, Offer offer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (offer == null)
                throw new ArgumentNullException(nameof(offer));

            //get product categories
            var widgetZoneOffers = _offerService.GetOfferWidgetZonesByOfferId(offer.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new OfferWidgetZoneListModel().PrepareToGrid(searchModel, widgetZoneOffers, () =>
            {
                return widgetZoneOffers.Select(widgetzone =>
                {
                    //fill in model values from the entity
                    var offerWidgetZoneModel = widgetzone.ToModel<OfferWidgetZoneModel>();

                    //fill in additional values (not existing in the entity)
                   // offerProductModel.WidgetZone = _productService.GetProductById(productCategory.ProductId)?.Name;

                    return offerWidgetZoneModel;
                });
            });

            return model;
        }

        #endregion

        #region UrlRecord
        public virtual OfferUrlRecordListModel PrepareOfferUrlRecordListModel(OfferUrlRecordSearchModel searchModel, Offer offer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (offer == null)
                throw new ArgumentNullException(nameof(offer));

            var urlRecordOffers = _offerService.GetOfferUrlRecordsByOfferId(offer.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            // prepare grid model
            var model = new OfferUrlRecordListModel().PrepareToGrid(searchModel, urlRecordOffers, () =>
            {
                return urlRecordOffers.Select(item =>
                {
                    //fill in model values from the entity
                    var offerUrlRecordModel = item;

                    //fill in additional values (not existing in the entity)
                    // offerProductModel.WidgetZone = _productService.GetProductById(productCategory.ProductId)?.Name;

                    return offerUrlRecordModel;
                });
            });

            return model;
        }

        #endregion
    }
}
