using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModPackerModule.Structure.ListData;
using ModPackerModule.Utility;
using MyBox;
using UnityEngine;

namespace ModPackerModule.Structure.SideloaderMod.Data
{
    [Serializable]
    public class StudioInfo
    {
        private readonly Dictionary<string, ListStudioItem> _objectToName = new Dictionary<string, ListStudioItem>();

        public bool TryGetInfo(GameObject gameObject, out ListStudioItem item)
        {
            return _objectToName.TryGetValue(gameObject.name, out item);
        }

        public IEnumerable<ListStudioItem> StudioItems => _objectToName.Values.AsEnumerable();

        public IEnumerable<(string Asset, string Name)> ThumbnailTargets =>
            _objectToName.Select(x =>
            {
                var name = $"{x.Value.BigCategory:D8}-{x.Value.MidCategory:D8}-{x.Value.Name.SanitizeBadPath()}.png";
                return (x.Value.Asset, name);
            });

        public bool RememberStudioItem(ListStudioItem item)
        {
            if (item == null || item.Asset.IsNullOrEmpty()) return false;
            if (_objectToName.ContainsKey(item.Asset)) return false;
            _objectToName.Add(item.Asset, item);
            return true;
        }
    }
}