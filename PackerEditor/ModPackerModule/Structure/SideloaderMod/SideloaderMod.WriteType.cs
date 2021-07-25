using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using hooh_ModdingTool.asm_Packer.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModPackerModule.Structure.SideloaderMod
{
    public partial class SideloaderMod
    {
        protected static void WriteStudioItem(in XElement studioItemList, in Dictionary<string, XElement> existingItems,
            GameObject gameObject, int bigCategory, int midCategory)
        {
            var gameObjectName = gameObject.name;
            var prettyName = CommonUtils.Prettify(gameObjectName);

            if (existingItems.ContainsKey(gameObjectName))
            {
                // Update
                var item = existingItems[gameObjectName];
                item.SetAttributeValue("mid-category", midCategory);
                item.SetAttributeValue("big-category", bigCategory);
            }
            else
            {
                // Insert
                studioItemList.Add(new XElement("item",
                    new XAttribute("object", gameObjectName),
                    new XAttribute("name", prettyName),
                    new XAttribute("mid-category", midCategory),
                    new XAttribute("big-category", bigCategory)
                ));
            }
        }

        protected static void WriteStudioMap(in XElement studioItemList, in Dictionary<string, XElement> existingItems,
            SceneAsset scene)
        {
            var gameObjectName = scene.name;
            var prettyName = CommonUtils.Prettify(gameObjectName);

            if (!existingItems.ContainsKey(gameObjectName))
                studioItemList.Add(new XElement("item",
                    new XAttribute("scene", gameObjectName),
                    new XAttribute("name", prettyName)
                ));
        }

        protected static void WriteCharacterItem(in XElement studioItemList,
            in Dictionary<string, XElement> existingItems,
            in GameObject gameObject)
        {
        }


        protected static void WriteMoveFolder(in XElement root, string from, string autoPath = "studiothumb",
            string filter = @".+?\.png")
        {
            var element = root.Elements("move").FirstOrDefault(x => x.Attribute("from")?.Value == from);

            if (element == null)
            {
                root.Add(
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


        protected static XElement GetBuildTarget(in XElement root, string type)
        {
            var list = root.Element(type);
            if (list != null) return list;
            list = new XElement("list", new XAttribute("type", type));
            root.Add(list);
            return list;
        }

        protected static void WriteCategory(in XElement root, bool big, int id, string name, int bigid = -1)
        {
            var listType = big ? "bigcategory" : "midcategory";
            var listRoot = GetBuildTarget(root, listType);
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

        protected static void WriteStudioItem(in XElement root, string asset, string name, int bigCategory,
            int midCategory)
        {
            var listRoot = GetBuildTarget(root, "studioitem");
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

        protected static void WriteFolderBundle(in XElement root, string from, string autoPath = "prefabs",
            string filter = @".+?\.prefab")
        {
            var element = root.Elements("folder")
                .FirstOrDefault(x => x.Attribute("from")?.Value == from);

            if (element == null)
            {
                root.Add(
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

        protected static void WriteModInfo(in XElement root, string key, string value)
        {
            var element = root.Element(key);
            if (element == null) root.Add(new XElement(key, value));
            else element.SetValue(value);
        }
    }
}