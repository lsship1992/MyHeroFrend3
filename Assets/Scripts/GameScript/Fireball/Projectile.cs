using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float arcHeight = 1f;
    public int damage = 10;

    private Transform target;
    private Vector3 startPosition;

    public IEnumerable<object> Renderers { get; internal set; }
    private ProjectileRenderer projectileRenderer;

    void Start()
    {
        projectileRenderer = gameObject.AddComponent<ProjectileRenderer>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Движение по дуге
        float distance = Vector3.Distance(startPosition, target.position);
        float x0 = startPosition.x;
        float x1 = target.position.x;
        float fracComplete = (Time.time * speed) / distance;

        float currentX = Mathf.Lerp(x0, x1, fracComplete);
        float currentY = Mathf.Lerp(startPosition.y, target.position.y, fracComplete) + arcHeight * Mathf.Sin(fracComplete * Mathf.PI);

        transform.position = new Vector3(currentX, currentY, 0);

        if (fracComplete >= 1)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (target.CompareTag("Enemy"))
            target.GetComponent<EnemyHealth>().TakeDamage(damage); // Меняем Enemy на EnemyHealth
        else if (target.CompareTag("Player"))
            target.GetComponent<PlayerHealth>().TakeDamage(damage); // Меняем Player на PlayerHealth
        Destroy(gameObject);
    }
    // Метод для установки порядка отрисовки
    public void SetSortingOrder(int order)
    {
        if (projectileRenderer != null)
        {
            projectileRenderer.SetSortingOrder(order);
        }
    }
    // Упрощаем SetTarget
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}