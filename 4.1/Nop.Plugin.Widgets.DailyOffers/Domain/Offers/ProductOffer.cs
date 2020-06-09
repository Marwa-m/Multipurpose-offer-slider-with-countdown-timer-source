using Nop.Core;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Domain.Offers
{
    /// <summary>
    /// Represents a product category mapping
    /// </summary>
    public partial class ProductOffer : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }



        /// <summary>
        /// Gets or sets the display order
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
        /// Gets the product
        /// </summary>
       // public virtual Product Product { get; set; }

    }

}
