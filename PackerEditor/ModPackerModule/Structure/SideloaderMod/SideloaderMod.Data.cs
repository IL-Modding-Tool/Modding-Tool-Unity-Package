using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModPackerModule.Utility;
using UnityEditor;
using UnityEngine;

namespace ModPackerModule.Structure.SideloaderMod
{
    public partial class SideloaderMod
    {
        public string AssetDirectory { get; protected set; }

        public string AssetFolder { get; protected set; }

        public string PathThumbnails
        {
            get
            {
                var path = Path.Combine(AssetFolder, "thumbs").ToUnixPath();
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        public GameObject[] StudioGameObjects => StudioInfo.StudioItems
            .Select(x => AssetDatabase.LoadAssetAtPath<GameObject>(Assets.GetPathFromName(x.Asset))).ToArray();

        public GameObject[] CharacterGameObjects => CharacterInfo.ThumbnailTargets
            .Select(x => AssetDatabase.LoadAssetAtPath<GameObject>(Assets.GetPathFromName(x))).ToArray();

        public IEnumerable<(GameObject, string Name)> StudioThumbnailTargets => StudioInfo.ThumbnailTargets.Select(x =>
            (AssetDatabase.LoadAssetAtPath<GameObject>(Assets.GetPathFromName(x.Asset)), x.Name));

        private string ManifestAssetFolder =>
            Path.Combine(Constants.PathBundleCache, DependencyData.ManifestName).ToUnixPath();

        private string TempZipFolder(string target) =>
            Path.Combine(Constants.PathTempFolder, GetSplitTargetName(target)).ToUnixPath();

        private static string TempPath(string targetFolder, string path) =>
            Path.Combine(targetFolder, path).ToUnixPath();
    }
}