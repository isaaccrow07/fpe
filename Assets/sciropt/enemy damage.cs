using UnityEngine;
public class DummyEnemy : MonoBehaviour, IDamageable
{
    public float health = 100;
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Enemy HP: " + health);
        if (health <= 0)
            Destroy(gameObject);
    }
}