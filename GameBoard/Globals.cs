﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard
{
    public static class Globals
    {
        public static ConcurrentDictionary<string, string> consoleData = new ConcurrentDictionary<string, string>();
    }
}
