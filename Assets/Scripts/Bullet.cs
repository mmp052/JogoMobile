using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // ou outro valor de dano
                
                // Notificar GameManager sobre morte se o inimigo morreu
                if (enemy.CurrentHealth <= 0)
                {
                    CoinManager.Instance?.OnEnemyKilled();
                }
            }

            Destroy(gameObject); // destrói a bala
        }
    }
}