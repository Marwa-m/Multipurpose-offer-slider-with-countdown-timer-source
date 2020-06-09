using Nop.Data.Mapping;
using Nop.Plugin.Widgets.DailyOffers.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Data.Mapping.Offers
{
   
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductOfferMap : NopEntityTypeConfiguration<ProductOffer>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ProductOfferMap()
        {
            this.ToTable("Product_Offer_Mapping");
            this.HasKey(pc => pc.Id);

            //this.HasRequired(pc => pc.Product)
            //    .WithMany(p => p.ProductOffers)
            //    .HasForeignKey(pc => pc.ProductId);

      
            this.HasRequired(pc => pc.Offer)
               .WithMany()
               .HasForeignKey(pc => pc.OfferId);
        }
    }

}
