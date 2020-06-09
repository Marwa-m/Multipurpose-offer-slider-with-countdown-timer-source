using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.DailyOffers.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Data
{
   
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductOfferBuilder : NopEntityBuilder<ProductOffer>
    {

        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductOffer.ProductId)).AsInt32().ForeignKey().NotNullable()
                .WithColumn(nameof(ProductOffer.OfferId)).AsInt32().ForeignKey().NotNullable();


        }

        #endregion
    }

}
