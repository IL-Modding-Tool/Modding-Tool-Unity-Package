using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetPipeline.Editor
{
    public class TextureProcessor : AssetPostprocessor
    {
        private void OnPostprocessTexture(Texture2D texture)
        {
        }

        private void OnPreprocessTexture()
        {
            var directoryName = (Path.GetDirectoryName(assetPath) ?? string.Empty);
            if (!(assetImporter is TextureImporter textureImporter)) return;
            if (directoryName.EndsWith("thumbs"))
            {
                if (!assetPath.EndsWith(".png")) return;
                textureImporter.sRGBTexture = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.compressionQuality = 60;
                textureImporter.crunchedCompression = true;
                textureImporter.maxTextureSize = 128;
                textureImporter.textureCompression = TextureImporterCompression.CompressedLQ;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
            }
            else if (directoryName.EndsWith("frames"))
            {
                textureImporter.isReadable = true;
                textureImporter.sRGBTexture = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.crunchedCompression = false;
                textureImporter.alphaIsTransparency = true;
                textureImporter.maxTextureSize = 128;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
            }
        }
    }
}