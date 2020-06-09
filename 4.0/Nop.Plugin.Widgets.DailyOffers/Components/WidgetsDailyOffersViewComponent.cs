using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.DailyOffers.Models.Offers;
using Nop.Plugin.Widgets.DailyOffers.Services.Offers;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Components
{
    [ViewComponent(Name = "WidgetsDailyOffers")]
    public class WidgetsDailyOffersViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IOfferService _offerService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        #endregion
        public WidgetsDailyOffersViewComponent(IOfferService offerService,
                                            IWorkContext workContext,
                                            IStoreContext storeContext,
                                            ISettingService settingService)
        {
            this._offerService = offerService;
            this._storeContext = storeContext;
            this._workContext = workContext;
            _settingService = settingService;
        }
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            ////Trial
            DateTime dtCreate = _settingService.GetSettingByKey<DateTime>("DailyOffersdt");
            if (dtCreate.AddDays(7) < DateTime.Now)
                return View("~/Plugins/Widgets.DailyOffers/Views/PublicInfo.cshtml", new List<OfferPresentationModel>());

            var offers = _offerService.GetOffersByWidgetZoneName(widgetZone);
            Product p = new Product();
            ProductOverviewModel po = new ProductOverviewModel();
            var offerModels = offers.Select(x =>
            {
                OfferPresentationModel model = new OfferPresentationModel
                {
                    Name = x.GetLocalized(l => l.Name),
                    CustomStyle=x.CustomStyle,
                    ShowName=x.ShowName,
                    AutoPlay=x.AutoPlay,
                    AutoPlaySpeed=x.AutoPlaySpeed,
                    ButtonClasses=x.ButtonClasses,
                    PauseOnHover=x.PauseOnHover,
                   
                    
                };
                model.productPresentationModels = _offerService.PrepareProductPresentationModels(x.Id);
                return model;
            });
            var result = offerModels.ToList();
            return View("~/Plugins/Widgets.DailyOffers/Views/PublicInfo.cshtml", result);


        }
    }


}
