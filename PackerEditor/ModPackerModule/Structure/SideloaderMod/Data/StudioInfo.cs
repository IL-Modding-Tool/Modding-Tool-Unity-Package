using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModPackerModule.Structure.ListData;
using ModPackerModule.Utility;
using UnityEngine;

namespace ModPackerModule.Structure.SideloaderMod.Data
{
    [Serializable]
    public class StudioInfo
    {
        private readonly Dictionary<string, ListStudioItem> ObjectToName = new Dictionary<string, ListStudioItem>();

        public bool TryGetInfo(GameObject gameObject, out ListStudioItem item)
        {
            return ObjectToName.TryGetValue(gameObject.name, out item);
        }

        public IEnumerable<ListStudioItem> StudioItems => ObjectToName.Values.AsEnumerable();

        public IEnumerable<(string Asset, string Name)> ThumbnailTargets =>
            ObjectToName.Select(x =>
            {
                var name = $"{x.Value.BigCategory:D8}-{x.Value.MidCategory:D8}-{x.Value.Name.SanitizeBadPath()}.png";
                return (x.Value.Asset, name);
            });

        public bool RememberStudioItem(ListStudioItem item)
        {
            if (ObjectToName.ContainsKey(item.Asset)) return false;
            ObjectToName.Add(item.Asset, item);
            return true;
        }
    }
}