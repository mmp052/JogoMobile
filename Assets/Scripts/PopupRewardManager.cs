using UnityEngine;
using UnityEngine.UI;

public class PopupRewardManager : MonoBehaviour
{
    [Header("Auto Configuration")]
    public bool autoFindComponents = true;
    
    [Header("Components")]
    public PopupGain popupGain;
    public AdSimulator adSimulator;
    public Button doubleRewardButton;
    
    void Awake()
    {
        // Primeiro garantir que o sistema de anúncios existe
        AutoAdSetup autoSetup = FindObjectOfType<AutoAdSetup>();
        if (autoSetup == null)
        {
            // Criar AutoAdSetup temporário para configurar o sistema
            GameObject tempObject = new GameObject("TempAutoAdSetup");
            autoSetup = tempObject.AddComponent<AutoAdSetup>();
            autoSetup.SetupAdSystem();
            Destroy(tempObject);
        }
        
        if (autoFindComponents)
        {
            AutoConfigureComponents();
        }
    }
    
    void AutoConfigureComponents()
    {
        // Encontrar PopupGain
        if (popupGain == null)
        {
            popupGain = GetComponent<PopupGain>();
            if (popupGain == null)
            {
                popupGain = GetComponentInChildren<PopupGain>();
            }
        }
        
        // Encontrar AdSimulator na scene
        if (adSimulator == null)
        {
            adSimulator = FindObjectOfType<AdSimulator>();
        }
        
        // Encontrar botão 2X
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
        
        // Configurar PopupGain se encontrado
        if (popupGain != null && adSimulator != null)
        {
            popupGain.adSimulator = adSimulator;
            popupGain.doubleRewardButton = doubleRewardButton;
            Debug.Log("PopupRewardManager: Componentes configurados automaticamente!");
        }
        
        // Configurar botão
        if (doubleRewardButton != null && popupGain != null)
        {
            doubleRewardButton.onClick.RemoveAllListeners();
            doubleRewardButton.onClick.AddListener(popupGain.WatchAdToDoubleReward);
            Debug.Log("PopupRewardManager: Botão 2X configurado!");
        }
    }
    
    // Método público para configurar manualmente
    public void ConfigureManually()
    {
        AutoConfigureComponents();
    }
    
    // Método simples para testar dobrar recompensa (sem anúncio)
    public void TestDoubleRewardNow()
    {
        if (popupGain != null)
        {
            popupGain.TestDoubleReward();
        }
        else
        {
            Debug.LogError("PopupRewardManager: PopupGain não encontrado!");
        }
    }
} 