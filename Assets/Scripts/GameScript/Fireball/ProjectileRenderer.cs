using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProjectileRenderer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSortingOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }
}