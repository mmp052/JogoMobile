using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 1.0f;
    private float fireTimer;

    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    void Fire()
    {
        Instantiate(bulletPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
    }
}
