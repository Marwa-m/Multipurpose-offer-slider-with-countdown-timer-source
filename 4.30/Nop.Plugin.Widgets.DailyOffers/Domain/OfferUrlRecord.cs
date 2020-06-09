using Nop.Core;
using Nop.Core.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers.Domain
{
   public partial class OfferUrlRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the Offer identifier
        /// </summary>
        public int OfferId { get; set; }

        /// <summary>
        /// Gets the offer
        /// </summary>
        public virtual Offer Offer { get; set; }

        /// <summary>
        /// Gets or sets the Offer identifier
        /// </summary>
        public int UrlRecordId { get; set; }

        /// <summary>
        /// Gets the offer
        /// </summary>
        public virtual UrlRecord UrlRecord { get; set; }

    }
}
