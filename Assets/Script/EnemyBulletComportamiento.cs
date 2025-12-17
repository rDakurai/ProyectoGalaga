using UnityEngine;

public class EnemyBulletComportamiento : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField, Min(0f)]
    private float speed = 15f;

    [SerializeField]
    private Vector2 direction = Vector2.left;

    [SerializeField, Min(0f)]
    private float lifetime = 5f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
        }
        _rb.gravityScale = 0f;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Start()
    {
        if (_rb != null)
        {
            _rb.linearVelocity = direction.normalized * speed;
        }
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
        if (_rb != null)
        {
            _rb.linearVelocity = direction.normalized * speed;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        if (_rb != null)
        {
            _rb.linearVelocity = direction.normalized * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo procesar si esta bala es del enemigo
        if (!gameObject.CompareTag("EnemyBullet"))
        {
            return;
        }

        // Si choca con una bala del jugador, se destruye
        if (other.CompareTag("PlayerAttack"))
        {
            Destroy(gameObject);
            return;
        }

        // Si choca con el jugador, aplica daño
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Maneja colisiones físicas (no trigger) como respaldo
        var other = collision.collider;
        if (!gameObject.CompareTag("EnemyBullet"))
        {
            return;
        }

        if (other.CompareTag("PlayerAttack"))
        {
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }
}
