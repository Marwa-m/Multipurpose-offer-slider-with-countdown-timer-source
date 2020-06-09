using Nop.Plugin.Widgets.DailyOffers.Domain.Offers;
using Nop.Plugin.Widgets.DailyOffers.Extensions;
using Nop.Plugin.Widgets.DailyOffers.Models.Offers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers.Factories
{
    public partial class OfferModelFactory:IOfferModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;

        public OfferModelFactory(ILocalizationService localizationService, ILocalizedModelFactory localizedModelFactory)
        {
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
        }

        #endregion
        #region Ctor

        #endregion
        public OfferModel PrepareOfferModel(OfferModel model, Offer offer, bool excludeProperties = false)
        {
            Action<OfferLocalizedModel, int> localizedModelConfiguration = null;

            if (offer != null)
            {
                //fill in model values from the entity
                model = model ?? offer.ToModel<OfferModel>();

                

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

            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);


            return model;
        }

        
    }
}
