using UnityEngine;

namespace AssetPipeline
{
    public static class MaterialUtility
    {
        public static bool TrySetTexture(this Material material, string property, Texture2D texture)
        {
            if (!material.HasProperty(property)) return false;
            material.SetTexture(property, texture);
            return true;
        }
        
        public static bool TrySetSpecular(this Material material, Texture2D texture)
        {
            if (material.TrySetTexture("_Specular", texture)) return true;
            if (material.TrySetTexture("_SpecularMap", texture)) return true;
            if (material.TrySetTexture("_Spec", texture)) return true;
            return false;
        }
        public static bool TrySetMetallic(this Material material, Texture2D texture)
        {
            if (material.TrySetTexture("_Metallic", texture)) return true;
            if (material.TrySetTexture("_Metalic", texture)) return true;
            if (material.TrySetTexture("_MetalicGlossiness", texture)) return true;
            if (material.TrySetTexture("_MetalicGloss", texture)) return true;
            if (material.TrySetTexture("_Metal", texture)) return true;
            return false;
        }
        public static bool TrySetOcclusion(this Material material, Texture2D texture)
        {
            if (material.TrySetTexture("_Occlusion", texture)) return true;
            if (material.TrySetTexture("_AmbientOcclusion", texture)) return true;
            return false;
        }
        
        public static bool TrySetNormal(this Material material, Texture2D texture)
        {
            if (material.TrySetTexture("_BumpMap", texture)) return true;
            if (material.TrySetTexture("_NormalMap", texture)) return true;
            return false;
        }
        
        public static bool TrySetRMA(this Material material, Texture2D texture)
        {
            material.SetFloat("_Glossiness", 1f);
            material.SetFloat("_Metallic", 1f);
            if (material.TrySetTexture("_RMA", texture)) return true;
            return false;
        }
    }
}