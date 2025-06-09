using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PopupGain : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI coinText;
    public Button collectButton;
    public Button doubleRewardButton;
    
    [Header("Ad System")]
    public AdSimulator adSimulator;
    
    [Header("Animation")]
    public float showDuration = 0.3f;
    
    private int currentReward = 0;
    private int originalReward = 0;
    private bool hasDoubledReward = false;
    
    void Start()
    {
        // Auto-configuração se componentes não estiverem definidos
        AutoConfigureComponents();
        
        // Configurar botões
        SetupButtons();
        
        // Registrar para detectar mudanças de cena
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Se o popup não tem recompensa definida, ocultar inicialmente
        if (currentReward <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    
    void OnDestroy()
    {
        // Remover listener ao destruir
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Se uma nova cena foi carregada, destruir este popup
        Debug.Log($"PopupGain: Nova cena carregada ({scene.name}), destruindo popup da cena anterior");
        
        // Garantir que o jogo não fique pausado
        Time.timeScale = 1f;
        
        // Destruir este GameObject
        Destroy(gameObject);
    }
    
    void AutoConfigureComponents()
    {
        // Encontrar AdSimulator se não definido
        if (adSimulator == null)
        {
            adSimulator = FindObjectOfType<AdSimulator>();
        }
        
        // Encontrar componentes de UI se não definidos
        if (coinText == null)
        {
            coinText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        if (collectButton == null)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                if (btn.name.Contains("Collect") || btn.name.Contains("Coletar"))
                {
                    collectButton = btn;
                    break;
                }
            }
        }
        
        if (doubleRewardButton == null)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                if (btn.name.Contains("2X") || btn.name.Contains("Double") || btn.name.Contains("Ad"))
                {
                    doubleRewardButton = btn;
                    break;
                }
            }
        }
    }
    
    void SetupButtons()
    {
        // Configurar botão de coletar
        if (collectButton != null)
        {
            collectButton.onClick.RemoveAllListeners();
            collectButton.onClick.AddListener(CollectReward);
        }
        
        // Configurar botão de dobrar recompensa
        if (doubleRewardButton != null)
        {
            doubleRewardButton.onClick.RemoveAllListeners();
            doubleRewardButton.onClick.AddListener(WatchAdToDoubleReward);
        }
    }
    
    // Método para mostrar recompensa da wave
    public void ShowWaveReward(int waveNumber)
    {
        Debug.Log($"PopupGain.ShowWaveReward: Chamado para wave {waveNumber}");
        int reward = 100 * waveNumber;
        ShowReward(reward, $"Wave {waveNumber} Concluída!");
    }
    
    // Método genérico para mostrar recompensa
    public void ShowReward(int amount, string title = "Recompensa!")
    {
        Debug.Log($"PopupGain.ShowReward: Iniciando com {amount} moedas, título: {title}");
        
        currentReward = amount;
        originalReward = amount;
        hasDoubledReward = false;
        
        // Atualizar UI
        UpdateCoinDisplay();
        
        // Ativar popup
        Debug.Log($"PopupGain.ShowReward: Ativando gameObject. Ativo antes: {gameObject.activeInHierarchy}");
        gameObject.SetActive(true);
        Debug.Log($"PopupGain.ShowReward: GameObject ativo depois: {gameObject.activeInHierarchy}");
        
        // Pausar o jogo
        Time.timeScale = 0f;
        Debug.Log($"PopupGain.ShowReward: Jogo pausado. TimeScale: {Time.timeScale}");
        
        // Animação de entrada
        StartCoroutine(ShowAnimation());
        
        Debug.Log($"PopupGain.ShowReward: Popup configurado e animação iniciada");
    }
    
    void UpdateCoinDisplay()
    {
        if (coinText != null)
        {
            coinText.text = currentReward.ToString();
        }
    }
    
    public void CollectReward()
    {
        // Adicionar moedas ao CoinManager
        CoinManager coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null)
        {
            coinManager.AddCoins(currentReward);
            Debug.Log($"PopupGain: {currentReward} moedas coletadas!");
        }
        
        // Fechar popup
        ClosePopup();
    }
    
    public void WatchAdToDoubleReward()
    {
        if (hasDoubledReward)
        {
            Debug.Log("PopupGain: Recompensa já foi dobrada!");
            return;
        }
        
        if (adSimulator != null)
        {
            Debug.Log("PopupGain: Iniciando anúncio para dobrar recompensa...");
            
            // Desabilitar botão durante anúncio
            if (doubleRewardButton != null)
            {
                doubleRewardButton.interactable = false;
            }
            
            // Mostrar anúncio
            StartCoroutine(WatchAdCoroutine());
        }
        else
        {
            Debug.LogError("PopupGain: AdSimulator não encontrado!");
            // Fallback - dobrar sem anúncio para teste
            TestDoubleReward();
        }
    }
    
    void OnAdCompleted()
    {
        Debug.Log("PopupGain: Anúncio assistido com sucesso! Dobrando recompensa...");
        
        // Dobrar recompensa
        currentReward = originalReward * 2;
        hasDoubledReward = true;
        
        // Atualizar UI
        UpdateCoinDisplay();
        
        // Desabilitar/ocultar botão 2X
        if (doubleRewardButton != null)
        {
            doubleRewardButton.gameObject.SetActive(false);
        }
        
        Debug.Log($"PopupGain: Recompensa dobrada para {currentReward} moedas!");
    }
    
    void OnAdFailed()
    {
        Debug.Log("PopupGain: Anúncio falhou ou foi cancelado");
        
        // Desbloquear UI se estiver bloqueada
        AdBlocker adBlocker = FindObjectOfType<AdBlocker>();
        if (adBlocker != null)
        {
            adBlocker.UnblockUI();
        }
        
        // Reabilitar botão
        if (doubleRewardButton != null)
        {
            doubleRewardButton.interactable = true;
        }
    }
    
    // Método para teste (dobra sem anúncio)
    public void TestDoubleReward()
    {
        if (!hasDoubledReward)
        {
            OnAdCompleted();
        }
    }
    
    void ClosePopup()
    {
        // Despausar o jogo
        Time.timeScale = 1f;
        
        // Desativar popup
        gameObject.SetActive(false);
        
        Debug.Log("PopupGain: Popup fechado");
    }
    
    IEnumerator ShowAnimation()
    {
        Debug.Log($"PopupGain.ShowAnimation: Iniciando animação");
        
        Transform popupTransform = transform;
        Vector3 originalScale = popupTransform.localScale;
        Debug.Log($"PopupGain.ShowAnimation: Scale original: {originalScale}");
        
        popupTransform.localScale = Vector3.zero;
        Debug.Log($"PopupGain.ShowAnimation: Scale zerado, iniciando animação de {showDuration} segundos");
        
        float elapsed = 0;
        while (elapsed < showDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / showDuration;
            
            popupTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, progress);
            
            yield return null;
        }
        
        popupTransform.localScale = originalScale;
        Debug.Log($"PopupGain.ShowAnimation: Animação concluída. Scale final: {popupTransform.localScale}");
    }
    
    IEnumerator WatchAdCoroutine()
    {
        if (adSimulator != null)
        {
            // Encontrar ou criar AdBlocker para bloquear UI
            AdBlocker adBlocker = FindObjectOfType<AdBlocker>();
            if (adBlocker == null)
            {
                // Criar AdBlocker temporário se não existir
                GameObject blocker = new GameObject("TempAdBlocker");
                adBlocker = blocker.AddComponent<AdBlocker>();
            }
            
            Debug.Log("PopupGain: Bloqueando UI para anúncio...");
            
            // Bloquear toda a UI
            adBlocker.BlockUI();
            
            // Iniciar o anúncio
            adSimulator.ShowAd();
            
            // Aguardar 5 segundos (duração do anúncio)
            yield return new WaitForSecondsRealtime(5f);
            
            Debug.Log("PopupGain: Anúncio terminado, desbloqueando UI...");
            
            // Desbloquear UI
            adBlocker.UnblockUI();
            
            // Anúncio completado
            OnAdCompleted();
        }
        else
        {
            // Fallback se não houver AdSimulator
            OnAdFailed();
        }
    }
    
    // Método estático para destruir todas as instâncias de popup
    public static void DestroyAllPopups()
    {
        PopupGain[] allPopups = FindObjectsOfType<PopupGain>();
        
        foreach (PopupGain popup in allPopups)
        {
            if (popup != null)
            {
                Debug.Log($"PopupGain.DestroyAllPopups: Destruindo popup {popup.name}");
                
                // Garantir que o jogo não fique pausado
                Time.timeScale = 1f;
                
                // Destruir o popup
                Destroy(popup.gameObject);
            }
        }
        
        Debug.Log($"PopupGain.DestroyAllPopups: {allPopups.Length} popups destruídos");
    }
    
    // Métodos antigos para compatibilidade
    public TextMeshProUGUI gainText => coinText;
    
    public void SetGainText(int amount)
    {
        ShowReward(amount);
    }
    
    public void OnCollect()
    {
        CollectReward();
    }
}
