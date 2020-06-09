using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Discounts;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nop.Web.Models.Catalog.ProductDetailsModel;

namespace Nop.Plugin.Widgets.DailyOffers.Models.Offers
{
    
    public partial class ProductPresenationModel : ProductOverviewModel
    {
        public ProductPresenationModel()
        {            
        }


        public Category Category { get; set; }

        public DateTime? EndDateDiscount { get; set; }
        public decimal PriceBeforDiscountValue { get; set; }
        public decimal PercentageDisountsValue { get; set; }
        public decimal OldPriceValue { get; set; }
        public string PriceBeforDiscount { get; set; }
        public string OldPrice { get; set; }

        #region Nested Classes


        #endregion
    }

}
