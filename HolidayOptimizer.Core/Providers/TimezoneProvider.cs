using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System;
using HolidayOptimizer.Core.Providers.Timezone;
using System.Collections.Generic;

namespace HolidayOptimizer.Core.Providers {
  public class TimezoneProvider : ITimezoneProvider
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISupportedCountries _supportedCountries;
        private IMemoryCache _cache;

        public TimezoneProvider(IHttpClientFactory httpClientFactory, ISupportedCountries supportedCountries, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _supportedCountries = supportedCountries;
            _cache = memoryCache;
        }

        public async Task<GetTimezoneResponse> GetTimezones(GetTimezoneRequest request)
        {
            string cacheKey = string.Format("Timezones/{0}", request.CountryCode);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);

                // Get an HttpClient configured to the specification you defined in StartUp.
                var client = _httpClientFactory.CreateClient("TimezoneApi");
                var response = await client.GetStringAsync(string.Format("{0}", request.CountryCode));
                var timezones = JsonConvert.DeserializeObject<GetTimezoneResponse>(response);
                return timezones;
            });
            
        }
        public async Task<IDictionary<string, IEnumerable<TimeSpan>>> GetAllTimezones()
        {
            string cacheKey = "Timezones";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);

                var timezones = new Dictionary<string, IEnumerable<TimeSpan>>();
                foreach (var country in _supportedCountries.Get())
                {
                    //Get country timezones
                    var timezonesOfCountry = await GetTimezones(new GetTimezoneRequest() { CountryCode = country });
                    var timeSpans= new List<TimeSpan>();
                    //parse country timezones into timespans
                    foreach (var item in timezonesOfCountry.Timezones) {
                        TimeSpan span;
                        if (TimeSpan.TryParse(item.Replace("UTC", "").Replace("+", ""), out span)) 
                        {
                            timeSpans.Add(span);
                        }
                    }
                    timezones.Add(country, timeSpans);
                }
                return timezones;
            });
            
        }
    }
}
