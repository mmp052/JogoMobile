using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("Sistema de Moedas")]
    public int coins = 100;
    public float coinsPerSecond = 1f;
    public int coinsPerKill = 10;
    
    [Header("UI")]
    public TextMeshProUGUI coinsText;
    
    // Singleton
    public static CoinManager Instance { get; private set; }
    
    private float coinTimer = 0f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        UpdateCoinsUI();
        
        // Conectar aos eventos de wave
        WaveManager.OnWaveCompleted += OnWaveCompleted;
        
        Debug.Log("CoinManager: Sistema de moedas iniciado");
    }
    
    void OnDestroy()
    {
        // Desconectar dos eventos
        WaveManager.OnWaveCompleted -= OnWaveCompleted;
    }
    
    void OnWaveCompleted(int waveNumber)
    {
        // Recompensa por completar wave
        int waveReward = 50 + (waveNumber * 10); // 50 + 10 por wave
        AddCoins(waveReward);
        Debug.Log($"CoinManager: Wave {waveNumber} completada! +{waveReward} moedas de recompensa");
    }
    
    void Update()
    {
        // Ganho passivo a cada segundo
        coinTimer += Time.deltaTime;
        if (coinTimer >= 1f)
        {
            AddCoins(Mathf.FloorToInt(coinsPerSecond));
            coinTimer = 0f;
        }
    }
    
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinsUI();
        Debug.Log($"CoinManager: +{amount} moedas! Total: {coins}");
    }
    
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateCoinsUI();
            Debug.Log($"CoinManager: -{amount} moedas! Total: {coins}");
            return true;
        }
        Debug.Log($"CoinManager: Moedas insuficientes! Tem: {coins}, Precisa: {amount}");
        return false;
    }
    
    public void OnEnemyKilled()
    {
        AddCoins(coinsPerKill);
        Debug.Log($"CoinManager: Inimigo morto! +{coinsPerKill} moedas");
    }
    
    void UpdateCoinsUI()
    {
        if (coinsText != null)
        {
            coinsText.text = FormatCoins(coins);
        }
    }
    
    // Função para formatar números grandes (K, M, B, etc.)
    string FormatCoins(int value)
    {
        if (value >= 1000000000) // Bilhões
        {
            float billions = value / 1000000000f;
            return $"{billions:F1}B".Replace(".0B", "B"); // Remove .0 se for número inteiro
        }
        else if (value >= 1000000) // Milhões
        {
            float millions = value / 1000000f;
            return $"{millions:F1}M".Replace(".0M", "M");
        }
        else if (value >= 1000) // Milhares
        {
            float thousands = value / 1000f;
            return $"{thousands:F1}K".Replace(".0K", "K");
        }
        else
        {
            return value.ToString(); // Números menores que 1000 aparecem normais
        }
    }
    
    public int GetCoins()
    {
        return coins;
    }
} 