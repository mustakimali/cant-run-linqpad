using System;

namespace CantRunLinqPad.Core.Dumpers
{
    public class ExceptionDumper : IDumper
    {
        public bool CanDump(object obj)
        {
            return obj is Exception;
        }

        public void Dump(object obj)
        {
            var ex = (Exception)obj;

            $"Exception of type {ex.GetType().Name} is thrown".Dump("EXCEPTION");
            ex.GetBaseException().Message.Dump();
            ex.StackTrace.Dump("STACK TRACE:");
        }
    }
}
