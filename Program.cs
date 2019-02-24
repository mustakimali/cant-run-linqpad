using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using CantRunLinqPad.Core;
using static System.Console;
using static CantRunLinqPad.Core.Util;

#pragma warning disable CS1998
namespace CantRunLinqPad
{
    class Program
    {
        static async Task Main(string[] args) => await Init(Code);
        
        private static async Task Code()
        {
            //
            // Write your code here
            //
            "Your code is running and waiting for any changes to Programs.cs".Dump();
            
        }

        

        
    }
}
