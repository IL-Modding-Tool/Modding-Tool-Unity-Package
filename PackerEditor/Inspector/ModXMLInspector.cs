using System;
using System.Diagnostics;
using System.Linq;
using System.Media;
using Common;
using hooh_ModdingTool.asm_Packer.Editor;
using ModPackerModule.Structure.SideloaderMod;
using ModPackerModule.Structure.SideloaderMod.TypingMonkey;
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

            GUILayout.Label("Build Mod", HoohWindowStyles.Medium);
            using (new GUILayout.VerticalScope("box"))
            {
                if (assetTarget is SideloaderMod t)
                {
                    if (!t.lastBuildTime.IsNullOrEmpty() && !t.lastBuildVersion.IsNullOrEmpty())
                    {
                        EditorGUILayout.HelpBox(
                            $"You built the mod in {t.lastBuildTime}.\nThe last build was version {t.lastBuildVersion}.",
                            MessageType.Info);
                    }

                    var isValid = t.Issues.Count <= 0;
                    var gameExportPath = HoohTools.GameExportPath;

                    using (new GUILayout.HorizontalScope())
                    {
                        EditorPrefs.SetString("hoohTool_exportPath",
                            EditorGUILayout.TextField("Output Directory: ", HoohTools.GameExportPath));
                        HoohTools.GameExportPath = EditorPrefs.GetString("hoohTool_exportPath");
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        GUI.backgroundColor = isValid ? HoohWindowStyles.green : HoohWindowStyles.red;
                        PackerButton("Build Mod", mod =>
                        {
                            var check = isValid || EditorUtility.DisplayDialog("Are you sure?",
                                "This XML is not valid. Are you sure that you want to continue?", "Yes", "No");
                            if (!check) return;
                            if (mod.Build(gameExportPath))
                            {
                                WindowUtility.PlayClip("good");
                            }
                            else
                            {
                                if (BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling ||
                                    EditorApplication.isPlayingOrWillChangePlaymode) return;

                                WindowUtility.PlayClip("warning");
                                EditorUtility.DisplayDialog("Error!",
                                    "An error occured while the tool is building the mod.\nCheck console for more detailed information.",
                                    "Dismiss");
                            }
                        });
                        GUI.backgroundColor = Color.white;
                        PackerButton("Test Mod", mod => { mod.Build(gameExportPath, true); });
                    }
                }
            }

            // -----------------------------------------------
            GUILayout.Label("Thumbnail Creation", HoohWindowStyles.Medium);
            using (new GUILayout.VerticalScope("box"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    PackerButton("Studio", mod => mod.CreateStudioThumbnails());
                    PackerButton("Item", mod => mod.CreateItemThumbnails());
                }
            }

            // -----------------------------------------------
            GUILayout.Label("Example Creation", HoohWindowStyles.Medium);
            using (new GUILayout.VerticalScope("box"))
            {
                EditorGUILayout.HelpBox(
                    "This will only add example item entry inside of the <target> tag in sxml file. \n" +
                    "Which will invalidate this mod $xml file by the default. " +
                    "\n You will need to look inside of the file in order to make this mod sxml file valid after adding example file.",
                    MessageType.Warning);

                using (new GUILayout.HorizontalScope())
                {
                    PackerButton("Add Studio Example", mod =>
                    {
                        var monkey = new TypingMonkey(in mod, mod.InputDocumentObject);
                        monkey.WriteCategory(true, 2020, "Example Big Category");
                        monkey.WriteCategory(false, 2020, "Example Mid Category", 1);
                        monkey.WriteStudioItem("example", "Example Item", 2020, 1);
                        monkey.Update();
                    });
                    PackerButton("Add Studio Map Example", mod =>
                    {
                        var monkey = new TypingMonkey(in mod, mod.InputDocumentObject);
                        monkey.WriteStudioMap("example", "example");
                        monkey.Update();
                    });
                }

                using (new GUILayout.HorizontalScope())
                {
                    PackerCharacterButton("Add Clothing Example", _dropDownExampleItems);
                    PackerCharacterButton("Add Character Example", _dropDownExampleItems);
                }
            }

            // -----------------------------------------------
            GUILayout.Label("Item Registration from Selection", HoohWindowStyles.Medium);
            using (new GUILayout.VerticalScope("box"))
            {
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


                var xmlBigCategoryField = serializedObject.FindProperty("xmlBigCategory");
                var xmlSmallCategoryField = serializedObject.FindProperty("xmlSmallCategory");

                using (new GUILayout.HorizontalScope())
                {
                    var l = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 150;
                    EditorGUILayout.PropertyField(xmlBigCategoryField, new GUIContent("Big Category"));
                    EditorGUILayout.PropertyField(xmlSmallCategoryField, new GUIContent("Mid Category"));
                    EditorGUIUtility.labelWidth = l;
                }

                using (new GUILayout.HorizontalScope())
                {
                    PackerButton("As Studio Items",
                        mod =>
                        {
                            mod.UpsertStudioItems(SelectedPrefabs, xmlBigCategoryField.intValue,
                                xmlSmallCategoryField.intValue);
                            mod.Save(true);
                        });
                    PackerButton("As Studio Scenes", mod =>
                    {
                        mod.InsertStudioMaps(SelectedScenes);
                        mod.Save(true);
                    });
                }

                using (new GUILayout.HorizontalScope())
                {
                    // PackerCharacterButton("As Clothing Items", _dropDownItems);
                    // PackerCharacterButton("As Character Items", _dropDownItems);
                    GUI.enabled = true;
                }
            }

            ApplyRevertGUI();
            GUI.enabled = false;
        }

        private GameObject[] SelectedPrefabs => Selection.objects.OfType<GameObject>()
            .Where(PrefabUtility.IsPartOfAnyPrefab).ToArray();

        private SceneAsset[] SelectedScenes => Selection.objects.OfType<SceneAsset>().ToArray();

        private static WindowUtility.DropDownItem[] _dropDownItems;
        private static WindowUtility.DropDownItem[] _dropDownExampleItems;

        public override void OnEnable()
        {
            _dropDownExampleItems = Constants.CharacterClothingDropdownMenu(delegate(object userdata)
            {
                if (!(assetTarget is SideloaderMod t)) return;
                if (!(userdata is object[] parameters)) return;

                var key = parameters[0] as string;
                var monkey = new TypingMonkey(in t, t.InputDocumentObject);
                monkey.WriteCharacterClothing(key, "example", "example");
                monkey.Update();
            });
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

                var monkey = new TypingMonkey(in t, t.InputDocumentObject);
                monkey.Update();
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
            if (!(target is XMLImporter xmlImporter)) return;
            t.LoadFromContext(xmlImporter.assetPath);
            callback?.Invoke(t);
        }
    }
}