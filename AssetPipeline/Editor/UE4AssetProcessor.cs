#pragma warning disable 618
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using UnityEditor;
using UnityEngine;

namespace AssetPipeline.Editor
{
    /// <inheritdoc />
    // ReSharper disable once InconsistentNaming
    public class UE4AssetProcessor : AssetPostprocessor
    {
        public static Regex SpecialTextureRegex =
            new Regex(
                @"^.*_(rma|mask|metalic|m|glossiness|g|roughness|r|rough|gloss|metal|o|occlusion|ambientocclusion|occ|ao)\.([A-z]+)$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex RMATextureRegex =
            new Regex(@"^.*_(rma|mask)\.([A-z]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex SpecularTextureRegex =
            new Regex(@"^.*_(specular|spec|s|spc)\.([A-z]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex GlossinessTextureRegex =
            new Regex(@"^.*_(glossiness|g|gloss|gls)\.([A-z]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex MetallicTextureRegex =
            new Regex(@"^.*_(metalic|metallic|m|met|metal|mat|mtlic)\.([A-z]+)$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex AOTextureRegex =
            new Regex(@"^.*_(ao|ambientOcclusion|o|occlusion|occ)\.([A-z]+)$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex NormalTextureRegex =
            new Regex(@"^.*_(bump|normal|n|b|nrm|bmp)\.([A-z]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex DiffuseTextureRegex =
            new Regex(@"^.*_(albedo|diffuse|d|a|col|color|color_alpha|albedotransparency|main)\.([A-z]+)$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex UE4NamePatternRegex =
            new Regex(@"(MI_|SM_|T_|UCX_)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public static Regex CommonStateNames =
            new Regex(@"(_used|_opened|_closed|_close|_open|_destroyed)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);


        public static IRatioScorer partialRatio = ScorerCache.Get<PartialRatioScorer>();


        private static string SaneString(string insaneString, string rendererName, string assetName)
        {
            string target;
            // shitass material - not assigned for fucking ass meshes
            if (insaneString.Replace("Material", "").Length < 5)
                if (rendererName.Replace("Object.", "").Length <= 3) target = assetName;
                else target = rendererName;
            else
                target = insaneString;

            var procString = UE4NamePatternRegex.Replace(target, "");
            return CommonStateNames.Replace(procString, "");
        }

        private Material AssignMaterialUE4(string realPath, Material material, Renderer renderer)
        {
            if (material.name == "")
            {
                renderer.name = "__remove_me";
                renderer.enabled = false;
                return null;
            }

            var targetMaterialFolderPath = Path.GetDirectoryName(Path.GetDirectoryName(realPath)) ?? string.Empty;
            var targetMaterialPath = Path.Combine(targetMaterialFolderPath, "Materials", $"{material.name}.mat");
            var newMaterial = AssetDatabase.LoadAssetAtPath<Material>(targetMaterialPath);
            if (newMaterial == null)
            {
                newMaterial = new Material(material);
                if (!AssetDatabase.IsValidFolder(Path.Combine(targetMaterialFolderPath, "Materials")))
                    AssetDatabase.CreateFolder(targetMaterialFolderPath, "Materials");
                AssetDatabase.CreateAsset(newMaterial, targetMaterialPath);
            }

            var texturesPath = Path.Combine(targetMaterialFolderPath, "Textures");
            var textureDirectory =
                Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? string.Empty, texturesPath);
            var files = Directory.GetFiles(textureDirectory).Where(x => !x.EndsWith(".meta")).ToArray();

            var candidates = FuzzySharp.Process
                .ExtractTop(SaneString(material.name, renderer.name, Path.GetFileNameWithoutExtension(assetPath)),
                    files, null, partialRatio, 10)
                .Select(x => x.Value).ToArray();

            if (!material.shader.name.Contains("RMA"))
            {
                newMaterial.shader = Shader.Find("RMAShaderOpaque");
            }

            var diffuseCandidates = candidates.FirstOrDefault(x => DiffuseTextureRegex.IsMatch(x));
            if (diffuseCandidates != null && diffuseCandidates.Length > 0)
            {
                newMaterial.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    Path.Combine(texturesPath, Path.GetFileName(diffuseCandidates))
                );
                newMaterial.color = Color.white;
            }

            var normalCandidates = candidates.FirstOrDefault(x => NormalTextureRegex.IsMatch(x));
            if (normalCandidates != null && normalCandidates.Length > 0)
            {
                newMaterial.TrySetNormal(
                    AssetDatabase.LoadAssetAtPath<Texture2D>(
                        Path.Combine(texturesPath, Path.GetFileName(normalCandidates))
                    )
                );
            }

            var rmaCandidates = candidates.FirstOrDefault(x => RMATextureRegex.IsMatch(x));
            if (rmaCandidates != null && rmaCandidates.Length > 0)
            {
                newMaterial.TrySetRMA(
                    AssetDatabase.LoadAssetAtPath<Texture2D>(
                        Path.Combine(texturesPath, Path.GetFileName(rmaCandidates))
                    )
                );
            }

            if (assetImporter is ModelImporter modelImporter)
                modelImporter.AddRemap(new AssetImporter.SourceAssetIdentifier(material), newMaterial);

            return newMaterial;
        }

        private Material AssignMaterialStandard(string realPath, Material material, Renderer renderer)
        {
            var materialName = material.name == "" ? Path.GetFileNameWithoutExtension(assetPath) : material.name;
            var targetMaterialFolderPath = Path.GetDirectoryName(Path.GetDirectoryName(realPath)) ?? string.Empty;
            var targetMaterialPath = Path.Combine(targetMaterialFolderPath, "Materials", $"{materialName}.mat");
            var newMaterial = AssetDatabase.LoadAssetAtPath<Material>(targetMaterialPath);
            if (newMaterial == null)
            {
                newMaterial = new Material(material);
                if (!AssetDatabase.IsValidFolder(Path.Combine(targetMaterialFolderPath, "Materials")))
                    AssetDatabase.CreateFolder(targetMaterialFolderPath, "Materials");
                AssetDatabase.CreateAsset(newMaterial, targetMaterialPath);
            }

            var texturesPath = Path.Combine(targetMaterialFolderPath, "Textures");
            var textureDirectory =
                Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? string.Empty, texturesPath);
            var files = Directory.GetFiles(textureDirectory).Where(x => !x.EndsWith(".meta")).ToArray();

            var candidates = FuzzySharp.Process
                .ExtractTop(SaneString(materialName, renderer.name, Path.GetFileNameWithoutExtension(assetPath)),
                    files, null, partialRatio, 10)
                .Select(x => x.Value).ToArray();

            var diffuseCandidates = candidates.FirstOrDefault(x => DiffuseTextureRegex.IsMatch(x));
            if (diffuseCandidates != null && diffuseCandidates.Length > 0)
            {
                newMaterial.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    Path.Combine(texturesPath, Path.GetFileName(diffuseCandidates))
                );
                newMaterial.color = Color.white;
            }

            var normalCandidates = candidates.FirstOrDefault(x => NormalTextureRegex.IsMatch(x));
            if (normalCandidates != null && normalCandidates.Length > 0)
            {
                newMaterial.TrySetNormal(
                    AssetDatabase.LoadAssetAtPath<Texture2D>(
                        Path.Combine(texturesPath, Path.GetFileName(normalCandidates))
                    )
                );
            }


            var aoCandidates = candidates.FirstOrDefault(x => AOTextureRegex.IsMatch(x));
            if (aoCandidates != null && aoCandidates.Length > 0)
            {
                newMaterial.TrySetOcclusion(
                    AssetDatabase.LoadAssetAtPath<Texture2D>(
                        Path.Combine(texturesPath, Path.GetFileName(aoCandidates))
                    )
                );
            }

            var specularCandidates = candidates.FirstOrDefault(x => SpecularTextureRegex.IsMatch(x));
            if (specularCandidates != null && specularCandidates.Length > 0)
            {
                if (!material.shader.name.Contains("Specular"))
                {
                    newMaterial.shader = Shader.Find("Standard (Specular setup)");
                }

                newMaterial.TrySetSpecular(
                    AssetDatabase.LoadAssetAtPath<Texture2D>(
                        Path.Combine(texturesPath, Path.GetFileName(specularCandidates))
                    )
                );
            }
            else
            {
                var metalCandidates = candidates.FirstOrDefault(x => MetallicTextureRegex.IsMatch(x));
                if (metalCandidates != null && metalCandidates.Length > 0)
                {
                    newMaterial.TrySetMetallic(
                        AssetDatabase.LoadAssetAtPath<Texture2D>(
                            Path.Combine(texturesPath, Path.GetFileName(metalCandidates))
                        )
                    );
                }
            }

            // var rmaCandidates = candidates.FirstOrDefault(x => RMATextureRegex.IsMatch(x));
            // if (rmaCandidates != null && rmaCandidates.Length > 0)
            // {
            //     newMaterial.TrySetRMA(
            //         AssetDatabase.LoadAssetAtPath<Texture2D>(
            //             Path.Combine(texturesPath, Path.GetFileName(rmaCandidates))
            //         )
            //     );
            // }
            //
            if (assetImporter is ModelImporter modelImporter)
                modelImporter.AddRemap(new AssetImporter.SourceAssetIdentifier(material), newMaterial);

            return newMaterial;
        }

        private Material OnAssignMaterialModel(Material material, Renderer renderer)
        {
            if (assetPath.Contains("UE4Mesh")) return AssignMaterialUE4(assetPath, material, renderer);
            if (assetPath.Contains("LODMesh")) return AssignMaterialStandard(assetPath, material, renderer);
            return null;
        }

        private void OnPreprocessTexture()
        {
            // ReSharper disable once InvertIf
            if (assetImporter is TextureImporter textureImporter)
            {
                if (SpecialTextureRegex.IsMatch(assetPath))
                    textureImporter.sRGBTexture = false;

                if (NormalTextureRegex.IsMatch(assetPath))
                {
                    textureImporter.textureType = TextureImporterType.NormalMap;
                    textureImporter.isReadable = true;
                    textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
                    textureImporter.sRGBTexture = false;
                }
            }
        }

        // 50 
        // 25
        // 10
        // 5
        // 3
        // 1
        // 0
        private readonly float[] autoLODPlot = {0.25f, 0.1f, 0.05f, 0.03f, 0.01f, 0.005f};

        private void OnPostprocessModel(GameObject g)
        {
            if (assetPath.Contains("LODMesh"))
            {
                var lod = g.AddComponent<LODGroup>();
                var renderers = Enumerable.Range(0, g.transform.childCount)
                    .Select(x => g.transform.GetChild(x).GetComponentsInChildren<Renderer>())
                    .ToArray();

                var slit = 1f / (renderers.Length - 1);
                lod.SetLODs(
                    renderers.Select((x, i) => new LOD(autoLODPlot[i], x)).ToArray()
                );
                lod.RecalculateBounds();
            }
        }
    }
}