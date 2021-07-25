using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace hooh_ModdingTool.asm_Packer.Editor
{
    public struct DisplayMenu
    {
        public object[] Parameters;
        public string Name;
        public bool Visible;
        public string Description;

        public DisplayMenu(object[] parameters, string name, bool visible, string description)
        {
            Parameters = parameters;
            Name = name;
            Visible = visible;
            Description = description;
        }

        public void DoSomething(object userdata)
        {
        }
    }

    public static class Constants
    {
        // Register with shit
        // Register what that.

        public static readonly List<DisplayMenu> NonCharacterMenu = new List<DisplayMenu>
        {
            new DisplayMenu {Parameters = new object[] { }, Name = "MainGame/AI/Map", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "MainGame/AI/Furniture", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "MainGame/HS2/Map", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Studio/Animation", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Studio/Item", Visible = true, Description = ""},
        };

        public static readonly List<DisplayMenu> CharacterClothingMenu = new List<DisplayMenu>
        {
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Arm", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Back", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Chest", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Ear", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Face", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Accessory/Glasses", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Hand", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Head", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Kokan", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Leg", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Neck", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/None", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Accessory/Shoulder", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Accessory/Waist", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Female/Top", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Female/Bottom", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Female/Gloves", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Female/Shoes", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Female/Socks", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Female/Stocking", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Female/InnerBottom", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Female/InnerTop", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Male/Top", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Male/Bottom", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Male/Gloves", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Male/Head", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Male/Shoes", Visible = true, Description = ""},
        };

        public static readonly List<DisplayMenu> CharacterMenu = new List<DisplayMenu>
        {
            new DisplayMenu {Parameters = new object[] { }, Name = "Female/Head", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Female/Texture/DetailBody", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Female/Texture/DetailFace", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Female/Texture/SkinBody", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Female/Texture/SkinFace", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Female/Texture/Sunburn", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Hair/Back", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Hair/Extension", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Hair/Front", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Hair/Side", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Male/Texture/Beard", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Male/Texture/DetailBody", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Male/Texture/DetailFace", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Male/Texture/SkinBody", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Male/Texture/SkinFace", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Male/Texture/Sunburn", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Texture/BreastTip", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Texture/Cheek", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Texture/Eyebrow", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Texture/EyeHighlight", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Texture/Eyelash", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Texture/Eyeshadow", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Texture/Iris", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Texture/Lip", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Texture/Mole", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Texture/Paint", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] { }, Name = "Texture/Pattern", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] { }, Name = "Texture/UnderHair", Visible = true, Description = ""},
        };

        public static WindowUtility.DropDownItem[] CharacterClothingDropdownMenu(
            GenericMenu.MenuFunction2 userdataCallback
        )
        {
            return CharacterClothingMenu
                .Where(x => x.Visible)
                .Select(x => new WindowUtility.DropDownItem
                {
                    Name = x.Name, On = false, Callback = userdataCallback,
                    Parameter = x.Parameters
                })
                .ToArray();
        }
    }
}