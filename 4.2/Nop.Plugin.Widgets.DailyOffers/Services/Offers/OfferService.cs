using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Plugin.Widgets.DailyOffers.Domain.Offers;
using Nop.Plugin.Widgets.DailyOffers.Infrastructure.Cache;
using Nop.Plugin.Widgets.DailyOffers.Models.Offers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.DailyOffers.Services.Offers
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
        private const string PRODUCTDETAILSOFFERS_ALLBYOFFERID_KEY= "Nop.productdetailsoffer.allbyofferid-{0}-{1}-{2}";
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

        private readonly IRepository<Offer> _offerRepository;
        private readonly IRepository<ProductOffer> _productOfferRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;

        private readonly IRepository<OfferWidgetZone> _offerWidgetZoneRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
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
        public OfferService(ICacheManager cacheManager,
            IRepository<Offer> offerRepository,
            IRepository<ProductOffer> productOfferRepository,
            IRepository<Product> productRepository,
            IRepository<OfferWidgetZone> offerWidgetZoneRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IDbContext dbContext,
            IDataProvider dataProvider,
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
            IPriceFormatter priceFormatter)
        {
            this._cacheManager = cacheManager;
            this._offerRepository = offerRepository;
            this._productOfferRepository = productOfferRepository;
            this._productRepository = productRepository;
            this._offerWidgetZoneRepository = offerWidgetZoneRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
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


            if (offer is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            //cache
           // _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.CategoriesPrefixCacheKey);

            _offerRepository.Insert(offer);

            //cache
            _cacheManager.RemoveByPrefix(OFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(WIDGETZONEOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY);

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

            //cache
            _cacheManager.RemoveByPrefix(OFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(WIDGETZONEOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY);
            //event notification
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


        public void DeleteOfferWidgetZones(IList<OfferWidgetZone> OfferWidgetZones)
        {
            throw new NotImplementedException();
        }

        public IPagedList<OfferWidgetZone> GetAllWidgetZones()
        {
            throw new NotImplementedException();
        }

        public OfferWidgetZone GetOfferWidgetZoneById(int OfferWidgetZoneId)
        {
            if (OfferWidgetZoneId == 0)
                return null;

            return  _offerWidgetZoneRepository.GetById(OfferWidgetZoneId);

        }

        public List<string> GetOfferWidgetZones()
        {
          return  _offerWidgetZoneRepository.Table.ToList().Select(x => x.WidgetZone).ToList();
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

            var key = string.Format(PRODUCTOFFERS_ALLBYOFFERID_KEY, showHidden, offerId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from po in _productOfferRepository.Table
                            where po.OfferId == offerId 
                            orderby po.DisplayOrder, po.Id
                            select po;

                
                var productOffers = new PagedList<ProductOffer>(query, pageIndex, pageSize);
                return productOffers;
            });
        }

        public virtual List<ProductDetailsModel> GetProductDetailsByOfferId(int offerId)
        {
            if (offerId == 0)
                return new List<ProductDetailsModel>();

            var key = string.Format(PRODUCTDETAILSOFFERS_ALLBYOFFERID_KEY, offerId, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from po in _productOfferRepository.Table
                            where po.OfferId == offerId
                            orderby po.DisplayOrder, po.Id
                            select po;


                var productOffers = new List<ProductOffer>(query);
                    var prodModels = productOffers.Select(x =>
                    {
                        Product product = _productRepository.GetById(x.Id);
                        var productPresentationModel = new ProductPresenationModel();
                      var  model = _productModelFactory.PrepareProductDetailsModel(product, null, false);
                        
                        return model;
                    });
                return prodModels.ToList();
            });
        }

        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product</returns>
        public virtual Offer GetOfferById(int offerId)
        {
            if (offerId == 0)
                return null;

            var key = string.Format(OFFERS_BY_ID_KEY, offerId);
            return _cacheManager.Get(key, () => _offerRepository.GetById(offerId));
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

            //cache
            _cacheManager.RemoveByPrefix(OFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(WIDGETZONEOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY);
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

            //cache
            _cacheManager.RemoveByPrefix(OFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(WIDGETZONEOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY);
            //event notification
            _eventPublisher.EntityDeleted(productOffer);
        }
        public virtual IPagedList<Offer> GetAllOffers(string offerName = "",
            int storeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false)
        {
            var query = _offerRepository.Table;
            if (!showHidden)
                query = query.Where(m => m.Published);
            if (!string.IsNullOrWhiteSpace(offerName))
                query = query.Where(m => m.Name.Contains(offerName));
            query = query.Where(m => !m.Deleted);
            query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);

            if ((storeId > 0 ) || (!showHidden ))
            {
                if (!showHidden )
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                    var ACLQry = from acl in _aclRepository.Table
                                   where (acl.EntityName == _entityName && allowedCustomerRolesIds.Contains(acl.CustomerRoleId))
                                   select acl.EntityId;
                    var aclMap = ACLQry.ToList();

                    //ACL mapping
                    query = from g in query
                            where !g.SubjectToAcl || aclMap.Contains(g.Id)
                            select g;
                   
                }
                if (storeId > 0 )
                {
                    //Store mapping Net
                    var storeQry = from sm in _storeMappingRepository.Table
                                   where (sm.EntityName == _entityName && storeId == sm.StoreId)
                                   select sm.EntityId;
                    var storeMap = storeQry.ToList();

                    //Store mapping
                    query = from g in query
                            where !g.LimitedToStores || storeMap.Contains(g.Id)
                            select g;
                }
                //only distinct offers (group by ID)
                query = from m in query
                        group m by m.Id
                            into mGroup
                        orderby mGroup.Key
                        select mGroup.FirstOrDefault();
                query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);
            }

            return new PagedList<Offer>(query, pageIndex, pageSize);
        }
        /// Prepare paged blog post list model
        /// </summary>
        /// <param name="searchModel">Blog post search model</param>
        /// <returns>Blog post list model</returns>
        public virtual OfferListModel PrepareOfferListModel(OfferSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get blog posts
            var offers = GetAllOffers(offerName: searchModel.SearchOfferName, storeId: searchModel.SearchStoreId, showHidden: true,
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

        public IList<Offer> GetOffersByWidgetZoneName(string WidgetName = "")
        {
            var key = string.Format(ModelCacheEventConsumer.OFFERS_BY_WIDGET_ZONE_KEY, WidgetName, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from of in _offerRepository.Table
                        join wz in _offerWidgetZoneRepository.Table on of.Id equals wz.OfferId
                        where wz.WidgetZone.Equals(WidgetName) && of.Published==true
                        select of ;
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
            
            
                //ACL (access control list)
                var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                    var ACLQry = from acl in _aclRepository.Table
                                 where (acl.EntityName == _entityName && allowedCustomerRolesIds.Contains(acl.CustomerRoleId))
                                 select acl.EntityId;
                    var aclMap = ACLQry.ToList();

                    //ACL mapping
                    query = from g in query
                            where !g.SubjectToAcl || aclMap.Contains(g.Id)
                            select g;

            if (_storeContext.CurrentStore.Id > 0 )
            {
                
                    //Store mapping
                    var storeQry = from sm in _storeMappingRepository.Table
                                   where (sm.EntityName == _entityName && _storeContext.CurrentStore.Id == sm.StoreId)
                                   select sm.EntityId;
                    var storeMap = storeQry.ToList();

                    //Store mapping
                    query = from g in query
                            where !g.LimitedToStores || storeMap.Contains(g.Id)
                            select g;
                }
            var offerList= query.ToList();
            var deletedList = new List<Offer>();
            foreach (var item in offerList)
            {
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
                        if (DateTime.Now.Day%2!=0)
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
            });

        }

        public void InsertOfferWidgetZone(OfferWidgetZone OfferWidgetZone)
        {
            if (OfferWidgetZone == null)
                throw new ArgumentNullException(nameof(OfferWidgetZone));

            _offerWidgetZoneRepository.Insert(OfferWidgetZone);

            //cache
            _cacheManager.RemoveByPrefix(OFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(WIDGETZONEOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY);
            //event notification
            _eventPublisher.EntityInserted(OfferWidgetZone);

        }

       
        public virtual void UpdateOfferWidgetZone(OfferWidgetZone OfferWidgetZone)
        {
            if (OfferWidgetZone == null)
                throw new ArgumentNullException(nameof(OfferWidgetZone));

            _offerWidgetZoneRepository.Update(OfferWidgetZone);

            //cache
            _cacheManager.RemoveByPrefix(OFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(WIDGETZONEOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY);
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

            //cache
            _cacheManager.RemoveByPrefix(OFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(WIDGETZONEOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY);
            //event notification
            _eventPublisher.EntityDeleted(OfferWidgetZone);
        }


        public virtual void InsertProductOffer(ProductOffer productOffer)
        {
            if (productOffer == null)
                throw new ArgumentNullException(nameof(productOffer));

            _productOfferRepository.Insert(productOffer);

            //cache
            _cacheManager.RemoveByPrefix(OFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(PRODUCTOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(WIDGETZONEOFFERS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY);
            //event notification
            _eventPublisher.EntityInserted(productOffer);
        }

        public virtual IPagedList<OfferWidgetZone> GetOfferWidgetZonesByOfferId(int offerId,
    int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (offerId == 0)
                return new PagedList<OfferWidgetZone>(new List<OfferWidgetZone>(), pageIndex, pageSize);

            var key = string.Format(OFFERSWIDGETZONES_ALLBYOFFERID_KEY, showHidden, offerId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from po in _offerWidgetZoneRepository.Table
                            where po.OfferId == offerId 
                            orderby po.DisplayOrder, po.Id
                            select po;


                var productOffers = new PagedList<OfferWidgetZone>(query, pageIndex, pageSize);
                return productOffers;
            });
        }


       

     public   IList<ProductPresenationModel> PrepareProductPresentationModels(int offerId)
        {
            if (offerId == 0)
                return new List<ProductPresenationModel>();

            var key = string.Format(ModelCacheEventConsumer.PRODUCTOFFER_KEY, offerId, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from po in _productOfferRepository.Table
                            where po.OfferId == offerId
                            orderby po.DisplayOrder, po.Id
                            select po;


                var productOffers = new List<ProductOffer>(query);
                var prodModels = productOffers.Select(x =>
                {
                    Product product = _productRepository.GetById(x.ProductId);
                    
                   var  pOverviewModel= _productModelFactory.PrepareProductOverviewModels(new[] {product}.AsEnumerable<Product>()).FirstOrDefault();
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
                    productPresentationModel.PriceBeforDiscount =  _priceFormatter.FormatPrice(productPresentationModel.PriceBeforDiscountValue);
                    productPresentationModel.OldPrice =  _priceFormatter.FormatPrice(productPresentationModel.OldPriceValue);

                    productPresentationModel.Category = GetProductMainCategoryByProductId(product.Id, _storeContext.CurrentStore.Id, false);
                    productPresentationModel.EndDateDiscount =((productPresentationModel.PriceBeforDiscountValue>productPresentationModel.ProductPrice.PriceValue)&& product.AppliedDiscounts!=null && product.AppliedDiscounts.Count>0 && product.AppliedDiscounts.Min(y => y.EndDateUtc) >DateTime.UtcNow? product.AppliedDiscounts.Min(y => y.EndDateUtc):DateTime.MaxValue);
                    productPresentationModel.EndDateDiscount = (DateTime)productPresentationModel.EndDateDiscount.Value.ToLocalTime();
                    // DateTime dt = (DateTime)(productPresentationModel.EndDateDiscount.Value().ToString("tt", CultureInfo.InvariantCulture);
                    // productPresentationModel.EndDateDiscount =Convert.ToDateTime( productPresentationModel.EndDateDiscount.Value.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture));
                    if (product.HasDiscountsApplied == false && product.ProductType == ProductType.GroupedProduct)
                    {
                        productPresentationModel.PriceBeforDiscountValue = 0;
                        productPresentationModel.ProductPrice.PriceValue = 0;
                    }
                       
//                   if (product.ProductType==ProductType.GroupedProduct)
//                    productPresentationModel.PercentageDisountsValue = (productPresentationModel.OldPriceValue != 0 ? ((productPresentationModel.OldPriceValue-productPresentationModel.ProductPrice.PriceValue)/ productPresentationModel.OldPriceValue)*100 : 0);
//else
//                    productPresentationModel.PercentageDisountsValue = (productPresentationModel.OldPriceValue != 0 ? ((productPresentationModel.OldPriceValue-productPresentationModel.ProductPrice.PriceValue)/ productPresentationModel.OldPriceValue) : ((productPresentationModel.PriceBeforDiscountValue - productPresentationModel.ProductPrice.PriceValue)/ productPresentationModel.PriceBeforDiscountValue))*100;
                    return productPresentationModel;
                });
                return prodModels.ToList();
            });
        }

        public virtual Category GetProductMainCategoryByProductId(int productId, int storeId, bool showHidden = false)
        {
            if (productId == 0)
                return new Category();

            var key = string.Format(OFFERPRODUCTCATEGORY_MAINBYPRODUCTID_KEY, showHidden, productId, _workContext.CurrentCustomer.Id, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _productCategoryRepository.Table
                            join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                            where pc.ProductId == productId &&
                                  !c.Deleted &&
                                  (showHidden || c.Published)
                            orderby pc.DisplayOrder, pc.Id
                            select pc;

                var mainCategory = query.ToList().FirstOrDefault().Category;
                var result = new Category();
                if (!showHidden && mainCategory != null)
                {
                    //ACL (access control list) and store mapping
                    if (_aclService.Authorize(mainCategory) && _storeMappingService.Authorize(mainCategory, storeId))
                        result = mainCategory;
                }
                else
                {
                    //no filtering
                    return null;
                }
                return result;
            });
        }

    }
}
