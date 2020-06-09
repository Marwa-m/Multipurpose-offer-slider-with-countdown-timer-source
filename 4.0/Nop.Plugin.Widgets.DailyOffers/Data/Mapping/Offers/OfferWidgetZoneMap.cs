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
        public OfferWidgetZoneMap()
        {
            this.ToTable("OfferWidgetZone");
            this.HasKey(p => p.Id);
        }

    }

}
