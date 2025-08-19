using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    private void Start()
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
            Die();
        }
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    private void Die()
    {
        if (WaveSystem.Instance != null)
        {
            WaveSystem.Instance.OnPlayerDeath();
        }
        else
        {
            Debug.LogWarning("WaveSystem.Instance is null!");
            // ћожно добавить рестарт уровн€ другим способом
        }
    }
}