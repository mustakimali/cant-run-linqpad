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

        private static readonly Regex ParseNugetCommand = new Regex(@"(Install-Package |dotnet add package )([^ ]*).*[Vv]ersion (.*)$", RegexOptions.Compiled);
        private static readonly string Line = new string('-', 70);

        public static async Task Init(Func<Task> entryPoint)
        {
            Clear();
            await InstallNugetPackages();

            try
            {
                await entryPoint();
            }
            catch (Exception ex)
            {
                WriteLine(Line);
                ex.Dump();
                WriteLine(Line);
            }
        }

        private static async Task InstallNugetPackages()
        {
            // Get list of nuget packages to install
            var codeFile = Path.Combine(Assembly.GetExecutingAssembly().Location, "../../../../Program.cs".Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(codeFile))
            {
                Error($"Can't locate the file: {codeFile}");
            }

            var nugetPackages = await ParseNugetPackagesDeclarations(codeFile);
            const string csprojPath = "cant-run-linqpad.csproj";

            using(var updater = new NugetReferenceUpdater(csprojPath))
            {
                updater.AddOrUpdateReferences(nugetPackages);
            }
        }

        private static async Task<List<NugetReference>> ParseNugetPackagesDeclarations(string codeFile)
        {
            var nugetPackages = new List<NugetReference>();

            foreach (var line in File.ReadAllLines(codeFile))
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

        private static void Error(string message)
        {
            throw new ApplicationException($"#fatal: {message}");
        }
    }
}