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
                new BasicTypesDumper(),
                new EverythingElseDumper()
            };
        }
        public static T Dump<T>(this T obj, string title = null)
        {
            if (!string.IsNullOrEmpty(title))
            {
                WriteLine(title);
            }
            foreach (var dumper in _dumpers)
            {
                if (dumper.CanDump(obj))
                {
                    dumper.Dump(obj);
                    break;
                }
            }

            return obj;
        }
    }
}
