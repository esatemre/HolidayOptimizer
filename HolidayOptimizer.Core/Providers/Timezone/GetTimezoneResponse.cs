﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace HolidayOptimizer.Core.Providers.Timezone {
  /// <summary>
  /// {"name":"Netherlands","topLevelDomain":[".nl"],"alpha2Code":"NL","alpha3Code":"NLD","callingCodes":["31"],"capital":"Amsterdam","altSpellings":["NL","Holland","Nederland"],"region":"Europe","subregion":"Western Europe","population":17019800,"latlng":[52.5,5.75],"demonym":"Dutch","area":41850.0,"gini":30.9,
  /// "timezones":["UTC-04:00","UTC+01:00"],"borders":["BEL","DEU"],"nativeName":"Nederland","numericCode":"528","currencies":[{"code":"EUR","name":"Euro","symbol":"€"}],"languages":[{"iso639_1":"nl","iso639_2":"nld","name":"Dutch","nativeName":"Nederlands"}],"translations":{"de":"Niederlande","es":"Países Bajos","fr":"Pays-Bas",
  /// "ja":"オランダ","it":"Paesi Bassi","br":"Holanda","pt":"Países Baixos","nl":"Nederland","hr":"Nizozemska","fa":"پادشاهی هلند"},"flag":"https://restcountries.eu/data/nld.svg","regionalBlocs":[{"acronym":"EU","name":"European Union","otherAcronyms":[],"otherNames":[]}],"cioc":"NED"}
  /// </summary>
  public class GetTimezoneResponse {

    [JsonProperty("timezones")]
    public IEnumerable<string> Timezones { get; set; }

  }
}
