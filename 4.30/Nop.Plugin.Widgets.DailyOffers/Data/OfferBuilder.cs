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
   public partial class OfferBuilder : NopEntityBuilder<Offer>
    {

        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Offer.Name)).AsString(500).NotNullable()
                .WithColumn(nameof(Offer.ButtonClasses)).AsString(500).NotNullable()
                .WithColumn(nameof(Offer.FromTimeUtc)).AsTime().Nullable()
                .WithColumn(nameof(Offer.ToTimeUtc)).AsTime().Nullable()
                .WithColumn(nameof(Offer.BackgroundColor)).AsString(100).Nullable()
                .WithColumn(nameof(Offer.BackgroundNumberBox)).AsString(100).Nullable()
                .WithColumn(nameof(Offer.CounterNumbersColor)).AsString(100).Nullable()
                .WithColumn(nameof(Offer.CounterWordsOldPriceColor)).AsString(100).Nullable()
                .WithColumn(nameof(Offer.PriceButtonBorderColor)).AsString(100).Nullable()
                .WithColumn(nameof(Offer.PrimaryColor)).AsString(100).Nullable()
                ;
        }

        #endregion
    }
}
