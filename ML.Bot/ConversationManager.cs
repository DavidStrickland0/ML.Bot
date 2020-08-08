using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using IntentRecognizer;
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

        public ConversationManager(
            IQnAMachineLearningFacade qnaFacade,
            IIntentRecognizerFacade intentFacade,
            IWeatheMessageHandler weatherHandler,
            IServiceProvider serviceProvider)
        {
            _qnaFacade = qnaFacade;
            _intentFacade = intentFacade;
            _serviceProvider = serviceProvider;
        }

        public async Task<Queue<string>> ProcessMessageAsync(
            ConversationData conversationData,
            string message, 
            CancellationToken cancellationToken)
        {

            var result = new Queue<string>();
                var intent = _intentFacade.Predict(message);
                switch (intent)
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
                }
            // Save any state changes that might have occurred during the turn.
            return result;
        }
    }
}
