using static System.Console;
using Newtonsoft.Json;

namespace CantRunLinqPad.Core.Dumpers
{
    public class EverythingElseDumper : IDumper
    {
        public bool CanDump(object obj)
        {
            return true;
        }

        public void Dump(object obj)
        {
            WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}
