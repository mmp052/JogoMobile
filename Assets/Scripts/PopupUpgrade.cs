using UnityEngine;
using UnityEngine.Video;

public class PopupUpgrade : MonoBehaviour
{
    public VideoPlayer videoPlayer; // arraste o VideoPlayer no Inspector

    public void OnCollect()
    {
        Debug.Log("Iniciando vídeo de upgrade...");
        PlayVideo(); // primeiro toca o vídeo
        gameObject.SetActive(false); // depois esconde o popup
    }


    public void Close()
    {
        Debug.Log("upgrade recusado!");
        gameObject.SetActive(false);
    }

    private void PlayVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true);

            string path = System.IO.Path.Combine(Application.streamingAssetsPath, "upgrade_test.mp4");
            Debug.Log("Caminho do vídeo: " + path);
            videoPlayer.url = path;

            videoPlayer.loopPointReached += OnVideoFinished;

            videoPlayer.Prepare(); // prepara antes de dar play
            videoPlayer.prepareCompleted += (vp) =>
            {
                Debug.Log("Vídeo pronto, tocando...");
                vp.Play();
            };
        }
        else
        {
            Debug.LogError("VideoPlayer não está atribuído.");
        }
    }



    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Vídeo finalizado. Upgrade concedido!");
        vp.loopPointReached -= OnVideoFinished;

        // Aqui você pode chamar uma função de "ConcederUpgrade"
    }
}
