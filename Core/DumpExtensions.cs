using CantRunLinqPad.Core;
using CantRunLinqPad.Core.Dumpers;
using System.Collections.Generic;
using static System.Console;

namespace CantRunLinqPad.Core
{
    public static class DumpExtensions
    {
        private static readonly IDumper[] _dumpers;

        static DumpExtensions()
        {
            _dumpers = new IDumper[]
            {
                new ExceptionDumper(),
                new BasicTypesDumper(),
                new EverythingElseDumper()
            };
        }
        public static T Dump<T>(this T obj, string title = null, int indent = 0)
        {
            if (!string.IsNullOrEmpty(title))
            {
                WriteLine(title);
            }
            
            if(obj == null)
            {
                "{NULL}".Dump();
                return obj;
            }

            foreach (var dumper in _dumpers)
            {
                if (dumper.CanDump(obj))
                {
                    if(indent > 0)
                    {
                        Write(new string('\t', indent));
                    }
                    dumper.Dump(obj);
                    break;
                }
            }

            return obj;
        }
    }
}
