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
        private const string TemplateFileName = "mod.sxml";

        protected XDocument InputDocumentObject;
        protected XDocument OutputDocumentObject;

        private static string Word => Lorem.Words(1, 1, false).ToLower();

        private static string RandomName => $"{Word}.{Word}.{Word}";

        private static string GetTemplateDescription(string type)
        {
            return $"Template {type} Mod - Made with hooh's Modding Tool";
        }

        private static string WriteAssetPath(string path = "", bool isAbsolute = false)
        {
            if (path.IsNullOrEmpty())
                return Path.Combine(Directory.GetCurrentDirectory(), PathUtils.GetProjectPath(), TemplateFileName)
                    .ToUnixPath();
            return isAbsolute
                ? path
                : Path.Combine(Directory.GetCurrentDirectory(), path, TemplateFileName).ToUnixPath();
        }

        private static void CreateBaseXML(out XDocument root, out XElement packer, out XElement bundles,
            out XElement build)
        {
            root = new XDocument();
            bundles = new XElement("bundles");
            build = new XElement("build");
            packer = new XElement("packer", bundles, build);
            root.Add(packer);
        }

        [MenuItem("Assets/Mod XML Templates/Basic Mod")]
        public static void MakeTemplate(string guid = "", string name = "", string author = "", string desc = "",
            string path = "", bool isAbsolute = false)
        {
            var assetPath = WriteAssetPath(path, isAbsolute);
            CreateBaseXML(out var document, out var bundles, out var build, out var packer);

            WriteModInfo(packer, "guid", guid.IsNullOrEmpty() ? RandomName : guid);
            WriteModInfo(packer, "name", name.IsNullOrEmpty() ? TemplateModName : name);
            WriteModInfo(packer, "version", TemplateVersion);
            WriteModInfo(packer, "author", author.IsNullOrEmpty() ? TemplateAuthor : author);
            WriteModInfo(packer, "description", desc.IsNullOrEmpty() ? GetTemplateDescription("Generic") : desc);

            WriteFolderBundle(bundles, "output");

            document.NiceSave(assetPath);
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Mod XML Templates/Studio Map and Items")]
        public static void MakeStudioModTemplate()
        {
            var assetPath = WriteAssetPath();
            CreateBaseXML(out var document, out var bundles, out var build, out var packer);

            WriteModInfo(packer, "guid", RandomName);
            WriteModInfo(packer, "name", TemplateModName);
            WriteModInfo(packer, "version", TemplateVersion);
            WriteModInfo(packer, "author", TemplateAuthor);
            WriteModInfo(packer, "description", GetTemplateDescription("Studio Maps and Item"));

            WriteFolderBundle(bundles, "prefabs");
            WriteMoveFolder(bundles, "move");

            WriteCategory(build, true, 2020, "Example Big Category");
            WriteCategory(build, false, 2020, "Example Mid Category", 1);
            WriteStudioItem(build, "example", "Example Item", 2020, 1);

            document.NiceSave(assetPath);
            AssetDatabase.Refresh();
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


        public void CommonUpsertItems(string key, Action<XElement, Dictionary<string, XElement>> callback,
            string assetKey = "object", string elementName = "item")
        {
            try
            {
                if (!TryGetListElement(key, out var itemList)) return;
                var existingItems = itemList.Elements(elementName)
                    .Where(x => x.Attribute(assetKey) != null)
                    .ToDictionary(x => x.Attr(assetKey), x => x);
                callback?.Invoke(itemList, existingItems);
            }
            catch (Exception e)
            {
                CommonWriterExceptionHandler(e);
            }
        }

        public void UpsertStudioItems(in GameObject[] gameObjects, int bigCategory = 0, int midCategory = 0)
        {
            var objects = gameObjects;
            if (!ValidatePrefabWriter(in gameObjects)) return;
            CommonUpsertItems("studioitem", (itemList, existingItems) =>
            {
                foreach (var o in objects)
                    WriteStudioItem(in itemList, in existingItems, o, bigCategory, midCategory);
                Debug.Log($"Successfully wrote {objects.Length} Studio Item(s) to the this mod xml file.");
                EditorApplication.Beep();
            });
        }

        public void InsertStudioMaps(in SceneAsset[] scenes)
        {
            var objects = scenes;
            CommonUpsertItems("map", (itemList, existingItems) =>
            {
                foreach (var o in objects)
                    WriteStudioMap(in itemList, in existingItems, o);
            }, "scene");
        }

        public void InsertCharacterItems(in GameObject[] gameObjects, string key)
        {
            var objects = gameObjects;
            if (!ValidatePrefabWriter(in gameObjects)) return;
            CommonUpsertItems("studioitem", (itemList, existingItems) =>
            {
                foreach (var o in objects)
                    WriteCharacterItem(in itemList, in existingItems, in o);
                Debug.Log($"Successfully wrote {objects.Length} Studio Item(s) to the this mod xml file.");
                EditorApplication.Beep();
            });
        }

        public void Save(bool sayNoToExtension = false)
        {
            InputDocumentObject.NiceSave(Path.Combine(AssetDirectory, FileName + (sayNoToExtension ? "" : ".xml"))
                .ToUnixPath());
        }
    }
}