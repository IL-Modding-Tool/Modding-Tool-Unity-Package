using System;
using System.Globalization;
using System.Linq;
using Common;
using ModPackerModule.Structure.SideloaderMod;
using MyBox;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace ModPackerModule.Utility.Inspector
{
    [CustomEditor(typeof(XMLImporter))]
    public class ModXMLInspector : ScriptedImporterEditor
    {
        protected override bool useAssetDrawPreview
        {
            get => false;
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = true;
            GUILayout.BeginVertical("box");
            GUILayout.Label("Build Mod", HoohWindowStyles.Medium);
            GUILayout.BeginHorizontal();
            PackerButton("Build Mod", mod => { mod.Build(HoohTools.GameExportPath); });
            PackerButton("Test Mod", mod => { mod.Build(HoohTools.GameExportPath, true); });
            GUILayout.EndHorizontal();
            GUILayout.Label("Thumbnail Creation", HoohWindowStyles.Medium);
            GUILayout.BeginHorizontal();
            PackerButton("Studio", mod => mod.CreateStudioThumbnails());
            PackerButton("Item", mod => mod.CreateItemThumbnails());
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            ApplyRevertGUI();

            GUI.enabled = false;
        }

        private void Execute(Action<XMLImporter> callback)
        {
            foreach (var xmlImporter in targets.Cast<XMLImporter>())
            {
                callback?.Invoke(xmlImporter);
            }
        }

        private void PackerButton(string buttonName, Action<SideloaderMod> callback)
        {
            if (!GUILayout.Button(buttonName)) return;
            Execute(t =>
            {
                if (t.TryGetAsset(out var sideloaderMod))
                    callback?.Invoke(sideloaderMod);
            });
        }
    }
}