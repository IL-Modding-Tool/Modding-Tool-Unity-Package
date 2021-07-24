using System;
using System.IO;
using hooh_ModdingTool.asm_Packer.Editor;
using ModPackerModule.Structure.SideloaderMod;
using ModPackerModule.Utility;
using MyBox;
using UnityEditor;

using Style = Common.HoohWindowStyles;
public partial class HoohTools
{
    public bool foldModTemplateInitializer = true;
    public static string TemplateModName = "";
    public static string TemplateModGUID = "";
    public static string TemplateModAuthor = "";
    public static string TemplateModDescription = "";

    public void DrawXMLGenerator(SerializedObject serializedObject)
    {
        var thumbnailGeneratorField = serializedObject.FindProperty("foldModTemplateInitializer");
        thumbnailGeneratorField.boolValue =
            EditorGUILayout.Foldout(foldModTemplateInitializer, "Mod Folder Initializer", true, Style.Foldout);
        if (!foldModTemplateInitializer) return;

        WindowUtility.VerticalLayout(() =>
        {
            EditorPrefs.SetString("hoohTool_modGUID", EditorGUILayout.TextField("Mod GUID: ", TemplateModGUID));
            EditorPrefs.SetString("hoohTool_modName", EditorGUILayout.TextField("Mod Name: ", TemplateModName));
            EditorPrefs.SetString("hoohTool_modAuthor", EditorGUILayout.TextField("Mod Author: ", TemplateModAuthor));
            EditorPrefs.SetString("hoohTool_modDescription",
                EditorGUILayout.TextField("Mod Description: ", TemplateModDescription));

            TemplateModGUID = EditorPrefs.GetString("hoohTool_modGUID");
            TemplateModName = EditorPrefs.GetString("hoohTool_modName");
            TemplateModAuthor = EditorPrefs.GetString("hoohTool_modAuthor");
            TemplateModDescription = EditorPrefs.GetString("hoohTool_modDescription");

            EditorGUILayout.HelpBox(
                "The Example Mod Folder will be created within the path of current directory of the 'Project' window",
                MessageType.Warning);

            WindowUtility.Button("Create Studio Map Mod", () => { CreateModContext(() => { }); });
            WindowUtility.Button("Create Studio Items Mod", () => { CreateModContext(() => { }); });
            WindowUtility.Button("Create Clothing Mod", () => { CreateModContext(() => { }); });
            WindowUtility.Button("Create Character Texture Mod", () => { CreateModContext(() => { }); });
            WindowUtility.Button("Create Graphics Cubemap Mod", () => { CreateModContext(() => { }); });
        });
    }

    public void CreateModContext(Action callback)
    {
        if (TemplateModName.IsNullOrEmpty())
        {
            EditorUtility.DisplayDialog("Error", "Invalid Mod Name", "Ok");
            return;
        }

        if (TemplateModGUID.IsNullOrEmpty())
        {
            EditorUtility.DisplayDialog("Error", "Invalid Mod GUID", "Ok");
            return;
        }

        var projectPath = PathUtils.GetProjectPath();
        var parentFolder = Path.Combine(projectPath, TemplateModName);

        AssetDatabase.CreateFolder(projectPath, TemplateModName);
        SideloaderMod.MakeTemplate(
            TemplateModGUID,
            TemplateModName,
            TemplateModAuthor,
            TemplateModDescription,
            parentFolder
        );
        AssetDatabase.CreateFolder(parentFolder, "assets");
        var combine = Path.Combine(parentFolder, "assets");
        AssetDatabase.CreateFolder(combine, "Textures");
        AssetDatabase.CreateFolder(combine, "Materials");
        AssetDatabase.CreateFolder(combine, "Meshes");
        AssetDatabase.CreateFolder(combine, "AutoMeshes");
        AssetDatabase.CreateFolder(parentFolder, "output");
        AssetDatabase.CreateFolder(parentFolder, "thumbs");
        callback?.Invoke();
    }
}