﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ML.Bot
{
    public class ConversationData
    {
        public List<string> ActiveIntent { get; internal set; } = new List<string>();
    }
}
