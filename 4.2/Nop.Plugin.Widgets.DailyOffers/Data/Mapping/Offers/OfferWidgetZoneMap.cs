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
   public partial class OfferWidgetZoneMap : NopEntityTypeConfiguration<OfferWidgetZone>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<OfferWidgetZone> builder)
        {
            builder.ToTable("OfferWidgetZone");
            builder.HasKey(p => p.Id);

            base.Configure(builder);
        }

        #endregion

    }

}
