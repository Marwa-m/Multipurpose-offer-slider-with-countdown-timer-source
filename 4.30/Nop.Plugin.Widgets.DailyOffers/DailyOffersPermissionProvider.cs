using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.DailyOffers
{
        public partial class DailyOffersPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ManageDailyOffersPermission = new PermissionRecord { Name = "Admin area. Manage Daily Offers", SystemName = "ManageDailyOffers", Category = "Plugins" };

        HashSet<(string systemRoleName, PermissionRecord[] permissions)> IPermissionProvider.GetDefaultPermissions()
        {
            return new HashSet<(string, PermissionRecord[])>
            {
                (
                    NopCustomerDefaults.AdministratorsRoleName,
                    new[]
                    {
                        ManageDailyOffersPermission
                    }
                ),
            };
        }


        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageDailyOffersPermission
            };
        }

    }

}
