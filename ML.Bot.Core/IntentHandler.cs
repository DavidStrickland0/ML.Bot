using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using IntentRecognizer;
using QnAProcessor;

namespace ML.Bot.Core
{
    public abstract class IntentHandler : IIntentHandler
    {
        private ConversationState _conversationState;
        private IComponentContext _context;
        private IIntentRecognizerFacade _intentFacade;
        private IQnAMachineLearningFacade _qnaFacade;


        public IntentHandler(
            ConversationState conversationState,
            IComponentContext context
            )
        {
            _conversationState = conversationState;
            _context = context;
            _intentFacade = context.Resolve<IIntentRecognizerFacade>();
        }


        public virtual async Task<(IntentResult, Queue<string>)> HandleMessageAsync(
            IList<string> currentIntentIds,
            ITurnContext turnContext,
            CancellationToken cancellationToken
            )
        {
            var responseQueue = new Queue<string>(); 

            string intentkey = string.Join("-", currentIntentIds);
            var intentStateAccessors = _conversationState.CreateProperty<IntentData>(intentkey);
            IntentData intentData = await intentStateAccessors.GetAsync(turnContext, () => new IntentData(),
                cancellationToken: cancellationToken);


            if (intentData.ActiveIntent != null && intentData.ActiveIntent.Any())
            {
                (IntentResult, Queue<string>) handledResult;
                var activeIntent = intentData.ActiveIntent.Last();

                //Pops intent off the ActiveList
                intentData.ActiveIntent.RemoveAt(intentData.ActiveIntent.Count - 1);

                var _intentHandler = _context.ResolveKeyed<IIntentHandler>(activeIntent);

                var childIntentStack= currentIntentIds.Select(item => item).ToList();
                childIntentStack.Add(activeIntent);
                handledResult = await _intentHandler.HandleMessageAsync(childIntentStack, turnContext, cancellationToken);
                if (handledResult.Item1 == IntentResult.Waiting)
                {
                    // Waiting so intent is still Active;
                    intentData.ActiveIntent.Add(activeIntent);
                    return handledResult;
                }
                else if (handledResult.Item1 == IntentResult.Complete)
                {
                    if (intentData.ActiveIntent.Any())
                    {
                        return await HandleIntentAsync(intentData.ActiveIntent.Last(), intentData.Entities, turnContext,
                            cancellationToken);
                    }
                    else
                    {
                        return handledResult;
                    }
                }
                responseQueue = handledResult.Item2;
            }

            var prediction = await PredictAsync(turnContext.Activity.Text, intentData.Entities);
            
            var responseResult = await HandleIntentAsync(
                prediction.Item1,
                prediction.Item2,
                turnContext,
                cancellationToken);
            if (responseResult.Item1 == IntentResult.Waiting)
            {
                intentData.ActiveIntent.Add(prediction.Item1);
            }
            foreach (var response in responseResult.Item2)
            {
                responseQueue.Enqueue(response);
            }

            return (responseResult.Item1, responseQueue);
        }

        protected abstract Task<(IntentResult, Queue<string>)> HandleIntentAsync(
            string intent,
            Dictionary<string, IList<object>> entities,
            ITurnContext turnContext,
            CancellationToken cancellationToken);

        protected virtual async Task<(string, Dictionary<string,IList<object>>)> PredictAsync(string message, Dictionary<string, IList<object>> entities)
        {
            (string, Dictionary<string, IList<object>>) intent = _intentFacade.Predict(message);
            return intent;
        }
    }
}
