using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace ContextualizedIntentRecognizer
{
    public class IntentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Intent { get; set; }

        [ColumnName("Score")]
        public Single[] Score { get; set; }
    }
}
