using UnityEngine;

public class FireballController : MonoBehaviour
{
    public bool isPlayerProjectile;
    public int damage;
    public float speed = 10f;

    public void Initialize(bool isPlayerProjectile, int damage)
    {
        this.isPlayerProjectile = isPlayerProjectile;
        this.damage = damage;
        DebugLogger.Log($"Fireball initialized - Player: {isPlayerProjectile}, Damage: {damage}");
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerProjectile && collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyHealth>().TakeDamage(damage);
        }
        else if (!isPlayerProjectile && collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}