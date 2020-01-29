namespace HolidayOptimizer.Core.Providers {
  using System.Threading.Tasks;
  using System;
  using Timezone;
  using System.Collections.Generic;

  public interface ITimezoneProvider
    {
        Task<GetTimezoneResponse> GetTimezones(GetTimezoneRequest request);
        Task<IDictionary<string, IEnumerable<TimeSpan>>> GetAllTimezones();
    }
}
