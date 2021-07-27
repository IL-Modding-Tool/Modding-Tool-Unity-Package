using System;
using System.Collections.Generic;
using System.Linq;
using hooh_ModdingTool.asm_Packer.Editor;
using hooh_ModdingTool.asm_Packer.Utility;
using UnityEditor;
using UnityEngine;

public partial class HoohTools
{
    public enum Command
    {
        Hair, Clothing, Accessory, AccessoryWithTransform, AccessorySkinned,
        StudioItem, HS2Map = 10, AIMap = 20, AIFurniture, FemaleClothing = 100,
        FemaleTop, RemoveAll = 999
    }

    private static void DrawModSetup()
    {
        WindowUtility.VerticalLayout(() =>
        {
            WindowUtility.Button("Initialize HS2 Map", () => { InitializeObject(Command.HS2Map); });
        });

        WindowUtility.VerticalLayout(() =>
        {
            GUILayout.Label("Initialize Mod Components");
            WindowUtility.HorizontalLayout(() =>
            {
                WindowUtility.Dropdown("Common", new[]
                {
                    new WindowUtility.DropDownItem
                        {Name = "Hair", On = false, Callback = InitializeObject, Parameter = Command.Hair},
                    new WindowUtility.DropDownItem
                        {Name = "Clothing", On = false, Callback = InitializeObject, Parameter = Command.Clothing},
                    new WindowUtility.DropDownItem
                    {
                        Name = "Accessory/Initialize with Existing N_Move", On = false, Callback = InitializeObject,
                        Parameter = Command.Accessory
                    },
                    new WindowUtility.DropDownItem
                    {
                        Name = "Accessory/Initialize with New N_Move", On = false, Callback = InitializeObject,
                        Parameter = Command.AccessoryWithTransform
                    },
                    new WindowUtility.DropDownItem
                    {
                        Name = "Accessory/Skinned Accessory", On = false, Callback = InitializeObject,
                        Parameter = Command.AccessorySkinned
                    },
                    new WindowUtility.DropDownItem
                        {Name = "Studio Item", On = false, Callback = InitializeObject, Parameter = Command.StudioItem},
                    new WindowUtility.DropDownItem
                    {
                        Name = "Remove All Mod Related Components", On = false, Callback = InitializeObject,
                        Parameter = Command.RemoveAll
                    },
                });

                WindowUtility.Dropdown("HS2", new[]
                {
                    new WindowUtility.DropDownItem
                        {Name = "Playable Map", On = false, Callback = InitializeObject, Parameter = Command.HS2Map},
                });

                WindowUtility.Dropdown("AI", new[]
                {
                    new WindowUtility.DropDownItem
                    {
                        Name = "Furniture Data", On = false, Callback = InitializeObject,
                        Parameter = Command.AIFurniture
                    },
                    new WindowUtility.DropDownItem
                        {Name = "Playable Map", On = false, Callback = InitializeObject, Parameter = Command.AIMap},
                });
            }, false);
        });
    }

    private static void CallHelper(string functionName, params object[] parameters) =>
        MainAssemblyInterface.AIObjectHelper.InvokeStaticMethod(functionName, parameters);

    private static void MapHelper(string functionName, params object[] parameters) =>
        MainAssemblyInterface.MapInitializer.InvokeStaticMethod(functionName, parameters);

    private static readonly Dictionary<Command, Action<GameObject>> CommandDictionary =
        new Dictionary<Command, Action<GameObject>>
        {
            {Command.Hair, o => CallHelper("InitializeHair", o)},
            {Command.Clothing, o => CallHelper("InitializeClothes", o)},
            {Command.Accessory, o => CallHelper("InitializeAccessory", o, false)},
            {Command.AccessorySkinned, o => CallHelper("InitializeSkinnedAccessory", o)},
            {Command.StudioItem, o => CallHelper("InitializeItem", o)},
            {Command.AIFurniture, o => CallHelper("InitializeFurniture", o)},
            {Command.RemoveAll, o => CallHelper("RemoveAllModRelatedObjects", o)},
            {Command.AIMap, o => MapHelper("InitializeAIMap")},
            {Command.HS2Map, o => MapHelper("InitializeHS2Map")}
        };

    private static void InitializeObject(object commandObject)
    {
        if (!(commandObject is Command command)) return;
        foreach (var o in Selection.gameObjects.Where(PrefabUtility.IsPartOfPrefabInstance))
        {
            if (CommandDictionary.TryGetValue(command, out var callback)) callback?.Invoke(o);
            else Debug.LogError("The fuck just happened?");
        }
    }
}