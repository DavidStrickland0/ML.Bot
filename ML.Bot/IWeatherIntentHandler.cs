#region Copyright

// ==================================================================================================
//   This file is part of the Navitaire Digital Experience Platform.
//   Copyright © Navitaire LLC  An Amadeus company. All rights reserved.
// ==================================================================================================

#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IntentRecognizer;

namespace ML.Bot
{
    internal interface IWeatherIntentHandler
    {
        Task<(IntentResult, string)> HandleAsync(string message);
    }
}