using HolidayOptimizer.Core.Providers;
using HolidayOptimizer.Core.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayOptimizer.Test {
  public class HolidayOptimizeServiceTest {

    /*
     * SAMPLE RESULT FOR 2020 =>
     *{"result":{"time":{"ticks":12384000000000,"days":14,"hours":8,"milliseconds":0,"minutes":0,"seconds":0,"totalDays":14.333333333333334,"totalHours":344,"totalMilliseconds":1238400000,"totalMinutes":20640,"totalSeconds":1238400},
     *"startDateInUtc":"2020-10-30T22:00:00","endDateInUtc":"2020-11-14T06:00:00","visitedCountries":["FI","DE","SE","SI","FR","MG","BG","LT","AT","BE","CH","ES","GA","HR","HU","IT","LI","LU","PL","SK","SM","VA","PT","CL","VE","CO","HT","PE","GT","MC","BR","UY","BO","EC","SV","PA","RU","DO","BY","US","RS","CA","PR"]}}
     */

    private IHolidayOptimizeService holidayOptimizeService;
    [SetUp]
    public void Setup() {
      var publicHolidayProvider = new Mock<IPublicHolidayProvider>();
      publicHolidayProvider.Setup(x => x.GetAllPublicHolidays(It.IsAny<int>())).Returns(Task.FromResult(GenerateHolidays()));
      var timezoneProvider = new Mock<ITimezoneProvider>();
      timezoneProvider.Setup(x => x.GetAllTimezones()).Returns(Task.FromResult(GenerateTimezones()));
      holidayOptimizeService = new HolidayOptimizeService(publicHolidayProvider.Object, timezoneProvider.Object);
    }

    private IDictionary<string, IEnumerable<DateTime>> GenerateHolidays() {
      var response = new Dictionary<string, IEnumerable<DateTime>>();
      response.Add("TR", new List<DateTime>() { new DateTime(2020, 1, 1), new DateTime(2020, 1, 9), new DateTime(2020, 1, 10) });
      response.Add("NL", new List<DateTime>() { new DateTime(2020, 1, 3), new DateTime(2020, 1, 6), new DateTime(2020, 1, 7) });
      response.Add("BE", new List<DateTime>() { new DateTime(2020, 1, 4) });
      response.Add("DE", new List<DateTime>() { new DateTime(2020, 1, 5) });
      response.Add("RU", new List<DateTime>() { new DateTime(2020, 1, 6) });
      return response;
    }

    private IDictionary<string, IEnumerable<TimeSpan>> GenerateTimezones() {
      var response = new Dictionary<string, IEnumerable<TimeSpan>>();
      response.Add("TR", new List<TimeSpan>() { new TimeSpan(3, 0, 0) });
      response.Add("NL", new List<TimeSpan>() { new TimeSpan(-4, 0, 0), new TimeSpan(1, 0, 0) });
      response.Add("BE", new List<TimeSpan>() { new TimeSpan(1, 0, 0) });
      response.Add("DE", new List<TimeSpan>() { new TimeSpan(1, 0, 0) });
      response.Add("RU", new List<TimeSpan>() { new TimeSpan(3, 0, 0), new TimeSpan(4, 0, 0), new TimeSpan(7, 0, 0), new TimeSpan(12, 0, 0) });
      return response;
    }

    [Test]
    public async Task GetLongestHoliday() {
      var result = await holidayOptimizeService.GetLongestHoliday(2020);
      var begin = new DateTime(2020, 1, 2, 23, 0, 0);
      var end = new DateTime(2020, 1, 8, 4, 0, 0);
      Assert.NotNull(result);
      Assert.AreEqual(begin, result.StartDateInUtc);
      Assert.AreEqual(end, result.EndDateInUtc);
      Assert.AreEqual(new[] { "NL", "BE", "DE", "RU" }, result.VisitedCountries.ToArray());
      Assert.AreEqual(end.Subtract(begin), result.Time);
    }
  }
}