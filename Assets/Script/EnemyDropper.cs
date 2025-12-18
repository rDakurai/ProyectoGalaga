using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DropItem
{
    [Header("Item a Dropear")]
    public GameObject itemPrefab;
    
    [Header("Probabilidad")]
    [Range(0f, 100f)]
    public float dropChancePercent = 50f;

    [Header("Tiempo de Vida")]
    [Min(0f)]
    public float lifetime = 5f; // Tiempo antes de desaparecer (0 = infinito)
}

public class EnemyDropper : MonoBehaviour
{
    [Header("Items a Dropear")]
    [SerializeField]
    private List<DropItem> dropItems = new List<DropItem>();

    [Header("Física de Caída")]
    [SerializeField, Min(0f)]
    private float fallSpeedX = 3f; // velocidad hacia la izquierda

    [SerializeField, Min(0f)]
    private float fallSpeedY = 2f; // velocidad hacia abajo

    public void DropItems()
    {
        if (dropItems == null || dropItems.Count == 0) return;

        // Calcular suma total de probabilidades
        float totalProbability = 0f;
        foreach (var item in dropItems)
        {
            totalProbability += item.dropChancePercent;
        }

        if (totalProbability <= 0f) return;

        // Hacer roll en el pool de probabilidades
        float roll = UnityEngine.Random.Range(0f, totalProbability);
        float accumulated = 0f;

        // Encontrar qué item corresponde al roll
        foreach (var dropItem in dropItems)
        {
            accumulated += dropItem.dropChancePercent;
            if (roll < accumulated)
            {
                // Este es el item que cae
                if (dropItem.itemPrefab != null)
                {
                    InstantiateDroppedItem(dropItem.itemPrefab, dropItem.lifetime);
                }
                return; // Solo un item por muerte
            }
        }
    }

    private void InstantiateDroppedItem(GameObject itemPrefab, float itemLifetime)
    {
        Vector3 spawnPos = transform.position;
        GameObject droppedItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);

        // Aplicar física de caída hacia la izquierda
        Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Velocity hacia la izquierda y hacia abajo
            rb.linearVelocity = new Vector2(-fallSpeedX, -fallSpeedY);
            rb.gravityScale = 0f; // Sin gravedad, lo controlamos nosotros
        }

        // Auto-destruir después del lifetime del item (0 = infinito)
        if (itemLifetime > 0f)
        {
            Destroy(droppedItem, itemLifetime);
        }
    }
}
