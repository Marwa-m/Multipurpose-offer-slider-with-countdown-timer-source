using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.DailyOffers.Data;
using Nop.Plugin.Widgets.DailyOffers.Domain.Offers;
using Nop.Plugin.Widgets.DailyOffers.Infrastructure;
using Nop.Plugin.Widgets.DailyOffers.Services.Offers;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers
{
    public class DailyOffersPlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin, IPermissionProvider
    {
        private static readonly string systemName = "ManageDailyOffers";
        private static readonly string MenusystemName = "DailyOffers";
        public static readonly PermissionRecord ManageDailyOffersPermission = new PermissionRecord { Name = "Admin area. Manage Daily Offers", SystemName = systemName, Category = "Plugins" };

        private DailyOfferObjectContext _context;
        private IRepository<Offer> _offerRepo;
        private IOfferService _offerService;
        private IPermissionService _permissionService;
        private ISettingService _settings;
        private readonly ICustomerService _customerService;
        private ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private IRepository<OfferWidgetZone> _widgetZonesRepo;
        public DailyOffersPlugin(DailyOfferObjectContext context, IRepository<Offer> offerRepo, IPermissionService permissionService,
            ISettingService commonSettings, ICustomerService customerService, ILocalizationService localizationService,
            IWebHelper webHelper,IOfferService offerService, IRepository<OfferWidgetZone> widgetZoneRepo)
        {
            _context = context;
            _offerRepo = offerRepo;
            _settings = commonSettings;
            _permissionService = permissionService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            _webHelper = webHelper;
            _offerService = offerService;
            _widgetZonesRepo = widgetZoneRepo;
        }

        public IList<string> GetWidgetZones()
        {
            var sliders = _widgetZonesRepo.Table.Select(x=>x.WidgetZone).ToArray<string>();
            var zoneNames = new List<string>();
            zoneNames.Add("home_page_top");
            var aa =  sliders.ToList<string>() ;
            return aa;
        }
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "List";
            controllerName = "Offer";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.DailyOffers.Controllers" }, { "area", null } };
        }
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/Offer/List";
        }
        public void GetPublicViewComponent(string widgetZone, out string viewComponentName)
        {
            viewComponentName = "WidgetsDailyOffers";
        }
        public override void Install()
        {
            _context.Install();
           
            InstallPermissions();
            #region LocalResource
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.List.SearchOfferName", "Offer Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.List.AvailableStores", "Available Stores");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Name", "Title");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ShowName", "Show Title");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.CustomStyle", "Custom Style");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.DisplayOrder", "Display Order");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Published", "Published");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Deleted", "Deleted");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.FromDate", "From Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ToDate", "To Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.FromTime", "From Time");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ToTime", "To Time");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.SchedulePattern", "Schedule Pattern");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AclCustomerRoles", "Limited to customer roles");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.LimitedToStores", "Limited to stores");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.PauseOnHover", "Pause On Hover");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.PauseOnHover.hint", "Pause slider when mouse is on hover");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlay", "Auto Play");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlay.hint", "Slider is played automatically");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlaySpeed", "Auto Play Speed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlaySpeed.hint", "Slider speed on miliseconds when autoplay is true");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ButtonClasses", "Button Classes");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ButtonClasses.hint", "Assign classes to button");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.LeftButtonIcon", "Left button icon");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.RightButtonIcon", "Right button icon");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Product", "Product");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Offer", "Offer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.DisplayOrder", "Display order");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.DisplayOrder.hint", "The product attribute display order. 1 represents the first item in the list.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Published", "Published");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.WidgetZone", "Widget Zone");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.Offer", "Offer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.DisplayOrder", "Display order");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Added", "The new offer has been added successfully.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.AddNewOffer", "Added a new offer (ID = {0})");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Updated", "The offer has been updated successfully.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.EditOffer", "Edited an offer ('{0}')");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.DeleteOffer", "Deleted an offer ('{0}')");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Deleted", "The offer has been deleted successfully.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Info", "Info");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.WidgetZones", "Widget Zones");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.SliderSettings", "Slider Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Products", "Products");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Scheduling", "Scheduling");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.CustomStyle", "Custom Style");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.AddNew", "Add new product");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.SaveBeforeEdit", "You need to save the offer before you can add products for this offer page.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.SchedulePattern", "Schedule Pattern");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Schedule", "Schedule");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.SaveBeforeEdit", "You need to save the offer before you can add schedule for this offer page.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.AddNew", "Add new widget zone");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.AddNew", "Add new offer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.BackToList", "Back To List");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.EditOfferDetails", "Edit offer details");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Manage", "Manage offer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.ShopNow", "Shop Now");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Title", "Daily Offers");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Days", "Days");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Hours", "Hours");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Minutes", "Minutes");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Seconds", "Seconds");

            #endregion
            //For trial Version
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.trialEnd", "Trial version has ended");

            if (_settings.GetSetting("DailyOffersdt") == null)
                _settings.SetSetting<DateTime>("DailyOffersdt", DateTime.Now);

            base.Install();
        }

        public override void Uninstall()
        {
            _context.Uninstall();
           

            UninstallPermissions();
            #region LocalResource
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.List.SearchOfferName");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.List.AvailableStores");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Name");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ShowName");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.CustomStyle");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.DisplayOrder");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Published");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Deleted");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.FromDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ToDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.FromTime");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ToTime");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.SchedulePattern");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AclCustomerRoles");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.LimitedToStores");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.PauseOnHover");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.PauseOnHover.hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlay");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlay.hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlaySpeed");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlaySpeed.hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ButtonClasses");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ButtonClasses.hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.LeftButtonIcon");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.RightButtonIcon");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Product");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Offer");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.DisplayOrder");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.DisplayOrder.hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Published");

            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.WidgetZone");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.Offer");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.DisplayOrder");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Added");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.AddNewOffer");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Updated");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.EditOffer");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.DeleteOffer");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Deleted");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Info");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.WidgetZones");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.SliderSettings");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Products");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Scheduling");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.CustomStyle");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.AddNew");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.SaveBeforeEdit");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.SchedulePattern");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Schedule");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.SaveBeforeEdit");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.AddNew");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.AddNew");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.BackToList");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.EditOfferDetails");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Manage");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.ShopNow");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Title");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Days");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Hours");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Minutes");
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Seconds");
            #endregion
            this.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.trialEnd");
            base.Uninstall();
        }
        public bool Authenticate()
        {
            return true;
        }


        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    SystemName = "Third party plugins",
                    Title = _localizationService.GetResource("Plugins"),
                    Visible = true,
                    IconClass = "fa-list"
                };

            }
            var OffersPluginNode = pluginNode.ChildNodes.FirstOrDefault(x => x.SystemName == MenusystemName);
            if (OffersPluginNode == null)
            {
                OffersPluginNode = new SiteMapNode()
                {
                    SystemName = MenusystemName,
                  Title=  _localizationService.GetResource("Plugins.Widgets.DailyOffers.Title"),
                    Visible = true,
                    IconClass = "fa-gear",
                    Url = "~/Admin/Offer/List"
                };

                pluginNode.ChildNodes.Add(OffersPluginNode);
            }

        }


        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageDailyOffersPermission
            };
        }

        public IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        ManageDailyOffersPermission
                    }
                }
            };
        }
        public virtual void InstallPermissions()
        {
            _permissionService.InsertPermissionRecord(ManageDailyOffersPermission);

            //install new permissions
            //        var permissions = GetPermissions();
            var permission1 = ManageDailyOffersPermission;



            //default customer role mappings
            var defaultPermissions = GetDefaultPermissions();
            foreach (var defaultPermission in defaultPermissions)
            {
                var customerRole = _customerService.GetCustomerRoleBySystemName(defaultPermission.CustomerRoleSystemName);
                if (customerRole == null)
                {
                    //new role (save it)
                    customerRole = new CustomerRole
                    {
                        Name = defaultPermission.CustomerRoleSystemName,
                        Active = true,
                        SystemName = defaultPermission.CustomerRoleSystemName
                    };
                    _customerService.InsertCustomerRole(customerRole);
                }


                var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                                              where p.SystemName == permission1.SystemName
                                              select p).Any();
                var mappingExists = (from p in customerRole.PermissionRecords
                                     where p.SystemName == permission1.SystemName
                                     select p).Any();
                if (defaultMappingProvided && !mappingExists)
                {
                    permission1.CustomerRoles.Add(customerRole);
                }
            }

        }
        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void UninstallPermissions()
        {
            var pm = _permissionService.GetPermissionRecordBySystemName(systemName);
            if (pm != null)
            {
                _permissionService.DeletePermissionRecord(pm);
            }
            //default customer role mappings
            //var defaultPermissions = GetDefaultPermissions();
            //foreach (var defaultPermission in defaultPermissions)
            //{
            //    var customerRole = _customerService.GetCustomerRoleBySystemName(defaultPermission.CustomerRoleSystemName);
            //    if (customerRole != null)
            //    {
            //        //delete role 

            //        _customerService.DeleteCustomerRole(customerRole);
            //    }


            //}

        }


    }

}
