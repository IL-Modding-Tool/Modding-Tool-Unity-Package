using System;
using System.Linq;

namespace hooh_ModdingTool.asm_Packer.Editor
{
    /// <summary>
    /// TODO: move this interface to more common side of the editor.
    /// </summary>
    public static class MainAssemblyInterface
    {
        public static readonly Type AIObjectHelper;
        public static readonly Type MapInitializer;

        static MainAssemblyInterface()
        {
            var fullTypeEnumerator = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Select(x => (x.Name, x.Assembly, x))
                .Where(x => x.Item2.FullName.Contains("Assembly-CSharp"));

            foreach (var (a, _, c) in fullTypeEnumerator)
            {
                switch (a)
                {
                    case "AIObjectHelper":
                        AIObjectHelper = c;
                        break;
                    case "MapInitializer":
                        MapInitializer = c;
                        break;
                }
            }
        }

        public static void CallMethod(string functionName, params object[] parameters)
        {
            if (ReferenceEquals(null, AIObjectHelper))
                throw new NullReferenceException("Failed to find MapInitializer in Unity Editor.");
            var method = AIObjectHelper.GetMethod(functionName);
            if (ReferenceEquals(null, method))
                throw new NullReferenceException($"Failed to find method {functionName}");
            method.Invoke(null, parameters);
        }
    }
}