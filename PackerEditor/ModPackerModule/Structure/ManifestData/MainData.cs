using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using hooh_ModdingTool.asm_Packer.Utility;
using ModPackerModule.Structure.SideloaderMod;
using ModPackerModule.Utility;
using MyBox;
using static ModPackerModule.Structure.SideloaderMod.Issue.IssueLevel;
using static ModPackerModule.Structure.SideloaderMod.Issue.IssueType;

namespace ModPackerModule.Structure.Classes.ManifestData
{
    [Serializable]
    public class MainData : IManifestData
    {
        public string author;
        public string description;
        public TargetGame Game = TargetGame.HS2;
        public string guid;
        public string name;
        public string uniqueId;
        public string version;

        // Separator for generic use
        public string Separator => (uniqueId.Length > 0 ? uniqueId : guid.SanitizeNonCharacters()).ToLower();
        public string SafeName => guid.SanitizeNonCharacters().ToLower();

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public (bool, string) IsValid()
        {
            if (guid.IsNullOrEmpty()) return (false, "Invalid GUID");
            if (name.IsNullOrEmpty()) return (false, "Invalid Mod Name");
            if (version.IsNullOrEmpty()) return (false, "Invalid Version");
            return (true, null);
        }

        public void ParseData(in SideloaderMod.SideloaderMod modObject, in XElement modDocument)
        {
            guid = modDocument.Element("guid")?.Value.Trim();
            name = modDocument.Element("name")?.Value.Trim();
            version = modDocument.Elem("version", "1.0.0").Trim();
            author = modDocument.Elem("author", "Anonymous").Trim();
            description = modDocument.Elem("description", "No Description Provided - Packed with Modding tool.").Trim();
            uniqueId = modDocument.Elem("mod-id", "").Trim();

            if (guid.IsNullOrEmpty())
                modObject.Issues.Add(new Issue(Error, MainInfo,
                    "[packer > guid] Invalid Mod Info: GUID cannot be empty."));
            if (name.IsNullOrEmpty())
                modObject.Issues.Add(new Issue(Error, MainInfo,
                    "[packer > name] Invalid Mod Info: Name cannot be empty."));
            if (version.IsNullOrEmpty())
                modObject.Issues.Add(new Issue(Error, MainInfo,
                    "[packer > version] Invalid Mod Info: Version cannot be empty."));
            if (author.IsNullOrEmpty())
                modObject.Issues.Add(new Issue(Error, MainInfo,
                    "[packer > author] Invalid Mod Info: Author cannot be empty."));
            if (modObject.Issues.Count > 0) throw new InvalidXMLValue();
        }

        public void SaveData(ref XDocument document)
        {
            var root = document.Root;
            if (ReferenceEquals(null, root)) return;

            root.Add(new XElement("guid", guid));
            root.Add(new XElement("name", name));
            root.Add(new XElement("version", version));
            root.Add(new XElement("author", author));
            root.Add(new XElement("description", description));
        }
    }
}