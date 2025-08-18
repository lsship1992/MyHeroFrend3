using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float arcHeight = 1f;
    public float speed = 5f;

    public void FireAtTarget(Transform target)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        Projectile projScript = projectile.GetComponent<Projectile>();
        projScript.SetTarget(target);
        // ������������� ��������� ����� ��������� ��������
        projScript.GetComponent<Projectile>().speed = speed;
        // ... ������ ���������
    }
}