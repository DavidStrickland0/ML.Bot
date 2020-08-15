using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using IntentRecognizer;
using Microsoft.Bot.Builder;
using ML.Bot.Core;

namespace ML.Bot
{
    class WeatherIntentHandler : IntentHandler
    {
        private IOpenWeatherApiFacade _openWeatherApiFacade;

        public WeatherIntentHandler(IOpenWeatherApiFacade openWeatherApiFacade, ConversationState conversationState,
            IComponentContext context) : base(conversationState,context)
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

        protected async override Task<(string, Dictionary<string, IList<object>>)> PredictAsync(string message, Dictionary<string, IList<object>> entities)
        {
            var result = await base.PredictAsync(message, entities);
            if (message.Contains("Houston"))
            {
                result.Item2 = entities??new Dictionary<string, IList<object>>();
                result.Item2.Add("City", new List<object>() { "Houston" });
            }

            return result;
        }

        protected async override Task<(IntentResult, Queue<string>)> HandleIntentAsync(string intent, Dictionary<string, IList<object>> entities, ITurnContext turnContext,
            CancellationToken cancellationToken)
        {
            if (entities != null && entities.ContainsKey("City"))
            {
                var builder = new StringBuilder();
                foreach (var city in entities["City"])
                {
                    builder.Append(await _openWeatherApiFacade.GetCurrentWeatherAsync(city.ToString()));
                }

                var que = new Queue<string>();
                que.Enqueue(builder.ToString());
                return (IntentResult.Complete, que);
            }
            else
            {
                //Pushes Intent Onto Stack
                var que = new Queue<string>();
                que.Enqueue("What City did you want to check the weather in?");
                return (IntentResult.Waiting, que);
            }
        }


    }
}
