using System;
using System.Collections.Generic;

namespace HolidayOptimizer.Core.Models {
  public class LongestHoliday {
    public TimeSpan Time { get { return EndDateInUtc - StartDateInUtc; }   }
    public DateTime StartDateInUtc { get; set; }
    public DateTime EndDateInUtc { get; set; }
    public HashSet<string> VisitedCountries { get; set; }

    public LongestHoliday(KeyValuePair<DateTime, HashSet<string>> holiday) {
      StartDateInUtc = holiday.Key; //startDateInUtc
      EndDateInUtc = holiday.Key.AddHours(24);
      VisitedCountries = new HashSet<string>();
      VisitedCountries.UnionWith(holiday.Value); //country codes
    }

    public void Extend(KeyValuePair<DateTime, HashSet<string>> holiday) {
      EndDateInUtc = holiday.Key.AddHours(24);
      VisitedCountries.UnionWith(holiday.Value);
    }
  }
}
