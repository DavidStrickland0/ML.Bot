using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using IntentRecognizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using QnAProcessor;

namespace ML.Bot.Bots
{
    public class BotActivityHandler : ActivityHandler
    {
        private readonly IConversationManager _conversationManager;
        private readonly ConversationState _conversationState;

        public BotActivityHandler(
            IConversationManager conversationManager,
            ConversationState conversationState)
        {
            _conversationManager = conversationManager;
            _conversationState = conversationState;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationStateAccessors = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData(), cancellationToken: cancellationToken);
           
                var response = await _conversationManager.ProcessMessageAsync(conversationData,turnContext.Activity.Text, cancellationToken);

            while (response.Count > 0)
            {
                var responseText = response.Dequeue();
                await turnContext.SendActivityAsync(MessageFactory.Text(responseText, responseText), cancellationToken);
            }

            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
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
