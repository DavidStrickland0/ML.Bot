using System;
using System.Collections.Generic;
using System.Text;

namespace ML.Bot.Core
{
    public class IntentData
    {
        public List<string> ActiveIntent { get; internal set; } = new List<string>();
        public Dictionary<string,IList<object>> Entities { get; internal set; } = new Dictionary<string, IList<object>>();
    }
}
