namespace HolidayOptimizer.Core.Providers {
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using PublicHoliday;
  using System;

  public interface IPublicHolidayProvider {
    Task<IEnumerable<GetPublicHolidayResponse>> GetPublicHolidays(GetPublicHolidayRequest request);
    Task<IDictionary<string, IEnumerable<DateTime>>> GetAllPublicHolidays(int year);
  }
}
