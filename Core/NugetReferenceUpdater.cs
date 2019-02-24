using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace CantRunLinqPad.Core
{
    internal struct NugetReference : IEquatable<NugetReference>
    {
        public NugetReference(string name, string version)
        {
            PackageName = name;
            Version = version;
        }

        public string PackageName { get; set; }
        public string Version { get; set; }

        public bool Equals(NugetReference other)
        {
            return other.PackageName == PackageName && other.Version == Version;
        }
    }

    internal class NugetReferenceUpdater : IDisposable
    {
        private readonly XmlDocument _csprojXmlDoc;
        private readonly string _csprojPath;

        public NugetReferenceUpdater(string csprojPath)
        {
            if (!File.Exists(csprojPath))
            {
                throw new ApplicationException($"The file can't be found: {csprojPath}");
            }

            _csprojXmlDoc = new XmlDocument();
            _csprojXmlDoc.Load(csprojPath);
            this._csprojPath = csprojPath;
        }

        public void AddOrUpdateReferences(List<NugetReference> references)
        {
            var itemGroup = _csprojXmlDoc.SelectSingleNode("//Project//ItemGroup");
            if (itemGroup == null)
            {
                throw new ApplicationException("The csproj does not have an ItemGroup element. Clone cant-run-linqpad again.");
            }

            var refsToAdd = references
                                .GroupBy(r => r.PackageName)
                                .Select(g => g.OrderBy(o => o.Version).Last())
                                .ToArray();
            var newRefs = new HashSet<NugetReference>(refsToAdd);
            var existingRefs = GetExistingReferences(itemGroup);
            foreach (var exRef in existingRefs)
            {
                if (newRefs.Contains(exRef))
                {
                    newRefs.Remove(exRef);
                }
                else
                {
                    // Changes detected
                    RefreshReferences();
                    return;
                }
            }

            if (newRefs.Any())
            {
                RefreshReferences();
            }

            void RefreshReferences()
            {
                ClearExistingRefs(itemGroup);
                AddReferenceNodes(_csprojXmlDoc, itemGroup, refsToAdd);
                SaveUpdateCsprojFile(_csprojXmlDoc, _csprojPath);
            }
        }

        private void ClearExistingRefs(XmlNode itemGroupNode)
        {
            var existingRefs = itemGroupNode.SelectNodes("PackageReference");

            foreach (XmlNode item in existingRefs)
            {
                itemGroupNode.RemoveChild(item);
            }
        }

        private void AddReferenceNodes(XmlDocument doc, XmlNode itemGroupNode, IEnumerable<NugetReference> refs)
        {
            foreach (var pkg in refs)
            {
                var tmpDoc = new XmlDocument();
                tmpDoc.LoadXml($"<PackageReference Include=\"{pkg.PackageName}\" Version=\"{pkg.Version}\" />");
                itemGroupNode.AppendChild(doc.ImportNode(tmpDoc.FirstChild, false));

                $"Detected NuGet Package: {pkg.PackageName}, v{pkg.Version}".Dump();
            }
        }

        private void SaveUpdateCsprojFile(XmlDocument doc, string path)
        {
            File.Copy(path, path + ".bak", true);
            doc.Save(path);
        }

        private IEnumerable<NugetReference> GetExistingReferences(XmlNode itemGroupNode)
        {
            var existingRefs = itemGroupNode.SelectNodes("PackageReference");
            foreach (XmlNode existingRef in existingRefs)
            {
                var name = existingRef.Attributes["Include"].Value;
                var version = existingRef.Attributes["Version"].Value;

                yield return new NugetReference(name, version);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}