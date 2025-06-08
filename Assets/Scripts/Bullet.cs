using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 1f;

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
                enemy.TakeDamage(Mathf.RoundToInt(damage)); // converte o dano para int
                
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