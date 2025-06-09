using UnityEngine;
using UnityEngine.UI;

public class WaveTestHelper : MonoBehaviour
{
    [Header("Botões de Teste")]
    public Button testPopupButton;
    public Button forceCompleteWaveButton;
    public Button addCoinsButton;
    public Button testGameOverButton;
    
    [Header("Configurações de Teste")]
    public int testWaveNumber = 1;
    public int testCoinsAmount = 100;
    
    void Start()
    {
        // Configurar botões automaticamente se existirem
        SetupTestButtons();
    }
    
    void SetupTestButtons()
    {
        if (testPopupButton != null)
        {
            testPopupButton.onClick.AddListener(TestShowPopup);
        }
        
        if (forceCompleteWaveButton != null)
        {
            forceCompleteWaveButton.onClick.AddListener(ForceCompleteCurrentWave);
        }
        
        if (addCoinsButton != null)
        {
            addCoinsButton.onClick.AddListener(AddTestCoins);
        }
        
        if (testGameOverButton != null)
        {
            testGameOverButton.onClick.AddListener(TestGameOver);
        }
    }
    
    [ContextMenu("Testar Popup")]
    public void TestShowPopup()
    {
        // Primeiro tentar encontrar popup já na scene
        PopupGain popupGain = FindObjectOfType<PopupGain>();
        
        if (popupGain != null)
        {
            Debug.Log($"WaveTestHelper: Testando popup existente para Wave {testWaveNumber}");
            popupGain.ShowWaveReward(testWaveNumber);
            return;
        }
        
        // Se não encontrou, tentar usar o WaveManager para instanciar
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            Debug.Log($"WaveTestHelper: Usando WaveManager para mostrar popup para Wave {testWaveNumber}");
            waveManager.TestShowRewardPopup(testWaveNumber);
        }
        else
        {
            Debug.LogError("WaveTestHelper: Nem popup nem WaveManager encontrados! Configure o sistema adequadamente.");
        }
    }
    
    [ContextMenu("Forçar Completar Wave")]
    public void ForceCompleteCurrentWave()
    {
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        
        if (waveManager != null)
        {
            Debug.Log("WaveTestHelper: Forçando completar wave atual...");
            waveManager.ForceNextWave();
        }
        else
        {
            Debug.LogError("WaveTestHelper: WaveManager não encontrado!");
        }
    }
    
    [ContextMenu("Adicionar Moedas")]
    public void AddTestCoins()
    {
        CoinManager coinManager = FindObjectOfType<CoinManager>();
        
        if (coinManager != null)
        {
            Debug.Log($"WaveTestHelper: Adicionando {testCoinsAmount} moedas");
            coinManager.AddCoins(testCoinsAmount);
        }
        else
        {
            Debug.LogError("WaveTestHelper: CoinManager não encontrado!");
        }
    }
    
    [ContextMenu("Testar Game Over")]
    public void TestGameOver()
    {
        Debug.Log("WaveTestHelper: Testando Game Over...");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
        else
        {
            Debug.LogError("WaveTestHelper: GameManager não encontrado!");
        }
    }
    
    [ContextMenu("Testar Sistema Completo")]
    public void TestCompleteSystem()
    {
        Debug.Log("=== TESTANDO SISTEMA COMPLETO ===");
        
        // Verificar componentes
        PopupGain popup = FindObjectOfType<PopupGain>();
        AdSimulator adSim = FindObjectOfType<AdSimulator>();
        AdBlocker adBlocker = FindObjectOfType<AdBlocker>();
        AutoAdSetup autoSetup = FindObjectOfType<AutoAdSetup>();
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        CoinManager coinManager = FindObjectOfType<CoinManager>();
        
        Debug.Log($"PopupGain: {(popup != null ? "✅ OK" : "❌ FALTANDO")}");
        Debug.Log($"AdSimulator: {(adSim != null ? "✅ OK" : "❌ FALTANDO")}");
        Debug.Log($"AdBlocker: {(adBlocker != null ? "✅ OK" : "❌ FALTANDO")}");
        Debug.Log($"AutoAdSetup: {(autoSetup != null ? "✅ OK" : "❌ FALTANDO")}");
        Debug.Log($"WaveManager: {(waveManager != null ? "✅ OK" : "❌ FALTANDO")}");
        Debug.Log($"CoinManager: {(coinManager != null ? "✅ OK" : "❌ FALTANDO")}");
        
        if (popup != null)
        {
            Debug.Log($"PopupGain - AdSimulator: {(popup.adSimulator != null ? "✅ OK" : "❌ FALTANDO")}");
            Debug.Log($"PopupGain - CoinText: {(popup.coinText != null ? "✅ OK" : "❌ FALTANDO")}");
            Debug.Log($"PopupGain - CollectButton: {(popup.collectButton != null ? "✅ OK" : "❌ FALTANDO")}");
            Debug.Log($"PopupGain - DoubleRewardButton: {(popup.doubleRewardButton != null ? "✅ OK" : "❌ FALTANDO")}");
        }
        
        Debug.Log("================================");
        
        // Testar popup se tudo estiver OK
        if (popup != null)
        {
            Debug.Log("Testando popup...");
            TestShowPopup();
        }
    }
    
    void Update()
    {
        // Teclas de atalho para teste
        if (Input.GetKeyDown(KeyCode.P))
        {
            TestShowPopup();
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            ForceCompleteCurrentWave();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddTestCoins();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestCompleteSystem();
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            TestGameOver();
        }
    }
}

// Instruções para uso:
// 1. Adicione este script a qualquer GameObject na scene
// 2. Configure os botões de teste se quiser usar UI
// 3. Use as teclas de atalho:
//    - P: Testar Popup
//    - W: Forçar completar Wave
//    - C: Adicionar moedas
//    - T: Testar sistema completo
//    - G: Testar Game Over
// 4. Ou use o menu de contexto (clique direito no script no Inspector) 