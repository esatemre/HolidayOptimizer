using HolidayOptimizer.Core.Models;
using System.Threading.Tasks;

namespace HolidayOptimizer.Core.Services {
  public interface IHolidayOptimizeService {
    Task<LongestHoliday> GetLongestHoliday(int year);
  }
}
