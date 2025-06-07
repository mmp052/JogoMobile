#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureQualityFixer : EditorWindow
{
    // Menu Tools (método original)
    [MenuItem("Tools/Fix Mobile Texture Quality")]
    static void FixTextureQuality()
    {
        ProcessTextures();
    }
    
    [MenuItem("Tools/Quick Quality Settings")]
    static void QuickQualitySettings()
    {
        ApplyQualitySettings();
    }
    
    [MenuItem("Tools/Show Mobile Optimization Info")]
    static void ShowInfo()
    {
        ShowHelpInfo();
    }
    
    // Alternativa caso Tools não apareça
    [MenuItem("Window/Mobile Texture Optimizer")]
    static void ShowWindow()
    {
        TextureQualityFixer window = GetWindow<TextureQualityFixer>("Mobile Optimizer");
        window.Show();
    }
    
    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Mobile Texture Optimizer", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("1. Fix Mobile Texture Quality", GUILayout.Height(30)))
        {
            ProcessTextures();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("2. Quick Quality Settings", GUILayout.Height(30)))
        {
            ApplyQualitySettings();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Show Help Info", GUILayout.Height(25)))
        {
            ShowHelpInfo();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Execute na ordem 1 → 2 → Build → Teste!", EditorStyles.helpBox);
    }
    
    static void ProcessTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites" });
        int fixedCount = 0;
        
        EditorUtility.DisplayProgressBar("Corrigindo Texturas", "Processando...", 0f);
        
        for (int i = 0; i < guids.Length; i++)
        {
            string guid = guids[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null)
            {
                // Configurações gerais - compatível com todas versões
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.spritePixelsToUnits = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.maxTextureSize = 2048;
                importer.compressionQuality = 100;
                
                // Configurações para Default (Desktop)
                TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
                defaultSettings.maxTextureSize = 2048;
                defaultSettings.format = TextureImporterFormat.RGBA32;
                defaultSettings.compressionQuality = 100;
                importer.SetPlatformTextureSettings(defaultSettings);
                
                // Configurações específicas para Android
                TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
                androidSettings.overridden = true;
                androidSettings.maxTextureSize = 2048;
                androidSettings.format = TextureImporterFormat.RGBA32;
                androidSettings.compressionQuality = 100;
                importer.SetPlatformTextureSettings(androidSettings);
                
                // Aplicar mudanças
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                fixedCount++;
                
                // Atualizar barra de progresso
                float progress = (float)i / guids.Length;
                EditorUtility.DisplayProgressBar("Corrigindo Texturas", 
                    $"Processando: {Path.GetFileName(path)} ({i+1}/{guids.Length})", progress);
                
                Debug.Log($"✅ Fixed: {Path.GetFileName(path)}");
            }
        }
        
        EditorUtility.ClearProgressBar();
        
        Debug.Log($"🎯 CONCLUÍDO! Corrigidas {fixedCount} texturas para qualidade mobile!");
        
        // Mostrar mensagem de sucesso
        EditorUtility.DisplayDialog("Texturas Corrigidas!", 
            $"✅ {fixedCount} texturas foram otimizadas para mobile!\n\n" +
            "Agora faça o build novamente para testar no celular.", "OK");
        
        AssetDatabase.Refresh();
    }
    
    static void ApplyQualitySettings()
    {
        // Configurar qualidade para Mobile
        QualitySettings.SetQualityLevel(3); // High Quality
        
        // Configurações específicas para melhor qualidade
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        QualitySettings.antiAliasing = 2; // 2x MSAA
        
        Debug.Log("✅ Quality settings optimized for mobile!");
        
        EditorUtility.DisplayDialog("Configurações Aplicadas!", 
            "✅ Qualidade otimizada para mobile!\n\n" +
            "• Quality Level: High\n" +
            "• Anti-Aliasing: 2x MSAA\n" +
            "• Anisotropic Filtering: Ativo", "OK");
    }
    
    static void ShowHelpInfo()
    {
        EditorUtility.DisplayDialog("Como Otimizar para Mobile", 
            "🎯 PASSOS PARA CORRIGIR PIXELIZAÇÃO:\n\n" +
            "1️⃣ Fix Mobile Texture Quality\n" +
            "2️⃣ Quick Quality Settings\n" +
            "3️⃣ Build Settings → Build para Android\n" +
            "4️⃣ Teste no celular\n\n" +
            "✅ Suas imagens ficarão nítidas!", "Entendi!");
    }
}
#endif 