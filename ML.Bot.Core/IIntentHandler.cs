#region Copyright

// ==================================================================================================
//   This file is part of the Navitaire Digital Experience Platform.
//   Copyright © Navitaire LLC  An Amadeus company. All rights reserved.
// ==================================================================================================

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace ML.Bot
{
    public interface IIntentHandler
    {
        Task<(IntentResult, Queue<string>)> HandleMessageAsync(
            IList<string> activeIntents,
            ITurnContext turnContext,
            CancellationToken cancellationToken
        );
    }
}