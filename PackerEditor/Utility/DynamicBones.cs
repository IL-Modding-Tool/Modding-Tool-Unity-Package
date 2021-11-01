using System.Reflection;
using UnityEngine;

namespace hooh_ModdingTool.asm_Packer.Editor
{
    public static class DynamicBones
    {
        private static readonly FieldInfo AllOptionField;
        private static readonly MethodInfo ApplyAllOptionMethod;

        static DynamicBones()
        {
            AllOptionField = MainAssemblyInterface.DynamicBonePreset?.GetField("AllOptions");
            ApplyAllOptionMethod = MainAssemblyInterface.DynamicBonePreset?.GetMethod("ApplyAllOption");
        }

        public static string[] GetPresetList()
        {
            if (!(AllOptionField.GetValue(null) is string[] shit))
                return new string[] { };
            return shit;
        }

        public static void Apply(int index)
        {
            ApplyAllOptionMethod.Invoke(null, new object[]
            {
                index, "dyn_", true
            });
        }
    }
}