namespace HolidayOptimizer.Core.Providers
{
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using Timezone;
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
        public async Task<GetAllTimezonesResponse> GetAllTimezones()
        {
            string cacheKey = "Timezones";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);

                foreach (var country in _supportedCountries.Get())
                {
                    var timezonesOfCountry = await GetTimezones(new GetTimezoneRequest() { CountryCode = country });
                }
                var timezones = new GetAllTimezonesResponse();
                return timezones;
            });
            
        }
    }

    public interface ITimezoneProvider
    {
        Task<GetTimezoneResponse> GetTimezones(GetTimezoneRequest request);
        Task<GetAllTimezonesResponse> GetAllTimezones();
    }
}
