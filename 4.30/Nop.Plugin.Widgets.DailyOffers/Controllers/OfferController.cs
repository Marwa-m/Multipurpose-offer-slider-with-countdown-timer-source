using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.DailyOffers.Domain;
using Nop.Plugin.Widgets.DailyOffers.Extensions;
using Nop.Plugin.Widgets.DailyOffers.Models;
using Nop.Plugin.Widgets.DailyOffers.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Widgets.DailyOffers.Helpers;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Plugin.Widgets.DailyOffers.Infrastructure;
using Nop.Web.Framework.Factories;
using Nop.Plugin.Widgets.DailyOffers.Factories;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Factories;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Common;

namespace Nop.Plugin.Widgets.DailyOffers.Controllers
{
    [Area(AreaNames.Admin)]
    public class OfferController : BasePluginController
    {
        #region Fields

        private readonly IOfferService _offerService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILanguageService _languageService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IOfferModelFactory _offerModelFactory;
        private readonly ISettingService _settings;
        private readonly INotificationService _notificationService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;

        private readonly IUrlRecordService _urlRecordService;
        private readonly ICommonModelFactory _commonModelFactory;

        #endregion


        #region Ctor

        public OfferController(IOfferService offerService,
            IManufacturerService manufacturerService, IProductService productService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IVendorService vendorService,
            IWorkContext workContext,
            IStaticCacheManager cacheManager,
            ICustomerService customerService ,
            ICustomerActivityService customerActivityService,
            IOfferModelFactory offerModelFactory,
            ISettingService settingService,
            INotificationService notificationService,
            IBaseAdminModelFactory baseAdminModelFactory,
            IUrlRecordService urlRecordService,
            ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
            _urlRecordService = urlRecordService; 
            this._offerService = offerService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._vendorService = vendorService;
            this._aclService = aclService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._languageService = languageService;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._offerModelFactory = offerModelFactory;
            this._settings = settingService;
            _notificationService = notificationService;
            _baseAdminModelFactory = baseAdminModelFactory;
        }

        #endregion
        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(Offer offer, OfferModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(offer,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

               

            }
        }

