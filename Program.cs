using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using CantRunLinqPad.Core;
using static System.Console;
using static CantRunLinqPad.Core.Util;

namespace CantRunLinqPad
{
    class Program
    {
        static async Task Main(string[] args) => await Init(Code);
        
        // Write your code here
        private static async Task Code()
        {
            "Your code is running and waiting for any changes to Programs.cs".Dump();
        }
    }
}
