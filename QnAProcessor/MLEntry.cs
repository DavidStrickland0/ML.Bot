using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace QnAProcessor
{
    class MLEntry
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string[] PreviousIntents { get; set; } = new[] {"", "", "", "", ""};
    }
}
