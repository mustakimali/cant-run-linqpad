using static System.Console;

namespace CantRunLinqPad.Core.Dumpers
{
    public class BasicTypesDumper : IDumper
    {
        public bool CanDump(object obj)
        {
            switch (obj.GetType().Name)
            {
                case nameof(System.String):
                case nameof(System.Int32):
                case nameof(System.Int64):
                case nameof(System.Boolean):
                    return true;

                default:
                    return false;
            }
        }

        public void Dump(object obj)
        {
            WriteLine(obj);
        }
    }
}
