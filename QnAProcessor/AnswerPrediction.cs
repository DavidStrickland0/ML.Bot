using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace QnAProcessor
{
    public class AnswerPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Answer { get; set; }
    }
}
