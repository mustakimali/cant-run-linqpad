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
            try
            {
                ConsoleDump.Extensions.DumpObject(obj);
            }
            catch (System.Exception)
            {
                WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Error  = (s, args) => {
                        args.ErrorContext.Handled = true;
                    }
                }));
            }
        }
    }
}
