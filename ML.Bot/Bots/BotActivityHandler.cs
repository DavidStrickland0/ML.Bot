// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextualizedIntentRecognizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using QnAProcessor;

namespace ML.Bot.Bots
{
    public class BotActivityHandler : ActivityHandler
    {
        private IQnAMachineLearningFacade _qnaFacade;
        private IRecognizerMachineLearningFacade _intentFacade;

        public BotActivityHandler(IQnAMachineLearningFacade qnaFacade, IRecognizerMachineLearningFacade intentFacade)
        {
            _qnaFacade = qnaFacade;
            _intentFacade = intentFacade;

        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = _qnaFacade.Predict(turnContext.Activity.Text);
            //var intent = _intentFacade.Predict(turnContext.Activity.Text,null);

            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
