using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public int attackDamage = 1; // Dano que o inimigo causa
    public float attackCooldown = 1f; // Delay entre ataques (1 segundo)

    [Header("Vida")]
    public int maxHealth = 3;
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    
    private static bool barrierExists = true; // Se existe barreira ativa
    private bool isAttackingBarrier = false;
    private bool isAttackingTower = false;
    private Barrier currentBarrier = null;
    private Tower currentTower = null;
    private Coroutine attackCoroutine = null;

    private void Start()
    {
        CurrentHealth = maxHealth;
        
        // Escutar eventos da barreira
        Barrier.OnBarrierDestroyed += OnBarrierDestroyed;
    }
    
    private void OnDestroy()
    {
        // Remover listener
        Barrier.OnBarrierDestroyed -= OnBarrierDestroyed;
        
        // Parar corrotina de ataque se estiver rodando
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }
    
    void OnBarrierDestroyed()
    {
        barrierExists = false;
        isAttackingBarrier = false;
        currentBarrier = null;
        
        // Parar ataque da barreira se estiver atacando
        if (attackCoroutine != null && isAttackingBarrier)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        
        Debug.Log("🚨 Barreira destruída! Inimigos agora podem ir para as torres!");
    }

    private void Update()
    {
        // Só se mover se não estiver atacando nada
        if (!isAttackingBarrier && !isAttackingTower)
        {
            // Mover para baixo
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            
            // Verificar se chegou no fim da tela (Game Over)
            if (transform.position.y < -6f) // Ajuste conforme sua tela
            {
                GameOver();
            }
        }
    }
    
    public void StartAttackingBarrier(Barrier barrier)
    {
        if (isAttackingBarrier) return; // Já está atacando
        
        isAttackingBarrier = true;
        currentBarrier = barrier;
        
        // Iniciar corrotina de ataques contínuos
        attackCoroutine = StartCoroutine(AttackBarrierContinuously());
    }
    
    public void StopAttackingBarrier()
    {
        isAttackingBarrier = false;
        currentBarrier = null;
        
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
    
    public void StartAttackingTower(Tower tower)
    {
        if (isAttackingTower) return; // Já está atacando
        
        isAttackingTower = true;
        currentTower = tower;
        
        // Iniciar corrotina de ataques contínuos à torre
        attackCoroutine = StartCoroutine(AttackTowerContinuously());
    }
    
    public void StopAttackingTower()
    {
        isAttackingTower = false;
        currentTower = null;
        
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
    
    IEnumerator AttackBarrierContinuously()
    {
        while (isAttackingBarrier && currentBarrier != null)
        {
            // Atacar a barreira
            currentBarrier.TakeDamage(attackDamage);
            Debug.Log($"⚔️ Inimigo atacou a barreira! Dano: {attackDamage}");
            
            // Esperar o cooldown
            yield return new WaitForSeconds(attackCooldown);
        }
    }
    
    IEnumerator AttackTowerContinuously()
    {
        while (isAttackingTower && currentTower != null)
        {
            // Atacar a torre
            currentTower.TakeDamage(attackDamage);
            Debug.Log($"⚔️ Inimigo atacou a torre! Dano: {attackDamage}");
            
            // Esperar o cooldown
            yield return new WaitForSeconds(attackCooldown);
        }
    }
    
    // Remoção da lógica antiga - agora torres têm seu próprio OnTriggerEnter2D

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
    
    void GameOver()
    {
        Debug.Log("💀 GAME OVER! Inimigo chegou ao fim!");
        
        // Chamar Game Over do GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        
        Destroy(gameObject);
    }
}