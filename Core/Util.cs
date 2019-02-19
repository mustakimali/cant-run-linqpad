using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Console;

namespace CantRunLinqPad.Core
{
    public static class Util
    {
        private const string NugetReferenceLineStartWith = "#r ";
        private const string CSharpCommentLineStartWith = "//";

        private static readonly Regex ParseNugetCommand = new Regex(@"(Install-Package |dotnet add package )([^ ]*).*[Vv]ersion ([0-9.]*]*)", RegexOptions.Compiled);

        public static async Task Init()
        {
            Clear();
            await InstallNugetPackages();
        }

        private static async Task InstallNugetPackages()
        {
            // Get list of nuget packages to install
            var codeFile = Path.Combine(Assembly.GetExecutingAssembly().Location, "../../../../Program.cs".Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(codeFile))
            {
                Error($"Can't locate the file: {codeFile}");
            }

            // to be implemented
            var nugetPackages = await ParseNugetPackages(codeFile);

            foreach (var pkg in nugetPackages)
            {
                WriteLine($"Detected NuGet Package: {pkg.PackageName}, v{pkg.Version}");
            }
        }

        private static async Task<List<NugetReference>> ParseNugetPackages(string codeFile)
        {
            var nugetPackages = new List<NugetReference>();

            foreach (var line in await File.ReadAllLinesAsync(codeFile))
            {
                var lineTrimmed = line.Trim(' ', '\t');

                if (lineTrimmed.StartsWith(CSharpCommentLineStartWith))
                {
                    if (TryParseNugetCliCommand(lineTrimmed, out var nugetPackage))
                    {
                        nugetPackages.Add(nugetPackage);
                    }
                }
                else if (lineTrimmed.StartsWith(NugetReferenceLineStartWith))
                {
                    // #r <package name> [version]
                    // ToDo: Implement
                }
            }

            return nugetPackages;
        }

        private static bool TryParseNugetCliCommand(string command, out NugetReference reference)
        {
            var match = ParseNugetCommand.Match(command);
            if (match.Success && match.Groups.Count >= 3)
            {
                reference = new NugetReference
                {
                    PackageName = match.Groups[2].Value,
                    Version = match.Groups[3].Value
                };
                return true;
            }

            reference = default;
            return false;
        }

        private struct NugetReference
        {
            public string PackageName { get; set; }
            public string Version { get; set; }
        }


        private static void Error(string message)
        {
            throw new ApplicationException($"#fatal: {message}");
        }
    }
}