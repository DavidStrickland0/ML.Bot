using System;
using Microsoft.ML.Data;

namespace IntentRecognizer
{
    internal class IntentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Intent { get; set; }

        [ColumnName("Score")]
        public Single[] Score { get; set; }
    }
}