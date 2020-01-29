using System;
using System.Collections.Generic;
using System.Linq;

namespace HolidayOptimizer.Core.Models {
  public class LongestHolidayFinder {
    public SortedList<DateTime, HashSet<string>> HolidayCalendar { get; set; }

    public LongestHolidayFinder() {
      HolidayCalendar = new SortedList<DateTime, HashSet<string>>();
    }

    public void AddHoliday(DateTime date, string countryCode) {
      if (HolidayCalendar.ContainsKey(date)) {
        HolidayCalendar[date].Add(countryCode);
      }
      else {
        HolidayCalendar.Add(date, new HashSet<string>() { countryCode });
      }
    }

    public LongestHoliday GetLongestHoliday() {
      //first init
      var first = HolidayCalendar.First();
      var current = new LongestHoliday(first);
      var longest = current;

      foreach (var cal in HolidayCalendar.Skip(1)) {
        if (current.EndDateInUtc < cal.Key) {
          if (current.Time > longest.Time) {
            longest = current;
          }
          current = new LongestHoliday(cal);
        }
        else {
          current.Extend(cal);
        }
      }

      if (current.Time > longest.Time) {
        longest = current;
      }

      ClearMemory();
      return longest;
    }

    private void ClearMemory() {
      int identificador = GC.GetGeneration(HolidayCalendar);
      HolidayCalendar.Clear();
      GC.Collect(identificador, GCCollectionMode.Forced);
    }
  }
}
