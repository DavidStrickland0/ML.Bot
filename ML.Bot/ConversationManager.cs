using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using IntentRecognizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Bot.Builder;
using ML.Bot.Bots;
using QnAProcessor;

namespace ML.Bot
{
    class ConversationManager:IConversationManager
    {
        private IQnAMachineLearningFacade _qnaFacade;
        private IIntentRecognizerFacade _intentFacade;
        private IServiceProvider _serviceProvider;
        private IWeatherIntentHandler _weatherHandler;

        public ConversationManager(
            IQnAMachineLearningFacade qnaFacade,
            IIntentRecognizerFacade intentFacade,
            IWeatherIntentHandler weatherHandler,
            IServiceProvider serviceProvider)
        {
            _qnaFacade = qnaFacade;
            _intentFacade = intentFacade;
            _serviceProvider = serviceProvider;
            _weatherHandler = weatherHandler;
        }

        public async Task<Queue<string>> ProcessMessageAsync(
            ConversationData conversationData,
            string message, 
            CancellationToken cancellationToken)
        {

            var result = new Queue<string>();

            if (conversationData.ActiveIntent.Any())
            {
                var continueResult = await ContinuePreviousIntent(message, conversationData);
                return continueResult.Item2;
            }

            (IntentEnum, Dictionary<string,List<object>>) intent = _intentFacade.Predict(message);
                switch (intent.Item1)
                {
                    case IntentEnum.None:
                        var replyText = _qnaFacade.Predict(message);
                        result.Enqueue(replyText);
                        break;
                    case IntentEnum.Time:
                        result.Enqueue(DateTime.Now.ToShortTimeString());
                        break;
                    case IntentEnum.Date:
                        result.Enqueue(DateTime.Now.ToShortDateString());
                        break;
                    case IntentEnum.Weather:
                        var handledResult = await _weatherHandler.HandleAsync(message);
                        if (handledResult.Item1 == IntentResult.Waiting)
                        {
                            conversationData.ActiveIntent.Add(_weatherHandler.GetType().Name);
                        }
                        result.Enqueue(handledResult.Item2);
                        break;
            }
            // Save any state changes that might have occurred during the turn.
            return result;
        }

        private async Task<(IntentResult, Queue<string>)> ContinuePreviousIntent(string message, ConversationData conversationData)
        {

            var activeIntent = conversationData.ActiveIntent.Last();
            //Pops intent off the List
            conversationData.ActiveIntent.RemoveAt(conversationData.ActiveIntent.Count - 1);
            switch (activeIntent)
            {
                case "WeatherIntentHandler":
                    var handledResult = await _weatherHandler.HandleAsync(message);
                    if (handledResult.Item1 == IntentResult.Waiting)
                    {
                        conversationData.ActiveIntent.Add(_weatherHandler.GetType().Name);
                    }

                    var result = new Queue<string>();
                    result.Enqueue(handledResult.Item2);
                    return (handledResult.Item1, result);
                default:
                    return (IntentResult.None,null);
            }
        }
    }
}
