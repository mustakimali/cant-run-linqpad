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
        private static readonly Regex ParseNugetCommandWithoutVersion = new Regex(@"(Install-Package |dotnet add package )(.*)$", RegexOptions.Compiled);
        private static readonly string Line = new string('-', 70);

        public static async Task Init(Func<Task> entryPoint)
        {
            Clear();
            await InstallNugetPackages();

            try
            {
                $"---  PROGRAM START  ---".Dump();
                await entryPoint();
            }
            catch (Exception ex)
            {
                WriteLine(Line);
                ex.Dump();
                WriteLine(Line);
            }
            finally
            {
                $"--- PROGRAM FINISHED ---".Dump();
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
                        //
                        // Get the latest version using
                        // nuget package if not specified
                        //
                        if (string.IsNullOrEmpty(nugetPackage.Version))
                        {
                            nugetPackage.Version = await GetLatestVersion(nugetPackage.PackageName);
                        }

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

            // Nuget Package reference without version
            match = ParseNugetCommandWithoutVersion.Match(command);
            if(match.Success && match.Groups.Count >= 2)
            {
                var packageName = match.Groups[2].Value;
                reference = new NugetReference
                {
                    PackageName = packageName,
                    Version = string.Empty
                };
                return true;
            }

            reference = default;
            return false;
        }

        private static async Task<string> GetLatestVersion(string packageName)
        {
            var url = $"https://api.nuget.org/v3/registration3/{packageName.ToLower()}/index.json";
            using var client = new System.Net.Http.HttpClient();
            string json;
            try
            {
                json = await client.GetStringAsync(url);
            }
            catch (System.Exception e)
            {
                $"Can not determine latest version of {packageName}, Error: {e.GetBaseException().Message}".Dump();
                return string.Empty;
            }

            var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(json, new
            {
                items = new[] {
                    new {
                        upper = string.Empty
                    }
                }
            });

            foreach (var item in jsonObject.items)
            {
                if(!item.upper.Contains("-"))
                {
                    $"{packageName} -> {item.upper}".Dump();
                    return item.upper;
                }
            }

            return string.Empty;
        }

        private static void Error(string message)
        {
            throw new ApplicationException($"#fatal: {message}");
        }
    }
}