﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Bot.Builder;
using ML.Bot.Core;

namespace ML.Bot.IntentHandlers
{
    class TimeIntentHandler : IntentHandler
    {
        public TimeIntentHandler(ConversationState conversationState,
            IComponentContext context) : base(conversationState, context)
        {
        }

        protected override Task<(IntentResult, Queue<string>)> HandleIntentAsync(string intent, Dictionary<string, IList<object>> entities, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var que = new Queue<string>();
            que.Enqueue(DateTime.Now.ToShortTimeString());
            return Task.FromResult((IntentResult.Complete, que));
        }
    }
}
