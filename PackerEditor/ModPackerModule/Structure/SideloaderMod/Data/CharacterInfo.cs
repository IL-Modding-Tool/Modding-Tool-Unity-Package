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
    public class CharacterInfo
    {
        private readonly Dictionary<string, ListCharacterBase> ObjectToName = new Dictionary<string, ListCharacterBase>();

        public bool TryGetInfo(GameObject gameObject, out ListCharacterBase item)
        {
            return ObjectToName.TryGetValue(gameObject.name, out item);
        }

        public IEnumerable<ListCharacterBase> StudioItem()
        {
            return ObjectToName.Values.AsEnumerable();
        }

        public IEnumerable<string> ThumbnailTargets => ObjectToName.Keys;

        public bool RememberCharacterItem(ListCharacterBase item)
        {
            foreach (var t in item.MainAssets())
            {
                if (ObjectToName.ContainsKey(t)) return false;
                ObjectToName.Add(t, item);
            }

            return true;
        }
    }
}