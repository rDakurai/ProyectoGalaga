using UnityEngine;

public class RocketMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 12f;

    [Header("Daño")]
    [SerializeField] private int damage = 1;
    public int Damage => damage;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }
}

