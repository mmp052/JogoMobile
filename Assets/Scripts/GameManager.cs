using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public bool isGameOver = false;
    
    [Header("Scene Names")]
    public string gameOverSceneName = "GameOverScene"; // Nome da scene de Game Over
    public string gameSceneName = "SampleScene"; // Nome da scene do jogo (para restart)
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void GameOver()
    {
        if (isGameOver) return; // Evitar múltiplas chamadas
        
        isGameOver = true;
        Debug.Log("💀 GAME OVER!");
        
        // Carregar scene de Game Over
        LoadGameOverScene();
    }
    
    void LoadGameOverScene()
    {
        // Garantir que o tempo está normal antes de trocar de scene
        Time.timeScale = 1f;
        
        // Carregar scene de Game Over
        SceneManager.LoadScene(gameOverSceneName);
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f; // Garantir que o tempo está normal
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 