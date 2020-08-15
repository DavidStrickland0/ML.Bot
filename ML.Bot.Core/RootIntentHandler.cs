﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Bot.Builder;

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
            var result = await _context.ResolveKeyed<IIntentHandler>(intent)
                .HandleMessageAsync(
                new List<string>(){ nameof(RootIntentHandler), intent },
                turnContext,
                cancellationToken
                );
            return result;
        }
    }
}
