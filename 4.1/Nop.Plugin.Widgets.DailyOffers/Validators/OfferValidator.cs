using FluentValidation;
using Nop.Data;
using Nop.Plugin.Widgets.DailyOffers.Domain.Offers;
using Nop.Plugin.Widgets.DailyOffers.Models.Offers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Validators
{
    
    public partial class OfferValidator : BaseNopValidator<OfferModel>
    {
        public OfferValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));

            SetDatabaseValidationRules<Offer>(dbContext);
        }
    }
}