        [NonAction]
        protected virtual void PrepareOfferWidgetZoneModel(OfferWidgetZoneModel model, OfferWidgetZone offer, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");
           
        }
        //[NonAction]
        //protected virtual void PrepareOfferModel(OfferModel model, Offer offer, bool setPredefinedValues, bool excludeProperties)
        //{
        //    if (model == null)
        //        throw new ArgumentNullException("model");
        //    Action<OfferLocalizedModel, int> localizedModelConfiguration = null;
        //    //default values
        //    if (setPredefinedValues)
        //    {
        //        model.Published = true;
        //        model.PauseOnHover = true;
        //        model.AutoPlay = true;
        //        model.AutoPlaySpeed = 2000;
        //        model.ButtonClasses = "u-arrow-v1 g-width-50 g-height-50 g-brd-1 g-brd-style-solid g-brd-gray-light-v1 g-brd-primary--hover g-color-gray-dark-v5 g-bg-primary--hover g-color-white--hover g-absolute-centered--y rounded-circle g-mt-minus-25";
        //    }
        //    //prepare localized models
        //    if (!excludeProperties)
        //        model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);
        //}
        [NonAction]
        protected virtual void PrepareAclModel(OfferModel model, Offer offer, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && offer != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(offer).ToList();

            var allRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }
        [NonAction]
        protected virtual void SaveOfferAcl(Offer offer, OfferModel model)
        {
            offer.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(offer);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(offer, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(OfferModel model, Offer offer, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && offer != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(offer).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
            model.AvailableSchedulePatterns = SchedulePattern.EveryDay.ToSelectList(false).ToList();


        }

        [NonAction]
        protected virtual void SaveStoreMappings(Offer offer, OfferModel model)
        {
            offer.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(offer);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(offer, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }
        #endregion
        #region List
        public IActionResult Index()
        {
            return RedirectToAction("~/Plugins/Widgets.DailyOffers/Views/List.cshtml");
        }
        public virtual IActionResult List()
        {
            //Trial
            DateTime dtCreate = _settings.GetSettingByKey<DateTime>("DailyOffersdtv43");
            if (dtCreate.AddDays(7) < DateTime.Now)
                _notificationService.WarningNotification(_localizationService.GetResource("Plugins.Widgets.DailyOffers.trialEnd"));

            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            //prepare model
            var model = _offerModelFactory.PrepareOfferSearchModel(new OfferSearchModel());

            return View("~/Plugins/Widgets.DailyOffers/Views/List.cshtml", model);

        }
        [HttpPost]
        public virtual IActionResult List(OfferSearchModel searchModel)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedDataTablesJson();
            //prepare model
            var model = _offerModelFactory.PrepareOfferListModel(searchModel);

            return Json(model);

        }

        #endregion
        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

           // var model = new OfferModel();
            //prepare model
            var model = _offerModelFactory.PrepareOfferModel(new OfferModel(), null);
            //locales
            //  AddLocales(_languageService, model.Locales);

            //ACL
            PrepareAclModel(model, null, false);
            //Stores
            PrepareStoresMappingModel(model, null, false);
          //  PrepareOfferModel(model, null, true,false);


            return View("~/Plugins/Widgets.DailyOffers/Views/Create.cshtml", model);

        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(OfferModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var offer = model.ToEntity();
                offer.CreatedOnUtc = DateTime.UtcNow;
                offer.UpdatedOnUtc = DateTime.UtcNow;
                _offerService.InsertOffer(offer);

                //locales
                UpdateLocales(offer, model);
                //discounts
                _offerService.UpdateOffer(offer);
                //ACL (customer roles)
                SaveOfferAcl(offer, model);
                //Stores
                SaveStoreMappings(offer, model);
                _offerService.UpdateOffer(offer);
                //activity log
                _customerActivityService.InsertActivity("AddNewOffer", _localizationService.GetResource("Plugins.Widgets.DailyOffers.ActivityLog.AddNewOffer"), offer);

                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Widgets.DailyOffers.Offer.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = offer.Id });
            }
            //prepare model
            model = _offerModelFactory.PrepareOfferModel(model, null, true);
            ////If we got this far, something failed, redisplay form
            ////ACL
            PrepareAclModel(model, null, true);
            ////Stores
            PrepareStoresMappingModel(model, null, true);
            return View("~/Plugins/Widgets.DailyOffers/Views/Create.cshtml", model);

        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            var offer = _offerService.GetOfferById(id);
            if (offer == null || offer.Deleted)
                //No offer found with the specified id
                return RedirectToAction("List");

          //  var model = offer.ToModel();
            //prepare model
            var model = _offerModelFactory.PrepareOfferModel(null, offer);
            //locales
            //AddLocales(_languageService, model.Locales, (locale, languageId) =>
            //{
            //    locale.Name = _localizationService.GetLocalized(offer,x => x.Name, languageId, false, false);

            //});

            //ACL
            PrepareAclModel(model, offer, false);
            //Stores
            PrepareStoresMappingModel(model, offer, false);
            return View("~/Plugins/Widgets.DailyOffers/Views/Edit.cshtml", model);

        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(OfferModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            var offer = _offerService.GetOfferById(model.Id);
            if (offer == null || offer.Deleted)
                //No offer found with the specified id
                return RedirectToAction("List");
            try
            {
                //ensure color is valid (can be instantiated)
                System.Drawing.ColorTranslator.FromHtml(model.PrimaryColor);
                System.Drawing.ColorTranslator.FromHtml(model.BackgroundColor);
                System.Drawing.ColorTranslator.FromHtml(model.PriceButtonBorderColor);
                System.Drawing.ColorTranslator.FromHtml(model.BackgroundNumberBox);
                System.Drawing.ColorTranslator.FromHtml(model.CounterWordsOldPriceColor);
                System.Drawing.ColorTranslator.FromHtml(model.CounterNumbersColor);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError(string.Empty, exc.Message);
            }
            if (ModelState.IsValid)
            {
                
                offer = model.ToEntity(offer);
                offer.UpdatedOnUtc = DateTime.UtcNow;
                _offerService.UpdateOffer(offer);
                //locales
                UpdateLocales(offer, model);
                //ACL
                SaveOfferAcl(offer, model);
                //Stores
                SaveStoreMappings(offer, model);
                _offerService.UpdateOffer(offer);
                //activity log
                _customerActivityService.InsertActivity("EditOffer", _localizationService.GetResource("Plugins.Widgets.DailyOffers.ActivityLog.EditOffer"), offer);

                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Widgets.DailyOffers.Offer.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = offer.Id });
                }
                return RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            //ACL
            PrepareAclModel(model, offer, true);
            //Stores
            PrepareStoresMappingModel(model, offer, true);
            return View("~/Plugins/Widgets.DailyOffers/Views/Edit.cshtml", model);

        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            var offer = _offerService.GetOfferById(id);
            if (offer == null)
                //No offer found with the specified id
                return RedirectToAction("List");

