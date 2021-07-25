using System;
using System.IO;
using hooh_ModdingTool.asm_Packer.Editor;
using ModPackerModule.Structure.SideloaderMod;
using ModPackerModule.Utility;
using MyBox;
using UnityEditor;
using UnityEngine;
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
            TemplateModGUID = EditorPrefs.GetString("hoohTool_modGUID");
            TemplateModName = EditorPrefs.GetString("hoohTool_modName");
            TemplateModAuthor = EditorPrefs.GetString("hoohTool_modAuthor");
            TemplateModDescription = EditorPrefs.GetString("hoohTool_modDescription");

            EditorPrefs.SetString("hoohTool_modGUID", EditorGUILayout.TextField("Mod GUID: ", TemplateModGUID));
            EditorPrefs.SetString("hoohTool_modName", EditorGUILayout.TextField("Mod Name: ", TemplateModName));
            EditorPrefs.SetString("hoohTool_modAuthor", EditorGUILayout.TextField("Mod Author: ", TemplateModAuthor));
            EditorPrefs.SetString("hoohTool_modDescription",
                EditorGUILayout.TextField("Mod Description: ", TemplateModDescription));

            EditorGUILayout.HelpBox(
                "The Example Mod Folder will be created within the path of current directory of the 'Project' window",
                MessageType.Warning);

            WindowUtility.Button("Create Mod Context Folder", () => { CreateModContext(() => { }); });
        });
    }

    public void CreateModContext(Action callback)
    {
        if (TemplateModName.IsNullOrEmpty())
        {
            EditorApplication.Beep();
            EditorUtility.DisplayDialog("Error", "You need to provide valid Mod Name to create mod context folder.",
                "Ok");
            return;
        }

        if (TemplateModGUID.IsNullOrEmpty())
        {
            EditorApplication.Beep();
            EditorUtility.DisplayDialog("Error",
                "You need to provide valid Mod GUID Name to create mod context folder. The mod GUID should not be same with other mod's GUID in order to prevent conflict between the mods.",
                "Ok");
            return;
        }

        var projectPath = PathUtils.GetProjectPath();
        var parentFolder = Path.Combine(projectPath, TemplateModName);

        AssetDatabase.CreateFolder(projectPath, TemplateModName);
        AssetDatabase.CreateFolder(parentFolder, "assets");
        var combine = Path.Combine(parentFolder, "assets");
        AssetDatabase.CreateFolder(combine, "Textures");
        AssetDatabase.CreateFolder(combine, "Materials");
        AssetDatabase.CreateFolder(combine, "Meshes");
        AssetDatabase.CreateFolder(combine, "AutoMeshes");
        AssetDatabase.CreateFolder(parentFolder, "output");
        AssetDatabase.CreateAsset(new TextAsset("Put prefabs in here."),
            Path.Combine(parentFolder, "output", "README.txt"));
        AssetDatabase.CreateFolder(parentFolder, "thumbs");
        AssetDatabase.CreateAsset(new TextAsset("put thumbnails in here."),
            Path.Combine(parentFolder, "thumbs", "README.txt"));
        SideloaderMod.MakeTemplate(
            TemplateModGUID, TemplateModName, TemplateModAuthor, TemplateModDescription,
            parentFolder
        );
        callback?.Invoke();
    }
}