using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.DailyOffers.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers.Data
{
        public partial class OfferSliderUrlRecordBuilder : NopEntityBuilder<OfferUrlRecord>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OfferUrlRecord.UrlRecordId)).AsInt32().ForeignKey().NotNullable()
                .WithColumn(nameof(OfferUrlRecord.OfferId)).AsInt32().ForeignKey().NotNullable();

        }

        #endregion
    }
}
