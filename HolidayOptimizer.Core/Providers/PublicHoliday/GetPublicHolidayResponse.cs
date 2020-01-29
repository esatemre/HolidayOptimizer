using Newtonsoft.Json;
using System;

namespace HolidayOptimizer.Core.Providers.PublicHoliday {
  /// <summary>
  /// {"date":"2020-01-01","localName":"Nieuwjaarsdag","name":"New Year's Day","countryCode":"NL","fixed":true,"global":true,"counties":null,"launchYear":1967,"type":"Public"}
  /// </summary>
  public class GetPublicHolidayResponse {
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
  }
}
