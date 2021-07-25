using System.Collections.Generic;
using System.Xml.Linq;
using hooh_ModdingTool.asm_Packer.Utility;
using UnityEngine;

namespace ModPackerModule.Structure.SideloaderMod
{
    public partial class SideloaderMod
    {
        protected void WriteStudioItem(in XElement studioItemList, in Dictionary<string, XElement> existingItems,
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
    }
}