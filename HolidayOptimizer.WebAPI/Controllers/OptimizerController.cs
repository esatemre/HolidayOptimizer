using System;
using System.Threading.Tasks;
using HolidayOptimizer.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HolidayOptimizer.WebAPI.Controllers {
  [ApiController]
  [Route("api/optimizer")]
  public class OptimizerController : ControllerBase {
    private IHolidayOptimizeService _holidayOptimizeService;
    public OptimizerController(IHolidayOptimizeService holidayOptimizeService) {
      _holidayOptimizeService = holidayOptimizeService;
    }

    /// <summary>
    /// Gets the longest lasting sequence of holidays around the world for a specific year.
    /// </summary>
    /// <param name="year">The specific year for the search.</param>
    /// <returns>The date range and country codes for each date.</returns>
    [HttpGet("getlongestholiday/{year}")]
    public async Task<JsonResult> GetLongestHoliday(int year) {
      var result = await _holidayOptimizeService.GetLongestHoliday(year);
      return new JsonResult(new { result });
    }
  }
}
