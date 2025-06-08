using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AdSimulator : MonoBehaviour
{
    public GameObject adPanel; // Painel com a imagem do anúncio

    public void ShowAd()
    {
        StartCoroutine(ShowAdCoroutine());
    }

    private IEnumerator ShowAdCoroutine()
    {
        adPanel.SetActive(true);      // Mostrar o anúncio
        Time.timeScale = 0f;          // Pausar o jogo

        float waitTime = 5f;
        float counter = 0f;

        while (counter < waitTime)
        {
            counter += Time.unscaledDeltaTime;  // Usar tempo "real" (sem ser afetado por timeScale)
            yield return null;
        }

        adPanel.SetActive(false);     // Esconder o anúncio
        Time.timeScale = 1f;          // Retomar o jogo
    }
}
