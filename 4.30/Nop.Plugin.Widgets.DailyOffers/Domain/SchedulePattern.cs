using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.DailyOffers.Domain
{
    /// <summary>
    /// Represents a SchedulePattern
    /// </summary>
    public enum SchedulePattern
    {
        EveryDay=0,
        /// <summary>
        /// Simple
        /// </summary>
        EveryMonth = 5,
        OnOddDay = 10,
        OnEvenDay=15,
        OnMonday=20,
        OnTuesday=25,
        OnWednesday=30,
        OnThursday=35,
        OnFriday=40,
        OnSaturday=45,
        OnSunday=50,
        FromSundayToThursday=55,
        FromMondayToFriday=60,
        OnThursdayAndFriday=65,
        OnFridayAndSaturday=70,
        OnSaturdayAndSunday=75,
    }

}
