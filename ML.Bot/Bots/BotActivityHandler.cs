using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Google.Protobuf.WellKnownTypes;
using IntentRecognizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using ML.Bot.Core;
using QnAProcessor;

namespace ML.Bot.Bots
{
    public class BotActivityHandler : ActivityHandler
    {
        private readonly ConversationState _conversationState;
        private readonly IComponentContext _context;

        public BotActivityHandler(
            ConversationState conversationState,
            IComponentContext context)
        {
            _conversationState = conversationState;
            _context = context;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationStateAccessors = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData(), cancellationToken: cancellationToken);
           
            var response = await processMessageAsync(conversationData,turnContext, cancellationToken);

            while (response.Count > 0)
            {
                var responseText = response.Dequeue();
                await turnContext.SendActivityAsync(MessageFactory.Text(responseText, responseText), cancellationToken);
            }

            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        private async Task<Queue<string>> processMessageAsync(ConversationData conversationData, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            (IntentResult, Queue<string>) handledResult;
            if (conversationData.ActiveIntent != null && conversationData.ActiveIntent.Any())
            {
                var activeIntent = conversationData.ActiveIntent.LastOrDefault();
                //Pops intent off the List
                conversationData.ActiveIntent.RemoveAt(conversationData.ActiveIntent.Count - 1);
                var _IntentHandler = _context
                    .ResolveKeyed<IIntentHandler>(activeIntent);
                handledResult = await _IntentHandler.HandleMessageAsync(new List<string>(){activeIntent},turnContext,cancellationToken);
                if (handledResult.Item1 == IntentResult.Waiting)
                {
                    conversationData.ActiveIntent.Add(_IntentHandler.GetType().Name);
                }
            }
            else
            {
                var IntentHandler = new RootIntentHandler(_conversationState, _context);
                handledResult = await IntentHandler.HandleMessageAsync(new List<string>(){nameof(RootIntentHandler) },turnContext,cancellationToken);
            }
            return handledResult.Item2;
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
