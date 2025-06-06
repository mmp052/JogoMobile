using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public Sprite somLigadoSprite;
    public Sprite somDesligadoSprite;
    private Button botao;
    private Image imagemBotao;
    private bool somAtivado = true;

    void Start()
    {
        botao = GetComponent<Button>();
        imagemBotao = GetComponent<Image>();

        botao.onClick.AddListener(TrocarEstadoSom);
        AtualizarIcone();
    }

    void TrocarEstadoSom()
    {
        somAtivado = !somAtivado;

        AudioListener.volume = somAtivado ? 1 : 0;

        AtualizarIcone();
    }

    void AtualizarIcone()
    {
        imagemBotao.sprite = somAtivado ? somLigadoSprite : somDesligadoSprite;
    }
}
