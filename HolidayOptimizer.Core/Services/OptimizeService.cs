using HolidayOptimizer.Core.Providers;
using System.Threading.Tasks;

namespace HolidayOptimizer.Core.Services
{
    public class HolidayOptimizeService : IHolidayOptimizeService
    {
        private IPublicHolidayProvider _publicHolidayProvider;
        private ITimezoneProvider _timezoneProvider;
        
        public HolidayOptimizeService(IPublicHolidayProvider publicHolidayProvider, ITimezoneProvider timezoneProvider)
        {
            _publicHolidayProvider = publicHolidayProvider;
            _timezoneProvider = timezoneProvider;
        }

        public async Task<object> GetLongestHoliday(int year)
        {
            var allHolidays = _publicHolidayProvider.GetAllPublicHolidays(year);
            var timezones = _timezoneProvider.GetAllTimezones();

            return new string[] { "TR", "DE", "US", year.ToString() };
        }


    }

    public interface IHolidayOptimizeService
    {
        Task<object> GetLongestHoliday(int year);
    }
}
