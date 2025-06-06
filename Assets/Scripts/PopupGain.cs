using UnityEngine;
using TMPro;

public class PopupGain : MonoBehaviour
{
    public TextMeshProUGUI gainText;

    public void SetGainText(int amount)
    {
        gainText.text = $"{amount}";
    }

    public void OnCollect()
    {
        // Lógica de coletar o ganho
        Debug.Log("Ganho coletado!");
        gameObject.SetActive(false);
    }
}
