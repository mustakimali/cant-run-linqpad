using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using static System.Console;

namespace CantRunLinqPad.Core
{
    public static class Util
    {
        public static async Task Init()
        {
            Clear();
            await InstallNugetPackages();
        }

        private static async Task InstallNugetPackages()
        {
            // Get list of nuget packages to install
            var csFile = Path.Combine(Assembly.GetExecutingAssembly().Location, "../../../../Program.cs");
            if(!File.Exists(csFile))
            {
                Error($"Can't locate the file: {csFile}");
            }

            // to be implemented
            await Task.CompletedTask;
        }

        private static void Error(string message)
        {
            throw new ApplicationException($"#fatal: {message}");
        }
    }
}