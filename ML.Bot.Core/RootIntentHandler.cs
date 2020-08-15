using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Bot.Builder;
using QnAProcessor;

namespace ML.Bot.Core
{
    public class RootIntentHandler: IntentHandler
    {
        private IComponentContext _context;
        public RootIntentHandler(
            ConversationState conversationState,
            IComponentContext context
        ):base(conversationState, context)
        {
            _context = context;
        }

        protected async override Task<(IntentResult, Queue<string>)> HandleIntentAsync(
            string intent,
            Dictionary<string, IList<object>> intentData, 
            ITurnContext turnContext,
            CancellationToken cancellationToken
            )
        {
            if(intent != "None")
            {
                var result = await _context.ResolveKeyed<IIntentHandler>(intent)
                .HandleMessageAsync(
                new List<string>(){ nameof(RootIntentHandler), intent },
                turnContext,
                cancellationToken
                );
                if (result.Item1 != IntentResult.None)
                {
                    return result;
                }
                else
                {
                    return processAsQna(turnContext);
                }
            }
            else
            {
                return processAsQna(turnContext);
            }
        }

        private (IntentResult, Queue<string>) processAsQna(ITurnContext turnContext)
        {
            var _qnaFacade = _context.Resolve<IQnAMachineLearningFacade>();
            var qnaPrediction = _qnaFacade.Predict(turnContext.Activity.Text);
            var que = new Queue<string>();
            que.Enqueue(qnaPrediction);
            return (IntentResult.Complete, que);
        }
    }
}
