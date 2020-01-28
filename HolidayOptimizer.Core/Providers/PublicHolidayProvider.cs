namespace HolidayOptimizer.Core.Providers
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using PublicHoliday;
    using System;

    public class PublicHolidayProvider : IPublicHolidayProvider
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISupportedCountries _supportedCountries;
        private IMemoryCache _cache;

        public PublicHolidayProvider(IHttpClientFactory httpClientFactory, ISupportedCountries supportedCountries, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _supportedCountries = supportedCountries;
            _cache = memoryCache;
        }

        public async Task<IEnumerable<GetPublicHolidayResponse>> GetPublicHolidays(GetPublicHolidayRequest request)
        {
            string cacheKey = string.Format("PublicHolidays/{0}/{1}", request.Year, request.CountryCode);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);

                // Get an HttpClient configured to the specification you defined in StartUp.
                var client = _httpClientFactory.CreateClient("PublicHolidayApi");
                var response = await client.GetStringAsync(string.Format("{0}/{1}", request.Year, request.CountryCode));
                var holidays = JsonConvert.DeserializeObject<GetPublicHolidayResponse[]>(response);
                return holidays;
            });

            
        }

        public async Task<IDictionary<string, IEnumerable<GetPublicHolidayResponse>>> GetAllPublicHolidays(int year)
        {
            string cacheKey = string.Format("PublicHolidays/{0}", year);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);

                var result = new Dictionary<string, IEnumerable<GetPublicHolidayResponse>>();
                foreach (var country in _supportedCountries.Get())
                {
                    var publicholidaysOfCountry = await GetPublicHolidays(new GetPublicHolidayRequest() { CountryCode = country, Year = year });
                    result.Add(country, publicholidaysOfCountry);
                }
                return result;
            });
            
        }


    }

    public interface IPublicHolidayProvider
    {
        Task<IEnumerable<GetPublicHolidayResponse>> GetPublicHolidays(GetPublicHolidayRequest request);
        Task<IDictionary<string, IEnumerable<GetPublicHolidayResponse>>> GetAllPublicHolidays(int year);
    }
}
