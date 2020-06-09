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
   public partial class OfferMap : NopEntityTypeConfiguration<Offer>
    {
        
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.ToTable("Offer");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasMaxLength(500);
            builder.Property(p => p.ButtonClasses).HasMaxLength(500);

            base.Configure(builder);
        }

        #endregion
    }
}
