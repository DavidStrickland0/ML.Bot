using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace ML.Bot
{
    public interface IConversationManager
    {
        Task<Queue<string>> ProcessMessageAsync(
            ConversationData conversationData,
            string message,
            CancellationToken cancellationToken);
    }
}
