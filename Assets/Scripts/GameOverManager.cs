using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "SampleScene"; // Nome da scene do jogo
    public string menuSceneName = "MainMenu"; // Nome da scene do menu (se tiver)
    
    [Header("UI References (opcional)")]
    public Button restartButton;
    public Button menuButton;
    public Button quitButton;
    
    void Start()
    {
        // Configurar botões automaticamente se estiverem atribuídos
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMenu);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
            
        // Input de teclado como backup
        Debug.Log("Game Over! Pressione R para reiniciar, M para menu, ou ESC para sair");
    }
    
    void Update()
    {
        // Input de teclado como alternativa aos botões
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            GoToMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    
    public void RestartGame()
    {
        Debug.Log("Reiniciando jogo...");
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void GoToMenu()
    {
        Debug.Log("Voltando ao menu...");
        SceneManager.LoadScene(menuSceneName);
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