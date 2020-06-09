using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        

        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductOffer> builder)
        {
            builder.ToTable("Product_Offer_Mapping");
            builder.HasKey(p => p.Id);
            builder.HasOne(pc => pc.Offer)
                .WithMany()
                .HasForeignKey(pc => pc.OfferId)
                .IsRequired();
            base.Configure(builder);
        }

        #endregion
    }

}
