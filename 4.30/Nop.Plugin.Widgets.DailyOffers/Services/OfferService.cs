using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Plugin.Widgets.DailyOffers.Domain;
using Nop.Plugin.Widgets.DailyOffers.Infrastructure.Cache;
using Nop.Plugin.Widgets.DailyOffers.Models;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.DailyOffers.Services
{
    public partial class OfferService : IOfferService
    {
        #region Constant
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : offer ID
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        private const string PRODUCTOFFERS_ALLBYOFFERID_KEY = "Nop.productoffer.allbyofferid-{0}-{1}-{2}-{3}-{4}-{5}";
        private const string PRODUCTDETAILSOFFERS_ALLBYOFFERID_KEY = "Nop.productdetailsoffer.allbyofferid-{0}-{1}-{2}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string OFFERS_BY_ID_KEY = "Nop.offer.id-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string OFFERS_PATTERN_KEY = "Nop.offer.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTOFFERS_PATTERN_KEY = "Nop.productoffer.";

        private const string WIDGETZONEOFFERS_PATTERN_KEY = "Nop.widgetzoneoffer.";

        private const string OFFERSWIDGETZONES_ALLBYOFFERID_KEY = "Nop.offerwidetzone.allbyofferid-{0}-{1}-{2}-{3}-{4}-{5}";

        private const string OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY = "Nop.offerproductcategory.mainbyproductid-{0}-{1}-{2}-{3}";

        #endregion
        #region Fields
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<Offer> _offerRepository;
        private readonly IRepository<ProductOffer> _productOfferRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;

        private readonly IRepository<OfferWidgetZone> _offerWidgetZoneRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly CommonSettings _commonSettings;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly ILanguageService _languageService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly string _entityName;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerService _customerService;
        private readonly IPriceCalculationService _priceCalculationService;

        private readonly IRepository<OfferUrlRecord> _offerUrlRecordRepository;
        private readonly IRepository<UrlRecord> _UrlRecordRepository;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="offerRepository">Offer repository</param>
        /// <param name="productOfferRepository">ProductOffer repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public OfferService(
            IRepository<Offer> offerRepository,
            IRepository<ProductOffer> productOfferRepository,
            IRepository<Product> productRepository,
            IRepository<OfferWidgetZone> offerWidgetZoneRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IWorkContext workContext,
            IStoreContext storeContext,
            IEventPublisher eventPublisher,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            CommonSettings commonSettings,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            ILanguageService languageService,
            IProductModelFactory productModelFactory,
            ICategoryService categoryService,
            IRepository<Category> categoryRepository,
            IRepository<ProductCategory> productCategoryRepository,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            ICustomerService customerService, CatalogSettings catalogSettings,
            IPriceCalculationService priceCalculationService,
            IRepository<UrlRecord> urlRecordRepository,
            IRepository<OfferUrlRecord> offerUrlRecordRepository,
            IUrlRecordService urlRecordService,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager)
        {
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _webHelper = webHelper;
            _UrlRecordRepository = urlRecordRepository;
            _offerUrlRecordRepository = offerUrlRecordRepository;
            _urlRecordService = urlRecordService;
            _localizationService = localizationService;
            this._offerRepository = offerRepository;
            this._productOfferRepository = productOfferRepository;
            this._productRepository = productRepository;
            this._offerWidgetZoneRepository = offerWidgetZoneRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
            this._commonSettings = commonSettings;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            this._specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            this._languageService = languageService;
            this._productModelFactory = productModelFactory;
            this._categoryService = categoryService;
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            this._entityName = typeof(Offer).Name;
            _customerService = customerService;
            _catalogSettings = catalogSettings;
            _priceCalculationService = priceCalculationService;
        }

        #endregion

        #region UrlRecord
        public void UpdateOfferUrlRecord(OfferUrlRecord offerUrlRecord)
        {
            if (offerUrlRecord == null)
                throw new ArgumentNullException(nameof(offerUrlRecord));

            _offerUrlRecordRepository.Update(offerUrlRecord);

            //event notification
            _eventPublisher.EntityUpdated(offerUrlRecord);
        }

        public void InsertOfferUrlRecord(OfferUrlRecord offerUrlRecord)
        {
            if (offerUrlRecord == null)
                throw new ArgumentNullException(nameof(offerUrlRecord));

            _offerUrlRecordRepository.Insert(offerUrlRecord);

            //event notification
            _eventPublisher.EntityInserted(offerUrlRecord);
        }

        public void DeleteOfferUrlRecord(OfferUrlRecord offerUrlRecord)
        {
            if (offerUrlRecord == null)
                throw new ArgumentNullException(nameof(offerUrlRecord));
            _offerUrlRecordRepository.Delete(offerUrlRecord);
            //event notification
            _eventPublisher.EntityDeleted(offerUrlRecord);
        }


        public OfferUrlRecord GetOfferUrlRecordById(int OfferUrlRecordId)
        {
            if (OfferUrlRecordId == 0)
                return null;

            return _offerUrlRecordRepository.GetById(OfferUrlRecordId);
        }

        public List<UrlRecord> GetOfferUrlRecords()
        {
            return _offerUrlRecordRepository.Table.ToList().Select(x => x.UrlRecord).ToList();
        }

        public IPagedList<OfferModel.OfferUrlRecordModel> GetOfferUrlRecordsByOfferId(int offerId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {

            if (offerId == 0)
                return new PagedList<OfferModel.OfferUrlRecordModel>(new List<OfferModel.OfferUrlRecordModel>(), pageIndex, pageSize);
            //var key = _cacheKeyService.PrepareKeyForDefaultCache(NopOfferDefaults.GetOfferUrlsByOfferID, offerId,
            //               _storeContext.CurrentStore.Id,
            //               _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer));
            //return _staticCacheManager.Get(key, () =>
            //{
                var query = from po in _offerUrlRecordRepository.Table
                            where po.OfferId == offerId
                            orderby po.Id
                            select po;
                var OfferUrlRecords1 = query.ToList();
                var OfferUrlRecords = new List<OfferModel.OfferUrlRecordModel>();
                foreach (var item in OfferUrlRecords1)
                {
                    var urlRecord = _UrlRecordRepository.GetById(item.UrlRecordId);
                    if (urlRecord != null)
                    {
                        OfferUrlRecords.Add(new OfferModel.OfferUrlRecordModel
                        {
                            UrlRecordId = item.UrlRecordId,
                            Id = item.Id,
                            OfferId = item.OfferId,
                            UrlRecordName = urlRecord.Slug,
                            EntityName = urlRecord.EntityName,
                            IsActive = urlRecord.IsActive,
                            Language = urlRecord.LanguageId == 0
                                ? _localizationService.GetResource("Admin.System.SeNames.Language.Standard")
                                : _languageService.GetLanguageById(urlRecord.LanguageId)?.Name ?? "Unknown"

                        });
                    }
                }
                var OfferUrlRecordPage = new PagedList<OfferModel.OfferUrlRecordModel>(OfferUrlRecords, pageIndex, pageSize);
                return OfferUrlRecordPage;
            //});
        }

        #endregion
        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual void InsertOffer(Offer offer)
        {
            if (offer == null)
                throw new ArgumentNullException(nameof(offer));

            _offerRepository.Insert(offer);

            //event notification
            _eventPublisher.EntityInserted(offer);
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual void UpdateOffer(Offer offer)
        {
            if (offer == null)
                throw new ArgumentNullException(nameof(offer));

            _offerRepository.Update(offer);
            _eventPublisher.EntityUpdated(offer);
        }
        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="offer">Offer</param>
        public virtual void DeleteOffer(Offer offer)
        {
            if (offer == null)
                throw new ArgumentNullException(nameof(offer));

            offer.Deleted = true;
            UpdateOffer(offer);

            //event notification
            _eventPublisher.EntityDeleted(offer);
        }


        public OfferWidgetZone GetOfferWidgetZoneById(int OfferWidgetZoneId)
        {
            if (OfferWidgetZoneId == 0)
                return null;

            return _offerWidgetZoneRepository.GetById(OfferWidgetZoneId);

        }

        public List<string> GetOfferWidgetZones()
        {
            return _offerWidgetZoneRepository.Table.ToList().Select(x => x.WidgetZone).ToList();
        }
        /// <summary>
        /// Gets product offer mapping collection
        /// </summary>
        /// <param name="offerId">Offer identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a offer mapping collection</returns>
        public virtual IPagedList<ProductOffer> GetProductOffersByOfferId(int offerId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (offerId == 0)
                return new PagedList<ProductOffer>(new List<ProductOffer>(), pageIndex, pageSize);
            var query = from po in _productOfferRepository.Table
                        where po.OfferId == offerId
                        orderby po.DisplayOrder, po.Id
                        select po;


            var productOffers = new PagedList<ProductOffer>(query, pageIndex, pageSize);
            return productOffers;
            //});
        }

        //public virtual List<ProductDetailsModel> GetProductDetailsByOfferId(int offerId)
        //{
        //    if (offerId == 0)
        //        return new List<ProductDetailsModel>();

        //    // var key = string.Format(PRODUCTDETAILSOFFERS_ALLBYOFFERID_KEY, offerId, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
        //    //return _cacheManager.Get(key, () =>
        //    //{
        //    var query = from po in _productOfferRepository.Table
        //                where po.OfferId == offerId
        //                orderby po.DisplayOrder, po.Id
        //                select po;


        //    var productOffers = new List<ProductOffer>(query);
        //    var prodModels = productOffers.Select(x =>
        //    {
        //        Product product = _productRepository.GetById(x.Id);
        //        var productPresentationModel = new ProductPresenationModel();
        //        var model = _productModelFactory.PrepareProductDetailsModel(product, null, false);

        //        return model;
        //    });
        //    return prodModels.ToList();
        //    //});
        //}

        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product</returns>
        public virtual Offer GetOfferById(int offerId)
        {
            if (offerId == 0)
                return null;
            return _offerRepository.GetById(offerId);
        }
        /// <summary>
        /// Gets a product offer mapping 
        /// </summary>
        /// <param name="productOfferId">Product offer mapping identifier</param>
        /// <returns>Product offer mapping</returns>
        public virtual ProductOffer GetProductOfferById(int productOfferId)
        {
            if (productOfferId == 0)
                return null;

            return _productOfferRepository.GetById(productOfferId);

        }
        /// <summary>
        /// Updates the product offer mapping 
        /// </summary>
        /// <param name="productOffer">>Product offer mapping</param>
        public virtual void UpdateProductOffer(ProductOffer productOffer)
        {
            if (productOffer == null)
                throw new ArgumentNullException(nameof(productOffer));

            _productOfferRepository.Update(productOffer);

            //event notification
            _eventPublisher.EntityUpdated(productOffer);
        }

        /// <summary>
        /// Deletes a product category mapping
        /// </summary>
        /// <param name="productOffer">Product category</param>
        public virtual void DeleteProductOffer(ProductOffer productOffer)
        {
            if (productOffer == null)
                throw new ArgumentNullException(nameof(productOffer));

            _productOfferRepository.Delete(productOffer);
            //event notification
            _eventPublisher.EntityDeleted(productOffer);
        }
        public virtual IPagedList<Offer> GetAllOffers(string offerName = "",
            int storeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false)
        {
            //var key = _cacheKeyService.PrepareKeyForDefaultCache(NopOfferDefaults.OffersAllCacheKey,
            //    storeId,
            //    _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
            //    showHidden);
            //var offers = _staticCacheManager.Get(key, () =>
            //{

                var query = _offerRepository.Table;
                if (!showHidden)
                    query = query.Where(m => m.Published);
                if (!string.IsNullOrWhiteSpace(offerName))
                    query = query.Where(m => m.Name.Contains(offerName));
                query = query.Where(m => !m.Deleted);
                query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);

                if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
                {
                    if (!showHidden && !_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                        query = from c in query
                                join acl in _aclRepository.Table
                                    on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }

                    if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                    on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || storeId == sm.StoreId
                                select c;
                    }

                    query = query.Distinct().OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                }
                return new PagedList<Offer>(query, pageIndex, pageSize);
            //});
          
        }


        public IList<Offer> GetOffersByWidgetZoneName(string WidgetName = "")
        {
            var currentPageUrl = _webHelper.GetThisPageUrl(false);

            var uri = new Uri(currentPageUrl);
            var urlRecord = _urlRecordService.GetBySlug(uri.LocalPath.TrimStart('/'));

            //var key = _cacheKeyService.PrepareKeyForDefaultCache(NopOfferDefaults.GetOffersByWidgetZoneNameAndUrlRecordId, WidgetName,
            //    (urlRecord==null?0:urlRecord.Id),
            //                _storeContext.CurrentStore.Id,
            //                _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer));
            //return _staticCacheManager.Get(key, () =>
            //{
                var query = from of in _offerRepository.Table
                            join wz in _offerWidgetZoneRepository.Table on of.Id equals wz.OfferId
                            where wz.WidgetZone.Equals(WidgetName) && of.Published == true
                            select of;
                //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                //That's why we pass the date value
                var nowDateUtc = DateTime.UtcNow.Date;
                var nowTimeUtc = DateTime.UtcNow.TimeOfDay;

                //available dates
                query = query.Where(p =>
                    (!p.FromDate.HasValue || p.FromDate.Value <= nowDateUtc) &&
                    (!p.ToDate.HasValue || p.ToDate.Value >= nowDateUtc));
                query = query.Where(p =>
                    (!p.FromTimeUtc.HasValue || p.FromTimeUtc.Value < nowTimeUtc) &&
                    (!p.ToTimeUtc.HasValue || p.ToTimeUtc.Value > nowTimeUtc));

                if ((_storeContext.CurrentStore.Id > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!_catalogSettings.IgnoreAcl))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                        query = from c in query
                                join acl in _aclRepository.Table
                                    on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }

                    if (_storeContext.CurrentStore.Id > 0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                    on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || _storeContext.CurrentStore.Id == sm.StoreId
                                select c;
                    }

                    query = query.Distinct().OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                }



                var offerList = query.ToList();

                var deletedList = new List<Offer>();
                foreach (var item in offerList)
                {
                    var offerUrlList = GetOfferUrlRecordsByOfferId(item.Id);
                    if (offerUrlList != null && offerUrlList.Count > 0)
                    {
                        if (urlRecord == null)
                        {
                            deletedList.Add(item);
                            continue;
                        }
                        else if (!offerUrlList.Select(x => x.UrlRecordId).Contains(urlRecord.Id))
                        {
                            deletedList.Add(item);
                            continue;
                        }
                    }
                    switch (item.SchedulePatternId)
                    {
                        case (int)SchedulePattern.EveryDay:
                            break;
                        case (int)SchedulePattern.EveryMonth:
                            if (DateTime.Now.Day != 1)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.FromMondayToFriday:
                            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.FromSundayToThursday:
                            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnEvenDay:
                            if (DateTime.Now.Day % 2 != 0)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnFriday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Friday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnFridayAndSaturday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Friday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnMonday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Monday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnOddDay:
                            if (DateTime.Now.Day % 2 == 0)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnSaturday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnSaturdayAndSunday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnSunday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnThursday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Thursday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnThursdayAndFriday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Thursday && DateTime.Now.DayOfWeek != DayOfWeek.Friday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnTuesday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Tuesday)
                                deletedList.Add(item);
                            break;
                        case (int)SchedulePattern.OnWednesday:
                            if (DateTime.Now.DayOfWeek != DayOfWeek.Wednesday)
                                deletedList.Add(item);
                            break;

                    }

                }
                foreach (var s in deletedList)
                    offerList.Remove(s);


                return offerList;
            //});

        }

        public void InsertOfferWidgetZone(OfferWidgetZone OfferWidgetZone)
        {
            if (OfferWidgetZone == null)
                throw new ArgumentNullException(nameof(OfferWidgetZone));

            _offerWidgetZoneRepository.Insert(OfferWidgetZone);

            //event notification
            _eventPublisher.EntityInserted(OfferWidgetZone);

        }


        public virtual void UpdateOfferWidgetZone(OfferWidgetZone OfferWidgetZone)
        {
            if (OfferWidgetZone == null)
                throw new ArgumentNullException(nameof(OfferWidgetZone));

            _offerWidgetZoneRepository.Update(OfferWidgetZone);

            //event notification
            _eventPublisher.EntityUpdated(OfferWidgetZone);
        }

        /// <summary>
        /// Deletes a product category mapping
        /// </summary>
        /// <param name="productOffer">Product category</param>
        public virtual void DeleteOfferWidgetZone(OfferWidgetZone OfferWidgetZone)
        {
            if (OfferWidgetZone == null)
                throw new ArgumentNullException(nameof(OfferWidgetZone));

            _offerWidgetZoneRepository.Delete(OfferWidgetZone);

            //event notification
            _eventPublisher.EntityDeleted(OfferWidgetZone);
        }


        public virtual void InsertProductOffer(ProductOffer productOffer)
        {
            if (productOffer == null)
                throw new ArgumentNullException(nameof(productOffer));

            _productOfferRepository.Insert(productOffer);

            //event notification
            _eventPublisher.EntityInserted(productOffer);
        }

        public virtual IPagedList<OfferWidgetZone> GetOfferWidgetZonesByOfferId(int offerId,
    int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (offerId == 0)
                return new PagedList<OfferWidgetZone>(new List<OfferWidgetZone>(), pageIndex, pageSize);
            //var key = _cacheKeyService.PrepareKeyForDefaultCache(NopOfferDefaults.GetOfferWidgetZonesByOfferIDKey, offerId,
            //                           _storeContext.CurrentStore.Id,
            //                           _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer));
            //return _staticCacheManager.Get(key, () =>
            //{
                var query = from po in _offerWidgetZoneRepository.Table
                            where po.OfferId == offerId
                            orderby po.DisplayOrder, po.Id
                            select po;


                var productOffers = new PagedList<OfferWidgetZone>(query, pageIndex, pageSize);
                return productOffers;
            //});
        }




        public IList<ProductPresenationModel> PrepareProductPresentationModels(int offerId)
        {
            if (offerId == 0)
                return new List<ProductPresenationModel>();

            var key = _cacheKeyService.PrepareKeyForDefaultCache(NopOfferDefaults.GetOfferProductsByOfferID, offerId,
                             _storeContext.CurrentStore.Id,
                             _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer));
            return _staticCacheManager.Get(key, () =>
            {
                var query = from po in _productOfferRepository.Table
                            where po.OfferId == offerId
                            orderby po.DisplayOrder, po.Id
                            select po;


                var productOffers = new List<ProductOffer>(query);
                var prodModels = productOffers.Select(x =>
                {
                    Product product = _productRepository.GetById(x.ProductId);

                    var pOverviewModel = _productModelFactory.PrepareProductOverviewModels(new[] { product }.AsEnumerable<Product>()).FirstOrDefault();
                    var productPresentationModel = new ProductPresenationModel()
                    {
                        Id = pOverviewModel.Id,
                        Name = pOverviewModel.Name,
                        ProductPrice = pOverviewModel.ProductPrice,
                        SeName = pOverviewModel.SeName,
                        ShortDescription = pOverviewModel.ShortDescription,
                        DefaultPictureModel = pOverviewModel.DefaultPictureModel,
                        PriceBeforDiscountValue = _currencyService.ConvertFromPrimaryStoreCurrency(product.Price, _workContext.WorkingCurrency),
                        OldPriceValue = _currencyService.ConvertFromPrimaryStoreCurrency(product.OldPrice, _workContext.WorkingCurrency),
                        ProductType = product.ProductType
                    };
                    productPresentationModel.PriceBeforDiscount = _priceFormatter.FormatPrice(productPresentationModel.PriceBeforDiscountValue);
                    productPresentationModel.OldPrice = _priceFormatter.FormatPrice(productPresentationModel.OldPriceValue);

                    productPresentationModel.Category = GetProductMainCategoryByProductId(product.Id, _storeContext.CurrentStore.Id, false);
                    //discount
                    var tmpDiscountAmount = _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, 0, true, 1, out var tmpAppliedDiscounts, out var applieddescount);

                    productPresentationModel.EndDateDiscount = ((productPresentationModel.PriceBeforDiscountValue > productPresentationModel.ProductPrice.PriceValue) && product.HasDiscountsApplied && applieddescount.Count() > 0 && applieddescount.Min(y => y.EndDateUtc) > DateTime.UtcNow ? applieddescount.Min(y => y.EndDateUtc) : DateTime.MaxValue);
                    if (product.HasDiscountsApplied == false && product.ProductType == ProductType.GroupedProduct)
                    {
                        productPresentationModel.PriceBeforDiscountValue = 0;
                        productPresentationModel.ProductPrice.PriceValue = 0;
                    }
                    return productPresentationModel;
                });
                return prodModels.ToList();
            });
        }

        public virtual Category GetProductMainCategoryByProductId(int productId, int storeId, bool showHidden = false)
        {
            if (productId == 0)
                return new Category();

            // var key = string.Format(OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY, showHidden, productId, _workContext.CurrentCustomer.Id, storeId);
            //return _cacheManager.Get(key, () =>
            //{
            var query = from pc in _productCategoryRepository.Table
                        join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                        where pc.ProductId == productId &&
                              !c.Deleted &&
                              (showHidden || c.Published)
                        orderby pc.DisplayOrder, pc.Id
                        select pc;

            var mainCategoryID = query.ToList().FirstOrDefault().CategoryId;
            if (mainCategoryID != 0)
            {
                Category mainCategory = _categoryService.GetCategoryById(mainCategoryID);
                //ملاحظة مهمة هنا يظهر خطأ ائا كان الكاتيجوري فارغ
                if (!showHidden && mainCategory != null)
                {
                    //ACL (access control list) and store mapping
                    if (_aclService.Authorize(mainCategory) && _storeMappingService.Authorize(mainCategory, storeId))
                        return mainCategory;
                }
                else
                {
                    //no filtering
                    return null;
                }
            }
            return null;
            //});
        }
    }
}



                   
