
using IntentRecognizer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ML.Bot
{
    internal interface IOpenWeatherApiFacade
    {
        Task<string> GetCurrentWeatherAsync(string city);
    }
}