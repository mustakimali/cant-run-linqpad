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

            $"EXCEPTION OF TYPE {ex.GetType().Name} IS THROWN".Dump(); 
            ex.GetBaseException().Message.Dump(indent: 1);
            ex.StackTrace.Dump("STACK TRACE:");
        }
    }
}
