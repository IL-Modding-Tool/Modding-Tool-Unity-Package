using System;
using System.Linq;
using Common;
using hooh_ModdingTool.asm_Packer.Editor;
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
            if (assetTargets.Length > 1)
            {
                EditorGUILayout.HelpBox(
                    "Currently you cannot select more than two mod context xml files.\n Please use hooh Tools window to build multiple xml files at one time.",
                    MessageType.Info);
                return;
            }

            GUI.enabled = true;
            // -----------------------------------------------
            EditorGUILayout.HelpBox(
                "You can check information about this mod file in the \"Imported Objects\" Menu below.",
                MessageType.Info);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(
                ActiveEditorTracker.sharedTracker.isLocked ? "Release this window" : "Keep this window"))
                ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;

            if (ActiveEditorTracker.sharedTracker.isLocked && GUILayout.Button("Select this mod.sxml file."))
                EditorGUIUtility.PingObject(assetTarget);

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("box");
            GUILayout.Label("Build Mod", HoohWindowStyles.Medium);
            // -----------------------------------------------
            if (assetTarget is SideloaderMod t)
            {
                if (!t.lastBuildTime.IsNullOrEmpty() && !t.lastBuildVersion.IsNullOrEmpty())
                {
                    EditorGUILayout.HelpBox(
                        $"You built the mod in {t.lastBuildTime}.\nThe last build was version {t.lastBuildVersion}.",
                        MessageType.Info);
                }
            }

            GUILayout.BeginHorizontal();
            PackerButton("Build Mod", mod => { mod.Build(HoohTools.GameExportPath); });
            PackerButton("Test Mod", mod => { mod.Build(HoohTools.GameExportPath, true); });
            GUILayout.EndHorizontal();
            // -----------------------------------------------
            GUILayout.Label("Thumbnail Creation", HoohWindowStyles.Medium);
            GUILayout.BeginHorizontal();
            PackerButton("Studio", mod => mod.CreateStudioThumbnails());
            PackerButton("Item", mod => mod.CreateItemThumbnails());
            GUILayout.EndHorizontal();
            // -----------------------------------------------
            GUILayout.Label("Example Creation", HoohWindowStyles.Medium);
            EditorGUILayout.HelpBox(
                "This will only add example item entry inside of the <target> tag in sxml file. \n" +
                "Which will invalidate this mod $xml file by the default. " +
                "\n You will need to look inside of the file in order to make this mod sxml file valid after adding example file.",
                MessageType.Warning);
            GUILayout.BeginHorizontal();
            PackerButton("Add Studio Example", mod => mod.CreateStudioThumbnails());
            PackerCharacterButton("Add Clothing Example", _dropDownItems);
            PackerCharacterButton("Add Character Example", _dropDownItems);
            GUILayout.EndHorizontal();
            PackerButton("Add Studio Map Example", mod => mod.CreateItemThumbnails());
            // -----------------------------------------------
            GUILayout.Label("Item Registration from Selection", HoohWindowStyles.Medium);
            if (!ActiveEditorTracker.sharedTracker.isLocked)
            {
                EditorGUILayout.HelpBox(
                    "You can generate mod targets when you press 'Keep this window' and select other prefabs in the mod folders and press buttons below.",
                    MessageType.Info);
            }
            else if (Selection.activeObject == assetTarget)
            {
                EditorGUILayout.HelpBox(
                    "Select other prefabs to add from this folder. Just remember that you can't select assets from the parent directory.",
                    MessageType.Info);
            }

            GUI.enabled = assetTargets.Length <= 1
                          && Selection.objects.Length > 0
                          && ActiveEditorTracker.sharedTracker.isLocked
                          && Selection.activeObject != assetTarget;


            GUILayout.BeginHorizontal();
            var xmlBigCategoryField = serializedObject.FindProperty("xmlBigCategory");
            var xmlSmallCategoryField = serializedObject.FindProperty("xmlSmallCategory");
            var l = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 150;
            EditorGUILayout.PropertyField(xmlBigCategoryField, new GUIContent("Big Category"));
            EditorGUILayout.PropertyField(xmlSmallCategoryField, new GUIContent("Mid Category"));
            EditorGUIUtility.labelWidth = l;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            PackerButton("As Studio Items",
                mod =>
                {
                    mod.UpsertStudioItems(SelectedPrefabs, xmlBigCategoryField.intValue,
                        xmlSmallCategoryField.intValue);
                    mod.Save(true);
                });
            PackerButton("As Studio Scenes", mod => mod.CreateItemThumbnails());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            PackerCharacterButton("As Clothing Items", _dropDownItems);
            PackerCharacterButton("As Character Items", _dropDownItems);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            // -----------------------------------------------
            GUILayout.EndVertical();

            ApplyRevertGUI();

            GUI.enabled = false;
        }

        private GameObject[] SelectedPrefabs => Selection.objects.OfType<GameObject>()
            .Where(PrefabUtility.IsPartOfAnyPrefab).ToArray();

        private static WindowUtility.DropDownItem[] _dropDownItems;

        public override void OnEnable()
        {
            _dropDownItems = Constants.CharacterClothingDropdownMenu(delegate(object userdata)
            {
                if (!(assetTarget is SideloaderMod t)) return;
                var selectedPrefabs = SelectedPrefabs;
                if (selectedPrefabs.Length <= 0)
                {
                    EditorUtility.DisplayDialog("Invalid Command",
                        "You must select at least one valid prefab from the Project window.",
                        "Mama Mia");
                    return;
                }

                Debug.Log("bruhmoment");
            });
            base.OnEnable();
        }

        private void PackerCharacterButton(string buttonName, WindowUtility.DropDownItem[] items)
        {
            WindowUtility.Dropdown(buttonName, items);
        }

        private void PackerButton(string buttonName, Action<SideloaderMod> callback)
        {
            if (!GUILayout.Button(buttonName)) return;
            if (!(assetTarget is SideloaderMod t)) return;
            callback?.Invoke(t);
        }
    }
}