            _offerService.DeleteOffer(offer);

            //activity log
            _customerActivityService.InsertActivity("DeleteOffer", _localizationService.GetResource("Plugins.Widgets.DailyOffers.ActivityLog.DeleteOffer"), offer);

            _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Widgets.DailyOffers.Offer.Deleted"));
            return RedirectToAction("List");
        }
        #endregion
        #region Products
        [HttpPost]
        public virtual IActionResult ProductList(OfferProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedDataTablesJson();

            //try to get a category with the specified id
            var offer = _offerService.GetOfferById(searchModel.OfferId)
                ?? throw new ArgumentException("No offer found with the specified id");

            //prepare model
            var model = _offerModelFactory.PrepareOfferProductListModel(searchModel, offer);

            return Json(model);
        }


        public virtual IActionResult ProductUpdate(OfferModel.OfferProductModel model)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            var productOffer = _offerService.GetProductOfferById(model.Id);
            if (productOffer == null)
                throw new ArgumentException("No product offer mapping found with the specified id");

            productOffer.DisplayOrder = model.DisplayOrder;
            _offerService.UpdateProductOffer(productOffer);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductDelete(int id)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            var productOffer = _offerService.GetProductOfferById(id);
            if (productOffer == null)
                throw new ArgumentException("No product offer mapping found with the specified id");

            //var offerId = productOffer.OfferId;
            _offerService.DeleteProductOffer(productOffer);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductAddPopup(int offerId)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            var model = new AddProductToOfferSearchModel();
            //offers
            model.AvailableOffers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var offers = SelectedListHelper.GetOfferList(_offerService, true);
            foreach (var c in offers)
                model.AvailableOffers.Add(c);

            //manufacturers
            //prepare available manufacturers
            _baseAdminModelFactory.PrepareManufacturers(model.AvailableManufacturers);
            _baseAdminModelFactory.PrepareStores(model.AvailableStores);
            _baseAdminModelFactory.PrepareVendors(model.AvailableVendors);
            _baseAdminModelFactory.PrepareStores(model.AvailableProductTypes);
            return View("~/Plugins/Widgets.DailyOffers/Views/ProductAddPopup.cshtml", model);

        }

        [HttpPost]
        public virtual IActionResult ProductAddPopupList(AddProductToOfferSearchModel searchModel)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _offerModelFactory.PrepareAddProductToOfferListModel(searchModel);

            return Json(model);
        }

        [HttpGet,HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ProductAddPopup(OfferModel.AddOfferProductModel model)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            if (model.SelectedProductIds != null)
            {
                foreach (var id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        var existingProductOffers = _offerService.GetProductOffersByOfferId(model.OfferId, showHidden: true);
                        if (existingProductOffers.FindProductOffer(id, model.OfferId) == null)
                        {
                            _offerService.InsertProductOffer(
                                new ProductOffer
                                {
                                    OfferId = model.OfferId,
                                    ProductId = id,
                                    DisplayOrder = 1
                                    //OfferWidgetZoneId=model.of

                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            return View("~/Plugins/Widgets.DailyOffers/Views/ProductAddPopup.cshtml",new  AddProductToOfferSearchModel());

        }

        #endregion
        #region WidgetZones

        [HttpPost]
        public virtual IActionResult WidgetZoneList(OfferWidgetZoneSearchModel searchModel)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedDataTablesJson();

            //try to get a category with the specified id
            var offer = _offerService.GetOfferById(searchModel.OfferId)
                ?? throw new ArgumentException("No offer found with the specified id");

            //prepare model
            var model = _offerModelFactory.PrepareOfferWidgetZoneListModel(searchModel, offer);

            return Json(model);

        }

        public virtual IActionResult WidgetZoneUpdate(OfferModel.OfferWidgetZoneModel model)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            var widgetzoneOffer = _offerService.GetOfferWidgetZoneById(model.Id);
            if (widgetzoneOffer == null)
                throw new ArgumentException("No widgetzone offer mapping found with the specified id");

            widgetzoneOffer.DisplayOrder = model.DisplayOrder;
            _offerService.UpdateOfferWidgetZone(widgetzoneOffer);

            return new NullJsonResult();
        }

        public virtual IActionResult WidgetZoneDelete(int id)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            var widgetzoneOffer = _offerService.GetOfferWidgetZoneById(id);
            if (widgetzoneOffer == null)
                throw new ArgumentException("No widget zone offer mapping found with the specified id");

            //var offerId = productOffer.OfferId;
            _offerService.DeleteOfferWidgetZone(widgetzoneOffer);

            return new NullJsonResult();
        }

        public virtual IActionResult WidgetZoneAddPopup(int offerId)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

            var model = new OfferModel.AddOfferWidgetZoneModel();

            model.AvailableWidgetZones = SelectedListHelper.GetPublicWidgetZones();
            return View("~/Plugins/Widgets.DailyOffers/Views/WidgetZoneAddPopup.cshtml", model);

        }

        [HttpGet, HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult WidgetZoneAddPopup(OfferModel.AddOfferWidgetZoneModel model)
        {
            if (!_permissionService.Authorize(DailyOffersPlugin.ManageDailyOffersPermission))
                return AccessDeniedView();

                    if (model.WidgetZone != null)
                    {
                        var existingWidgetZoneOffers = _offerService.GetOfferWidgetZonesByOfferId(model.OfferId, showHidden: true);
                        if (existingWidgetZoneOffers.FindWidgetZoneOffer(model.WidgetZone, model.OfferId) == null)
                        {
                            _offerService.InsertOfferWidgetZone(
                                new OfferWidgetZone
                                {
                                    OfferId = model.OfferId,
                                    WidgetZone = model.WidgetZone,
                                    DisplayOrder = model.DisplayOrder
                                    //OfferWidgetZoneId=model.w

                                });
                        }
                    }

            ViewBag.RefreshPage = true;
            return View("~/Plugins/Widgets.DailyOffers/Views/WidgetZoneAddPopup.cshtml",model);

        }

        #endregion

        #region UrlRecord
        [HttpPost]
        public virtual IActionResult UrlRecordList(OfferUrlRecordSearchModel searchModel)
        {
            if (!_permissionService.Authorize(DailyOffersPermissionProvider.ManageDailyOffersPermission))
                return AccessDeniedDataTablesJson();

            //try to get a category with the specified id
            var offer = _offerService.GetOfferById(searchModel.OfferId)
                ?? throw new ArgumentException("No offer found with the specified id");

            //prepare model
            var model = _offerModelFactory.PrepareOfferUrlRecordListModel(searchModel, offer);

            return Json(model);

        }

        [HttpPost]
        public virtual IActionResult UrlRecordAddPopupList(UrlRecordSearchModel searchModel)
        {
            if (!_permissionService.Authorize(DailyOffersPermissionProvider.ManageDailyOffersPermission))
                return AccessDeniedDataTablesJson();
            //prepare model
            var model = _commonModelFactory.PrepareUrlRecordSearchModel(new UrlRecordSearchModel());

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult SeNames(UrlRecordSearchModel searchModel)
        {
            if (!_permissionService.Authorize(DailyOffersPermissionProvider.ManageDailyOffersPermission))
                return AccessDeniedDataTablesJson();
            searchModel.LanguageId = -1;
            searchModel.IsActiveId = 0;
            //prepare model
            var model = _commonModelFactory.PrepareUrlRecordListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult UrlRecordUpdate(OfferModel.OfferUrlRecordModel model)
        {
            if (!_permissionService.Authorize(DailyOffersPermissionProvider.ManageDailyOffersPermission))
                return AccessDeniedView();

            var urlRecordOffer = _offerService.GetOfferUrlRecordById(model.Id);
            if (urlRecordOffer == null)
                throw new ArgumentException("No widgetzone offer mapping found with the specified id");

            _offerService.UpdateOfferUrlRecord(urlRecordOffer);

            return new NullJsonResult();
        }

        public virtual IActionResult UrlRecordDelete(int id)
        {
            if (!_permissionService.Authorize(DailyOffersPermissionProvider.ManageDailyOffersPermission))
                return AccessDeniedView();

            var urlRecordOffer = _offerService.GetOfferUrlRecordById(id);
            if (urlRecordOffer == null)
                throw new ArgumentException("No widget zone offer mapping found with the specified id");

            _offerService.DeleteOfferUrlRecord(urlRecordOffer);

            return new NullJsonResult();
        }

        public virtual IActionResult UrlRecordAddPopup(int offerId)
        {
            if (!_permissionService.Authorize(DailyOffersPermissionProvider.ManageDailyOffersPermission))
                return AccessDeniedView();

            //prepare model
            var urlRecordSearchModel = _commonModelFactory.PrepareUrlRecordSearchModel(new UrlRecordSearchModel());
            OfferModel.AddOfferUrlRecordModel model = new OfferModel.AddOfferUrlRecordModel
            {
                OfferId = offerId,
                urlRecordSearchModel = urlRecordSearchModel,

            };

            return View("~/Plugins/Widgets.DailyOffers/Views/UrlRecordAddPopup.cshtml", model);

        }

        [HttpGet, HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult UrlRecordAddPopup(OfferModel.AddOfferUrlRecordModel model)
        {
            if (!_permissionService.Authorize(DailyOffersPermissionProvider.ManageDailyOffersPermission))
                return AccessDeniedView();


            if (model.SelectedUrlRecordsIds != null)
            {
                foreach (var id in model.SelectedUrlRecordsIds)
                {
                    var urlRecord = _urlRecordService.GetUrlRecordById(id);
                    if (urlRecord != null)
                    {
                        var existingUrlRecords = _offerService.GetOfferUrlRecordsByOfferId(model.OfferId, showHidden: true);
                        if (existingUrlRecords.FindOfferUrlRecord(id, model.OfferId) == null)
                        {
                            _offerService.InsertOfferUrlRecord(
                                new OfferUrlRecord
                                {
                                    OfferId = model.OfferId,
                                    UrlRecordId = id,
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            return View("~/Plugins/Widgets.DailyOffers/Views/UrlRecordAddPopup.cshtml", model);

        }

        #endregion
        #region BasePlugin
        protected virtual void SaveSelectedTabName(string tabName = "", bool persistForTheNextRequest = true)
        {
            //default root tab
            SaveSelectedTabName(tabName, "selected-tab-name", null, persistForTheNextRequest);
            //child tabs (usually used for localization)
            //Form is available for POST only
            if (Request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase))
                foreach (var key in this.Request.Form.Keys)
                    if (key.StartsWith("selected-tab-name-", StringComparison.InvariantCultureIgnoreCase))
                        SaveSelectedTabName(null, key, key.Substring("selected-tab-name-".Length), persistForTheNextRequest);
        }

        /// <summary>
        /// Save selected tab name
        /// </summary>
        /// <param name="tabName">Tab name to save; empty to automatically detect it</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request. Pass null to ignore</param>
        /// <param name="formKey">Form key where selected tab name is stored</param>
        /// <param name="dataKeyPrefix">A prefix for child tab to process</param>
        protected virtual void SaveSelectedTabName(string tabName, string formKey, string dataKeyPrefix, bool persistForTheNextRequest)
        {
            //keep this method synchronized with
            //"GetSelectedTabName" method of \Nop.Web.Framework\Extensions\HtmlExtensions.cs
            if (string.IsNullOrEmpty(tabName))
            {
                tabName = Request.Form[formKey];
            }

            if (!string.IsNullOrEmpty(tabName))
            {
                var dataKey = "nop.selected-tab-name";
                if (!string.IsNullOrEmpty(dataKeyPrefix))
                    dataKey += $"-{dataKeyPrefix}";

                if (persistForTheNextRequest)
                {
                    TempData[dataKey] = tabName;
                }
                else
                {
                    ViewData[dataKey] = tabName;
                }
            }
        }


        #endregion
    }
}