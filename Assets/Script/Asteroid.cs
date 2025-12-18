using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Vector2 direction = Vector2.left;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Rotación")]
    [SerializeField] private float rotateSpeed = 120f;

    [Header("Flama")]
    [SerializeField] private Transform flame;
    [SerializeField] private Vector3 flameOffset = new Vector3(0.6f, 0f, 0f);

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 10f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    private void Start()
    {
        if (lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        // Movimiento
        rb.linearVelocity = direction.normalized * moveSpeed;

        // Rotación del asteroide
        rb.MoveRotation(rb.rotation + rotateSpeed * Time.fixedDeltaTime);

        // Flama
        UpdateFlame();
    }

    private void UpdateFlame()
    {
        if (flame == null) return;

        flame.position = transform.position - (Vector3)direction.normalized * flameOffset.magnitude;

        flame.rotation = Quaternion.identity;
    }
}
