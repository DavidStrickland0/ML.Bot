using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntentRecognizer;

namespace ML.Bot
{
    class WeatherIntentHandler : IWeatherIntentHandler
    {
        private IOpenWeatherApiFacade _openWeatherApiFacade;

        public WeatherIntentHandler(IOpenWeatherApiFacade openWeatherApiFacade)
        {
            _openWeatherApiFacade = openWeatherApiFacade;
        }

        private Dictionary<string, List<object>> extractEntities(string message)
        {
            var entities = new Dictionary<string, List<object>>();
            if (message.Contains("Houston"))
            {
                entities.Add("City",new List<object>(){"Houston"});
            }

            return entities;
        }

        public async Task<(IntentResult, string)> HandleAsync(string message)
        {
            Dictionary<string, List<object>> entities = extractEntities(message);
            if (entities != null && entities.ContainsKey("City"))
            {
                var builder = new StringBuilder();
                foreach (var city in entities["City"])
                {
                    builder.Append(await _openWeatherApiFacade.GetCurrentWeatherAsync(city.ToString()));
                }

                return (IntentResult.Complete,builder.ToString());
            }
            else
            {
                //Pushes Intent Onto Stack
                return (IntentResult.Waiting,"What City did you want to check the weather in?");
            }
        }


    }
}
