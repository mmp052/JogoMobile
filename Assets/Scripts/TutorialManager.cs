using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // Referência ao painel de tutorial
    public GameObject tutorialPanel;

    // Alterna a visibilidade do painel de tutorial
    public void ToggleTutorialPanel()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(!tutorialPanel.activeSelf);
        }
    }
}
