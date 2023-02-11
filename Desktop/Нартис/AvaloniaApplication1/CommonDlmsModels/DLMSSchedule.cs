using Gurux.DLMS;
using Gurux.DLMS.Objects;
using System;
using System.Collections.Generic;

namespace DLMSClient.Net
{
    public class DLMSSchedule
    {
        public List<GXDLMSSeasonProfile> SeasonProfiles { get; set; }
        public List<GXDLMSWeekProfile> WeekProfiles { get; set; }
        public List<GXDLMSDayProfile> DayProfiles { get; set; }
        public List<GXDLMSSpecialDay> SpecialDays { get; set; }
        public bool ActivateNow { get; set; }
        public DateTime ActivationDate { get; set; }

        public DLMSSchedule()
        {

        }
    }
}