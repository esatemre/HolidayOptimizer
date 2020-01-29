using HolidayOptimizer.Core.Models;
using HolidayOptimizer.Core.Providers;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayOptimizer.Core.Services {
  public class HolidayOptimizeService : IHolidayOptimizeService {
    private IPublicHolidayProvider _publicHolidayProvider;
    private ITimezoneProvider _timezoneProvider;

    public HolidayOptimizeService(IPublicHolidayProvider publicHolidayProvider, ITimezoneProvider timezoneProvider) {
      _publicHolidayProvider = publicHolidayProvider;
      _timezoneProvider = timezoneProvider;
    }

    public async Task<LongestHoliday> GetLongestHoliday(int year) {
      var allHolidays = await _publicHolidayProvider.GetAllPublicHolidays(year);
      var timezones = await _timezoneProvider.GetAllTimezones();
      var longestHolidayFinder = new LongestHolidayFinder();

      foreach (var countryHolidays in allHolidays) {
        
        foreach (var holiday in countryHolidays.Value) {
          //get timezones of the country
          var timezonesOfCountry = timezones[countryHolidays.Key].ToArray();
      
          if (timezonesOfCountry.Length > 0) {
            //min and max timezone is quite enough instead of adding all timezones if there are multiple timezones
            longestHolidayFinder.AddHoliday(holiday.Subtract(timezonesOfCountry.Min()), countryHolidays.Key);
            if (timezonesOfCountry.Length > 1) {
              longestHolidayFinder.AddHoliday(holiday.Subtract(timezonesOfCountry.Max()), countryHolidays.Key);
            }
          }
        }
      }

      return longestHolidayFinder.GetLongestHoliday();
    }


  }
}
