using System.IO;
using ModPackerModule.Structure.SideloaderMod;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace ModPackerModule.Utility.Inspector
{
    [ScriptedImporter(1, "sxml")]
    public class XMLImporter : ScriptedImporter
    {
        public string Message;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var textAsset = new TextAsset(File.ReadAllText(ctx.assetPath))
                {name = Path.GetFileName(ctx.assetPath)};
            var sideloaderContext = ScriptableObject.CreateInstance<SideloaderMod>();
            sideloaderContext.LoadFromContext(ctx.assetPath);
            ctx.AddObjectToAsset("source", textAsset);
            ctx.AddObjectToAsset("asset", sideloaderContext);
            ctx.SetMainObject(sideloaderContext);
        }

        public bool TryGetAsset(out SideloaderMod output)
        {
            output = AssetDatabase.LoadAssetAtPath<SideloaderMod>(assetPath);
            output.LoadFromContext(assetPath);
            return output != null;
        }
    }
}