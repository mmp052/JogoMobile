using UnityEngine;
using UnityEngine.UI;

public class AutoAdSetup : MonoBehaviour
{
    [Header("Auto Configuration")]
    public bool setupOnAwake = true;
    public bool createAdBlockerIfMissing = true;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    void Awake()
    {
        if (setupOnAwake)
        {
            SetupAdSystem();
        }
    }
    
    public void SetupAdSystem()
    {
        if (showDebugLogs)
            Debug.Log("AutoAdSetup: Iniciando configuração automática do sistema de anúncios...");
        
        // 1. Garantir que existe um AdSimulator na scene
        SetupAdSimulator();
        
        // 2. Garantir que existe um AdBlocker na scene
        if (createAdBlockerIfMissing)
        {
            SetupAdBlocker();
        }
        
        // 3. Configurar todos os PopupGain existentes
        SetupPopupGains();
        
        if (showDebugLogs)
            Debug.Log("AutoAdSetup: Sistema de anúncios configurado com sucesso!");
    }
    
    void SetupAdSimulator()
    {
        AdSimulator adSimulator = FindObjectOfType<AdSimulator>();
        
        if (adSimulator == null)
        {
            // Criar AdSimulator
            GameObject adObject = new GameObject("AdSimulator");
            adSimulator = adObject.AddComponent<AdSimulator>();
            
            if (showDebugLogs)
                Debug.Log("AutoAdSetup: AdSimulator criado automaticamente!");
        }
        
        // AdSimulator simples não precisa de AdBlocker configurado
    }
    
    void SetupAdBlocker()
    {
        AdBlocker adBlocker = FindObjectOfType<AdBlocker>();
        
        if (adBlocker == null)
        {
            // Criar AdBlocker
            GameObject blockerObject = new GameObject("AdBlocker");
            adBlocker = blockerObject.AddComponent<AdBlocker>();
            
            if (showDebugLogs)
                Debug.Log("AutoAdSetup: AdBlocker criado automaticamente!");
        }
    }
    
    void SetupPopupGains()
    {
        PopupGain[] popupGains = FindObjectsOfType<PopupGain>();
        AdSimulator adSimulator = FindObjectOfType<AdSimulator>();
        
        foreach (PopupGain popup in popupGains)
        {
            // Configurar AdSimulator
            if (popup.adSimulator == null && adSimulator != null)
            {
                popup.adSimulator = adSimulator;
                
                if (showDebugLogs)
                    Debug.Log($"AutoAdSetup: AdSimulator configurado para {popup.name}");
            }
            
            // Procurar botão 2X no popup
            if (popup.doubleRewardButton == null)
            {
                Button[] buttons = popup.GetComponentsInChildren<Button>();
                foreach (Button btn in buttons)
                {
                    if (btn.name.Contains("2X") || btn.name.Contains("Double") || 
                        btn.name.Contains("Ad") || btn.name.Contains("x2"))
                    {
                        popup.doubleRewardButton = btn;
                        
                        // Configurar evento do botão
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(popup.WatchAdToDoubleReward);
                        
                        if (showDebugLogs)
                            Debug.Log($"AutoAdSetup: Botão 2X configurado para {popup.name}");
                        break;
                    }
                }
            }
        }
    }
    
    // Método para encontrar ou criar painel de bloqueio por nome
    GameObject FindOrCreateAdPanel(string panelName)
    {
        // Procurar painel existente
        GameObject panel = GameObject.Find(panelName);
        
        if (panel == null)
        {
            // Criar painel
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                panel = new GameObject(panelName);
                panel.transform.SetParent(canvas.transform, false);
                
                // Configurar como painel full screen
                RectTransform rectTransform = panel.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                
                // Adicionar Image transparente
                Image image = panel.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0.01f);
                
                panel.SetActive(false);
                
                if (showDebugLogs)
                    Debug.Log($"AutoAdSetup: Painel {panelName} criado automaticamente!");
            }
        }
        
        return panel;
    }
    
    // Método público para reconfigurar tudo
    public void ReconfigureAll()
    {
        SetupAdSystem();
    }
    
    // Método para teste
    public void TestConfiguration()
    {
        AdSimulator adSim = FindObjectOfType<AdSimulator>();
        AdBlocker adBlocker = FindObjectOfType<AdBlocker>();
        PopupGain[] popups = FindObjectsOfType<PopupGain>();
        
        Debug.Log($"=== TESTE DE CONFIGURAÇÃO ===");
        Debug.Log($"AdSimulator encontrado: {(adSim != null ? "SIM" : "NÃO")}");
        Debug.Log($"AdBlocker encontrado: {(adBlocker != null ? "SIM" : "NÃO")}");
        Debug.Log($"PopupGains encontrados: {popups.Length}");
        
        foreach (PopupGain popup in popups)
        {
            Debug.Log($"- {popup.name}: AdSimulator={popup.adSimulator != null}, Botão2X={popup.doubleRewardButton != null}");
        }
        Debug.Log($"=============================");
    }
} 