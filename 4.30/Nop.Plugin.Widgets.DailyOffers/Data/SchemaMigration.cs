using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.DailyOffers.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/05/30 09:30:17:6455422", "Widgets.DailyOffers base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<Offer>(Create);
            _migrationManager.BuildTable<OfferWidgetZone>(Create);
            _migrationManager.BuildTable<ProductOffer>(Create);
            _migrationManager.BuildTable<OfferUrlRecord>(Create);


        }
    }
}
