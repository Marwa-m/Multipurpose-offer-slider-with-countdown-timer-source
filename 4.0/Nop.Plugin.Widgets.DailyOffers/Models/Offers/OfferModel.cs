using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Models.Offers
{
    public partial class OfferModel: BaseNopEntityModel, ILocalizedModel<OfferLocalizedModel>
    {
        public OfferModel()
        {

            Locales = new List<OfferLocalizedModel>();
            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            AvailableSchedulePatterns = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.ShowName")]
        public bool ShowName { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.CustomStyle")]
        public string CustomStyle { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.FromDate")]
        [UIHint("DateNullable")]
        public DateTime? FromDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.ToDate")]
        [UIHint("DateNullable")]
        public DateTime? ToDate { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.FromTime")]
        [UIHint("~.Plugins.Widgets.DailyOffers.Views.Shared.EditorTemplates.TimeNullable")]
        public TimeSpan? FromTimeUtc { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.ToTime")]
        [UIHint("Plugins.Widgets.DailyOffers.Views.Shared.EditorTemplates.TimeNullable")]
        public TimeSpan? ToTimeUtc { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.SchedulePattern")]
        public int SchedulePatternId { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.SchedulePattern")]
        public string SchedulePatternIdName { get; set; }
        public IList<OfferLocalizedModel> Locales { get; set; }
        //ACL (customer roles)
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.AclCustomerRoles")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        //store mapping
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableSchedulePatterns { get; set; }

        #region setting slider
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.PauseOnHover")]
        public bool PauseOnHover { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlay")]
        public bool AutoPlay { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlaySpeed")]
        public int AutoPlaySpeed { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.ButtonClasses")]
        public string ButtonClasses { get; set; }
        #endregion
        #region Nested classes
        public partial class OfferProductModel : BaseNopEntityModel
        {
            public int OfferId { get; set; }

            public int ProductId { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Product")]
            public string ProductName { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Offer")]
            public string OfferName { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.DailyOffers.OfferProduct.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }
        public partial class AddOfferProductModel : BaseNopModel
        {
            public AddOfferProductModel()
            {
                AvailableOffers = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchOffer")]
            public int SearchOfferId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableOffers { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public int OfferId { get; set; }

            public int[] SelectedProductIds { get; set; }
        }

        public partial class OfferWidgetZoneModel : BaseNopEntityModel
        {
            public int OfferId { get; set; }

            public int WidgetZoneId { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.DailyOffers.WidgetZone.Fields.WidgetZone")]
            public string WidgetZoneName { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.DailyOffers.WidgetZone.Fields.Offer")]
            public string OfferName { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.DailyOffers.WidgetZone.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }
        public partial class AddOfferWidgetZoneModel : BaseNopModel
        {
            public AddOfferWidgetZoneModel()
            {
                AvailableWidgetZones = new List<SelectListItem>();
            }
            [NopResourceDisplayName("Plugins.Widgets.DailyOffers.WidgetZone.Fields.WidgetZone")]
            public string WidgetZone { get; set; }
            [NopResourceDisplayName("Plugins.Widgets.DailyOffers.WidgetZone.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
            public IList<SelectListItem> AvailableWidgetZones { get; set; }

            public int OfferId { get; set; }

        }

        #endregion
    }
    public partial class OfferLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.DailyOffers.Offer.Fields.Name")]
        public string Name { get; set; }


    }

}
