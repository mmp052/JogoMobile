using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 1f;
    public AudioClip shootSound; // Som de tiro específico (opcional)
    
    private bool hasBeenFired = false; // Controla se a bala foi disparada

    void OnEnable()
    {
        // Só toca som se a bala foi realmente disparada (não quando criada no pool)
        if (hasBeenFired && AudioManager.Instance != null)
        {
            if (shootSound != null)
            {
                // Se tem som específico, usa ele
                AudioManager.Instance.PlaySoundEffect(shootSound);
            }
            else
            {
                // Senão usa o som padrão
                AudioManager.Instance.PlayShootSound();
            }
        }
        hasBeenFired = true; // Marca que foi ativada
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        // Se sair da tela, volta pro pool
        if (transform.position.y > 10f || transform.position.y < -7f ||
            transform.position.x > 10f || transform.position.x < -10f)
        {
            BulletPoolManager.Instance.ReturnBullet(gameObject);
        }
    }

    public void ResetBullet()
    {
        // Reseta o estado quando volta pro pool
        hasBeenFired = false;
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

            BulletPoolManager.Instance.ReturnBullet(gameObject); // devolve a bala ao pool
        }
    }
}