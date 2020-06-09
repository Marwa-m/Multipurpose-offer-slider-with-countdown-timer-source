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

            return sliders.ToList<string>();
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
       
        public override void Install()
        {
            _context.Install();
           
            InstallPermissions();
            #region LocalResource
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.List.SearchOfferName", "Offer Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.List.AvailableStores", "Available Stores");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Name", "Title");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ShowName", "Show Title");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.CustomStyle", "Custom Style");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.DisplayOrder", "Display Order");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Published", "Published");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Deleted", "Deleted");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.FromDate", "From Date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ToDate", "To Date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.FromTime", "From Time");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ToTime", "To Time");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.SchedulePattern", "Schedule Pattern");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AclCustomerRole", "Limited to customer roles");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.LimitedToStore", "Limited to stores");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.PauseOnHover", "Pause On Hover");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.PauseOnHover.hint", "Pause slider when mouse is on hover");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlay", "Auto Play");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlay.hint", "Slider is played automatically");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlaySpeed", "Auto Play Speed");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlaySpeed.hint", "Slider speed on miliseconds when autoplay is true");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ButtonClasses", "Button Classes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ButtonClasses.hint", "Assign classes to button");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.LeftButtonIcon", "Left button icon");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.RightButtonIcon", "Right button icon");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Product", "Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Offer", "Offer");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.DisplayOrder", "Display order");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.DisplayOrder.hint", "The product attribute display order. 1 represents the first item in the list.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Published", "Published");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.WidgetZone", "Widget Zone");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.Offer", "Offer");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.DisplayOrder", "Display order");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Added", "The new offer has been added successfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.AddNewOffer", "Added a new offer (ID = {0})");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Updated", "The offer has been updated successfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.EditOffer", "Edited an offer ('{0}')");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.DeleteOffer", "Deleted an offer ('{0}')");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Deleted", "The offer has been deleted successfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Info", "Info");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.WidgetZones", "Widget Zones");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.SliderSettings", "Slider Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Products", "Products");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Scheduling", "Scheduling");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.CustomStyle", "Custom Style");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.AddNew", "Add new product");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.SaveBeforeEdit", "You need to save the offer before you can add products for this offer page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.SchedulePattern", "Schedule Pattern");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Schedule", "Schedule");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.SaveBeforeEdit", "You need to save the offer before you can add schedule for this offer page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.AddNew", "Add new widget zone");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.AddNew", "Add new offer");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.BackToList", "Back To List");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.EditOfferDetails", "Edit offer details");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Manage", "Manage offer");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.ShopNow", "Shop Now");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Title", "Daily Offers");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Days", "Days");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Hours", "Hours");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Minutes", "Minutes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.Seconds", "Seconds");

            #endregion

            //For trial Version
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.DailyOffers.trialEnd", "Trial version has ended");

            if (_settings.GetSetting("DailyOffersdt") == null)
                _settings.SetSetting<DateTime>("DailyOffersdt", DateTime.Now);
            base.Install();
        }

        public override void Uninstall()
        {
            _context.Uninstall();
           

            UninstallPermissions();
            #region LocalResource
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.List.SearchOfferName");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.List.AvailableStores");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Name");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ShowName");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.CustomStyle");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.DisplayOrder");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Published");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.Deleted");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.FromDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ToDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.FromTime");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ToTime");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.SchedulePattern");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AclCustomerRole");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.LimitedToStore");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.PauseOnHover");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.PauseOnHover.hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlay");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlay.hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlaySpeed");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.AutoPlaySpeed.hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ButtonClasses");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.ButtonClasses.hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.LeftButtonIcon");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Fields.RightButtonIcon");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Product");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Offer");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.DisplayOrder");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.DisplayOrder.hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.Fields.Published");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.WidgetZone");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.Offer");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.Fields.DisplayOrder");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Added");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.AddNewOffer");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Updated");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.EditOffer");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.ActivityLog.DeleteOffer");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Deleted");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Info");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.WidgetZones");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.SliderSettings");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Products");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Scheduling");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.CustomStyle");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.OfferProduct.SaveBeforeEdit");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.SchedulePattern");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Schedule");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.SaveBeforeEdit");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.WidgetZone.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.BackToList");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.EditOfferDetails");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Offer.Manage");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.ShopNow");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Title");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Days");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Hours");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Minutes");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.Seconds");
            #endregion

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.DailyOffers.trialEnd");

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
                    CustomerRoleSystemName = NopCustomerDefaults.AdministratorsRoleName,
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
                var mappingExists = (from mapping in customerRole.PermissionRecordCustomerRoleMappings
                                     where mapping.PermissionRecord.SystemName == permission1.SystemName
                                     select mapping).Any();
                if (defaultMappingProvided && !mappingExists)
                {
                    permission1.PermissionRecordCustomerRoleMappings.Add(new PermissionRecordCustomerRoleMapping { CustomerRole = customerRole });
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

        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsDailyOffers";
        }
    }

}
