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


        protected static void WriteModInfo(in XElement root, string key, string value)
        {
            var element = root.Element(key);
            if (element == null) root.Add(new XElement(key, value));
            else element.SetValue(value);
        }
    }
}