using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public HealthBar healthBar;
    private int currentHealth;

    public void Initialize(int maxHealth)
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            GetComponent<EnemyController>().OnDeath();
        }
    }
}