// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using IntentRecognizer;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ML.Bot
{
    class OpenWeatherApiFacade : IOpenWeatherApiFacade
    {
        private string _openWeatherApiKey;
        private HttpClient _httpClient;

        public OpenWeatherApiFacade(string openWeatherApiKey, HttpClient httpClient)
        {
            _openWeatherApiKey = openWeatherApiKey;
            _httpClient = httpClient;
        }

        public async Task<string> GetCurrentWeatherAsync(string cityName)
        {
            var weatherUrl = $"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={_openWeatherApiKey}";

            var weather =
                await _httpClient.GetStringAsync(weatherUrl);
            return weather;
        }
    }

}