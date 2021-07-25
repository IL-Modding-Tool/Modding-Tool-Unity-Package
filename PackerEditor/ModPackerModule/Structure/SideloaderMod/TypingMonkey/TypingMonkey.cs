using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using hooh_ModdingTool.asm_Packer.Utility;
using ModPackerModule.Utility;

namespace ModPackerModule.Structure.SideloaderMod.TypingMonkey
{
    public class TypingMonkey
    {
        private readonly XDocument Document;
        private readonly SideloaderMod Mod;

        private XElement Packer => Document.Root;
        private XElement Bundles => Packer.Element("bundles");
        private XElement Build => Packer.Element("build");
        public string AssetPath;

        public TypingMonkey(in SideloaderMod sideloaderMod, in XDocument document)
        {
            Mod = sideloaderMod;
            Document = document;
            if (Packer == null) throw new Exception("Invalid XML Document");
        }

        public TypingMonkey(string assetPath)
        {
            Document = new XDocument(new XElement("packer", new XElement("bundles"), new XElement("build")));
            AssetPath = assetPath;
        }

        public void WriteModInfo(string key, string value)
        {
            var packer = Packer;
            var element = packer.Element(key);
            if (element == null) packer.Add(new XElement(key, value));
            else element.SetValue(value);
        }

        public void WriteFolderBundle(string from, string autoPath = "prefabs",
            string filter = @".+?\.prefab")
        {
            var element = Bundles.Elements("folder")
                .FirstOrDefault(x => x.Attribute("from")?.Value == from);

            if (element == null)
            {
                Bundles.Add(
                    new XElement("folder",
                        new XAttribute("auto-path", autoPath),
                        new XAttribute("from", from),
                        new XAttribute("filter", filter))
                );
            }
            else
            {
                element.SetAttributeValue("auto-path", autoPath);
                element.SetAttributeValue("from", from);
                element.SetAttributeValue("filter", filter);
            }
        }

        public void WriteMoveFolder(string from, string autoPath = "studiothumb",
            string filter = @".+?\.png")
        {
            var element = Bundles.Elements("move").FirstOrDefault(x => x.Attribute("from")?.Value == from);

            if (element == null)
            {
                Bundles.Add(
                    new XElement("folder",
                        new XAttribute("auto-path", autoPath),
                        new XAttribute("from", from),
                        new XAttribute("filter", filter))
                );
            }
            else
            {
                element.SetAttributeValue("auto-path", autoPath);
                element.SetAttributeValue("from", from);
                element.SetAttributeValue("filter", filter);
            }
        }


        private XElement GetBuildTarget(string type)
        {
            var list = Build.Element(type);
            if (list != null) return list;
            list = new XElement("list", new XAttribute("type", type));
            Build.Add(list);
            return list;
        }


        public void WriteCategory(bool big, int id, string name, int bigid = -1)
        {
            var listType = big ? "bigcategory" : "midcategory";
            var listRoot = GetBuildTarget(listType);
            var strID = id.ToString();
            var strBigID = bigid.ToString();

            var element = listRoot.Elements("item").FirstOrDefault(x => x.Attribute("id")?.Value == strID);
            if (element == null)
            {
                listRoot.Add(big
                    ? new XElement("item", new XAttribute("id", strID), new XAttribute("name", name))
                    : new XElement("item", new XAttribute("big-category", strBigID), new XAttribute("id", strID),
                        new XAttribute("name", name)));
            }
            else
            {
                element.SetAttributeValue("id", strID);
                element.SetAttributeValue("name", name);
                if (!big) element.SetAttributeValue("big-category", strBigID);
            }
        }

        public void WriteStudioItem(string asset, string name, int bigCategory, int midCategory)
        {
            var listRoot = GetBuildTarget("studioitem");
            var element = listRoot.Elements("item").FirstOrDefault(x => x.Attribute("asset")?.Value == asset);

            if (element == null)
            {
                listRoot.Add(
                    new XElement("item",
                        new XAttribute("big-category", bigCategory),
                        new XAttribute("mid-category", midCategory),
                        new XAttribute("asset", asset),
                        new XAttribute("name", name)
                    )
                );
            }
            else
            {
                element.SetAttributeValue("big-category", bigCategory);
                element.SetAttributeValue("mid-category", midCategory);
                element.SetAttributeValue("asset", asset);
                element.SetAttributeValue("name", name);
            }
        }

        public void UpdateLastBuild()
        {
            var last = Packer.Element("last-build");
            var time = DateTime.Now.ToLocalTime().ToString(CultureInfo.CurrentCulture);
            if (last == null)
            {
                var newElement = new XElement("last-build");
                newElement.SetAttributeValue("time", time);
                newElement.SetAttributeValue("version", Mod.MainData.version);
                Packer.Add(newElement);
            }
            else
            {
                last.SetAttributeValue("time", time);
                last.SetAttributeValue("version", Mod.MainData.version);
            }
        }

        public void Update()
        {
            Document.NiceSave(Path.Combine(Mod.AssetDirectory, Mod.FileName).ToUnixPath());
        }
        public void Save()
        {
            Document.NiceSave(AssetPath);
        }
    }
}