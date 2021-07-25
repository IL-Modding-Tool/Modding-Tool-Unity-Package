using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using hooh_ModdingTool.asm_Packer.Utility;
using LoremNET;
using ModPackerModule.Utility;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace ModPackerModule.Structure.SideloaderMod
{
    public partial class SideloaderMod
    {
        private const string TemplateModName = "Template Mod";
        private const string TemplateVersion = "0.0.1";
        private const string TemplateAuthor = "Anonymous";

        protected XDocument InputDocumentObject;
        protected XDocument OutputDocumentObject;

        private static string Word()
        {
            return Lorem.Words(1, 1, false).ToLower();
        }

        private static string GetRandomName()
        {
            return $"{Word()}.{Word()}.{Word()}";
        }

        private static string GetTemplateDescription(string type)
        {
            return $"Template {type} Mod - Made with hooh's Modding Tool";
        }

        [MenuItem("Assets/Mod XML Templates/Basic Mod")]
        public static void MakeTemplate(string guid = "", string name = "", string author = "", string desc = "",
            string path = "", bool isAbsolute = false)
        {
            string assetPath;
            if (path.IsNullOrEmpty())
                assetPath = Path.Combine(Directory.GetCurrentDirectory(), PathUtils.GetProjectPath(), "mod.xml")
                    .ToUnixPath();
            else
                assetPath = isAbsolute
                    ? path
                    : Path.Combine(Directory.GetCurrentDirectory(), path, "mod.xml").ToUnixPath();

            var document = new XDocument();
            document.Add(new XElement("packer",
                new XElement("guid", guid.IsNullOrEmpty() ? GetRandomName() : guid),
                new XElement("name", name.IsNullOrEmpty() ? TemplateModName : name),
                new XElement("version", TemplateVersion),
                new XElement("author", author.IsNullOrEmpty() ? TemplateAuthor : author),
                new XElement("description", desc.IsNullOrEmpty() ? GetTemplateDescription("Generic") : desc),
                new XElement("bundles",
                    new XElement("folder", new XAttribute("auto-path", "prefabs"), new XAttribute("from", "output"),
                        new XAttribute("filter", @".+?\.prefab"))
                ),
                new XElement("build")
            ));
            document.NiceSave(assetPath);
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Mod XML Templates/Studio Map and Items")]
        public static void MakeStudioModTemplate()
        {
            var assetPath = Path.Combine(Directory.GetCurrentDirectory(), PathUtils.GetProjectPath(), "mod.xml")
                .ToUnixPath();
            var document = new XDocument();
            document.Add(new XElement("packer",
                new XElement("guid", GetRandomName()),
                new XElement("name", TemplateModName),
                new XElement("version", TemplateVersion),
                new XElement("author", TemplateAuthor),
                new XElement("description", GetTemplateDescription("Studio Maps and Item")),
                new XElement("bundles",
                    new XElement("folder", new XAttribute("auto-path", "prefabs"), new XAttribute("from", "prefabs"),
                        new XAttribute("filter", @".+?\.prefab")),
                    new XElement("move", new XAttribute("auto-path", "studiothumb"), new XAttribute("from", "thumbs"),
                        new XAttribute("filter", @".+?\.png"))
                ),
                new XElement("build",
                    new XElement("list", new XAttribute("type", "bigcategory"),
                        new XElement("item", new XAttribute("id", "2020"),
                            new XAttribute("name", "Example Big Category"))
                    ),
                    new XElement("list", new XAttribute("type", "midcategory"),
                        new XElement("item", new XAttribute("big-category", "2020"), new XAttribute("id", "1"),
                            new XAttribute("name", "Example Mid Category"))
                    ),
                    new XElement("list", new XAttribute("type", "studioitem"), "")
                )
            ));
            document.NiceSave(assetPath);
            AssetDatabase.Refresh();
        }

        protected void ChangeXMLData(string key, string value)
        {
            var xElement = InputDocumentObject.Root?.Element(key);
            if (xElement == null) return;
            xElement.Value = value;
        }

        protected void UpdateBuildInfo()
        {
            var xElement = InputDocumentObject.Root;
            if (xElement == null) return;
            var last = xElement.Element("last-build");
            var time = DateTime.Now.ToLocalTime().ToString(CultureInfo.CurrentCulture);
            if (last == null)
            {
                var newElement = new XElement("last-build");
                newElement.SetAttributeValue("time", time);
                newElement.SetAttributeValue("version", MainData.version);
                xElement.Add(newElement);
            }
            else
            {
                last.SetAttributeValue("time", time);
                last.SetAttributeValue("version", MainData.version);
            }

            Save(true);
        }

        private bool TryGetListElement(string type, out XElement output)
        {
            output = default;
            var buildElement = BuildTargetXElement;
            if (buildElement == null)
            {
                Debug.LogError("The xml file has no <build> node.");
                return false;
            }

            if (buildElement.Value != "") buildElement.Value = "";
            var itemList = buildElement.Elements()
                .FirstOrDefault(element =>
                    element.Name.LocalName == "list" &&
                    element.Attribute("type")?.Value == type);
            output = itemList;
            if (!ReferenceEquals(itemList, null)) return true;
            itemList = new XElement("list", new XAttribute("type", type));
            buildElement.Add(itemList);
            return true;
        }

        public void UpsertClothing(string type)
        {
            // clothing has numerous variants.
        }

        public bool ValidatePrefabWriter(in GameObject[] gameObjects)
        {
            if (gameObjects.Length <= 0)
            {
                Debug.LogError("You must select at least one valid prefabs from the project window.");
                return false;
            }

            Array.Sort(gameObjects, (x, y) => string.Compare(x.name, y.name, StringComparison.Ordinal));
            return true;
        }

        public void CommonWriterExceptionHandler(Exception e)
        {
            Debug.LogError(e);
            EditorApplication.Beep();
            EditorUtility.DisplayDialog("Error",
                "An error has occured while writing selected items into the sxml file.\n" +
                "Please file a report to the Modding Tool Repository if you think this is a bug.", "Hmmm");
        }


        public Dictionary<string, XElement> GetItemsByAsset(in XElement root)
        {
            return root.Elements("item")
                .Where(x => x.Attribute("object") != null)
                .ToDictionary(x => x.Attr("object"), x => x);
        }

        public void UpsertStudioItems(GameObject[] gameObjects, int bigCategory = 0, int midCategory = 0)
        {
            // I really don't like the performance here, There must be the way to unfuck this mess.
            try
            {
                if (!ValidatePrefabWriter(in gameObjects)) return;
                if (!TryGetListElement("studioitem", out var studioItemList)) return;

                var existingItems = GetItemsByAsset(studioItemList);
                foreach (var gameObject in gameObjects)
                    WriteStudioItem(in studioItemList, in existingItems, gameObject, bigCategory, midCategory);

                Debug.Log($"Successfully wrote {gameObjects.Length} item(s) to the this mod xml file.");
                EditorApplication.Beep();
            }
            catch (Exception e)
            {
                CommonWriterExceptionHandler(e);
            }
        }

        public void Save(bool sayNoToExtension = false)
        {
            InputDocumentObject.NiceSave(Path.Combine(AssetDirectory, FileName + (sayNoToExtension ? "" : ".xml"))
                .ToUnixPath());
        }
    }
}