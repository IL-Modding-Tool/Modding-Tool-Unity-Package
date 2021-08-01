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
            if (string.IsNullOrEmpty(assetPath)) return;
            if (!(assetImporter is TextureImporter textureImporter)) return;
            var directoryName = (Path.GetDirectoryName(assetPath) ?? string.Empty);
            var fileName = (Path.GetFileNameWithoutExtension(assetPath));

            // Textures
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
                return;
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
                return;
            }
            else if (fileName.EndsWith("bodymask"))
            {
                // do not - with it.
                textureImporter.textureType = TextureImporterType.Default;
                textureImporter.textureShape = TextureImporterShape.Texture2D;
                textureImporter.sRGBTexture = false;
                textureImporter.mipmapEnabled = false;
                textureImporter.alphaIsTransparency = false;
                textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                // Bodymask
            }
        }
    }
}