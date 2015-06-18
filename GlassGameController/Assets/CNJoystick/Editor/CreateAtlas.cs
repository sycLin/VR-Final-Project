using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class CreateAtlas : ScriptableWizard
{
    public string AtlasName = "Atlas";
    public Texture2D[] textures;
    public int padding = 4;

    [MenuItem("GameObject/Create Other/Create Atlas")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CreateAtlas>("Create Atlas");
    }

    void OnWizardCreate()
    {
        GenerateAtlas();
    }

    void ConfigureForAtlas(string texturePath)
    {
        TextureImporter texImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        TextureImporterSettings texImporterSettings = new TextureImporterSettings();

        texImporter.textureType = TextureImporterType.Sprite;
        texImporter.ReadTextureSettings(texImporterSettings);
        texImporterSettings.readable = true;

        texImporter.SetTextureSettings(texImporterSettings);
        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
    }

    void GenerateAtlas()
    {
        foreach (Texture2D t in textures)
            ConfigureForAtlas(AssetDatabase.GetAssetPath(t));

        Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        Rect[] uvs = texture.PackTextures(textures, padding, 4096);

        string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/" + AtlasName + ".png");

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(assetPath, bytes);
        bytes = null;

        UnityEngine.Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(assetPath);
        texture = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;

        ConfigureSpriteAtlas(texture, uvs);
    }

    void ConfigureSpriteAtlas(Texture2D texture, Rect[] uvs)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter texImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        TextureImporterSettings texImporterSettings = new TextureImporterSettings();

        texImporter.textureType = TextureImporterType.Sprite;
        texImporter.spriteImportMode = SpriteImportMode.Multiple;
      //  texImporterSettings.readable = true;

        SpriteMetaData[] spritesheetMeta = new SpriteMetaData[uvs.Length];
        for(int i = 0; i < uvs.Length; i++)
        {
            SpriteMetaData currentMeta = new SpriteMetaData();
            Rect currentRect = uvs[i];
            currentRect.x *= texture.width;
            currentRect.width *= texture.width; 
            currentRect.y *= texture.height;
            currentRect.height *= texture.height;
            currentMeta.rect = currentRect;
            currentMeta.name = textures[i].name;
            currentMeta.alignment = (int)SpriteAlignment.Center;
            currentMeta.pivot = new Vector2(currentRect.width / 2, currentRect.height / 2);
            spritesheetMeta[i] = currentMeta;
        }
        texImporter.spritesheet = spritesheetMeta;
        texImporter.spritePixelsToUnits = 1000f;
        texImporter.ReadTextureSettings(texImporterSettings);

        texImporter.SetTextureSettings(texImporterSettings);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
    }

}