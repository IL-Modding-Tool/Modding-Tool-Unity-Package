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
            new DisplayMenu
                {Parameters = new object[] {"accarm"}, Name = "Accessory/Arm", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"accback"}, Name = "Accessory/Back", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"accchest"}, Name = "Accessory/Chest", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"accear"}, Name = "Accessory/Ear", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"accface"}, Name = "Accessory/Face", Visible = true, Description = ""},
            new DisplayMenu
            {
                Parameters = new object[] {"accglasses"}, Name = "Accessory/Glasses", Visible = true, Description = ""
            },
            new DisplayMenu
                {Parameters = new object[] {"acchand"}, Name = "Accessory/Hand", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"acchead"}, Name = "Accessory/Head", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"acckokan"}, Name = "Accessory/Kokan", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"accleg"}, Name = "Accessory/Leg", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"accneck"}, Name = "Accessory/Neck", Visible = true, Description = ""},
            new DisplayMenu
            {
                Parameters = new object[] {"accshoulder"}, Name = "Accessory/Shoulder", Visible = true, Description = ""
            },
            new DisplayMenu
                {Parameters = new object[] {"accwaist"}, Name = "Accessory/Waist", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] {"ftop"}, Name = "Female/Top", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"fbottom"}, Name = "Female/Bottom", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"fgloves"}, Name = "Female/Gloves", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"fshoes"}, Name = "Female/Shoes", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"fsocks"}, Name = "Female/Socks", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"fpanst"}, Name = "Female/Stocking", Visible = true, Description = ""},
            new DisplayMenu
            {
                Parameters = new object[] {"finbottom"}, Name = "Female/InnerBottom", Visible = true, Description = ""
            },
            new DisplayMenu
                {Parameters = new object[] {"fintop"}, Name = "Female/InnerTop", Visible = true, Description = ""},
            new DisplayMenu {Parameters = new object[] {"mtop"}, Name = "Male/Top", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"mbottom"}, Name = "Male/Bottom", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"mgloves"}, Name = "Male/Gloves", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"mshoes"}, Name = "Male/Shoes", Visible = true, Description = ""},
        };

        public static readonly List<DisplayMenu> CharacterMenu = new List<DisplayMenu>
        {
            new DisplayMenu {Parameters = new object[] {"mhead"}, Name = "Male/Head", Visible = true, Description = ""},
            new DisplayMenu
                {Parameters = new object[] {"fhead"}, Name = "Female/Head", Visible = true, Description = ""},
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
                    Name = x.Name,
                    On = false,
                    Callback = userdataCallback,
                    Parameter = x.Parameters
                })
                .ToArray();
        }
    }
}