using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Domain.Offers
{
   public partial class OfferWidgetZone: BaseEntity
    {
        private ICollection<OfferWidgetZone> _offerWidgetZones;


        /// <summary>
        /// Gets or sets the Widget Zone
        /// </summary>
        public string WidgetZone { get; set; }
        /// <summary>
        /// Gets or sets a display order.
        /// This value is used when sorting associated offer (used with "grouped" products)
        /// This value is used when sorting home page offers
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int OfferId { get; set; }

        /// <summary>
        /// Gets the offer
        /// </summary>
        public virtual Offer Offer { get; set; }

        /// <summary>
        /// Gets or sets the collection of OfferWidgetZone
        /// </summary>
        public virtual ICollection<OfferWidgetZone> OfferWidgetZones
        {
            get { return _offerWidgetZones ?? (_offerWidgetZones = new List<OfferWidgetZone>()); }
            protected set { _offerWidgetZones = value; }
        }
    }
}
