using System;
using System.Collections.Generic;
using System.Text;

namespace ContextualizedIntentRecognizer
{
    public class MLEntry
    {
        public string[] PreviousIntents { get; set; }
        public string Text { get; set; }
        public string Intent { get; set; }
    }
}
