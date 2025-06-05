using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public Enemy enemy;
    public Transform fill; // o SpriteRenderer verde
    private float initialScaleX;
    private Vector3 initialPosition;

    void Start()
    {
        initialScaleX = fill.localScale.x;
        initialPosition = fill.localPosition;
    }

    void Update()
    {
        if (enemy != null && fill != null)
        {
            float percent = Mathf.Clamp01((float)enemy.CurrentHealth / enemy.MaxHealth);

            // Redimensiona a escala
            Vector3 scale = fill.localScale;
            scale.x = initialScaleX * percent;
            fill.localScale = scale;

            // Reposiciona para manter a esquerda fixa
            Vector3 pos = fill.localPosition;
            pos.x = initialPosition.x - (initialScaleX - scale.x) / 2f;
            fill.localPosition = pos;
        }
    }
}
