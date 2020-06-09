using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Models.Offers
{
    public partial class OfferPresentationModel : BaseNopEntityModel, ILocalizedModel<OfferLocalizedModel>
    {
        public OfferPresentationModel()
        {

            Locales = new List<OfferLocalizedModel>();
            productPresentationModels = new List<ProductPresenationModel>();
        }
        public string Name { get; set; }
        public string CustomStyle { get; set; }
        public bool ShowName { get; set; }
        public IList<OfferLocalizedModel> Locales { get; set; }
        public IList<ProductPresenationModel> productPresentationModels { get; set; }

        public bool PauseOnHover { get; set; }
        public bool AutoPlay { get; set; }
        public int AutoPlaySpeed { get; set; }
        public string ButtonClasses { get; set; }
    }

}
