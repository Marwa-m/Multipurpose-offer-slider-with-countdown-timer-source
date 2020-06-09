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
        public OfferMap()
        {
            this.ToTable("Offer");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).HasMaxLength(500);
            this.Property(p => p.ButtonClasses).HasMaxLength(500);
          

        }
    }
}
