using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace QnAProcessor
{
    public class QnAEntry
    {
        [LoadColumn(0)]
        public string Question { get; set; }
        [LoadColumn(1)]
        public string Answer { get; set; }
        //public string Source { get; set; }
        //public string Metadata { get; set; }
        //public string IsContextOnly { get; set; }
        //public string QnaId { get; set; }
    }
}
