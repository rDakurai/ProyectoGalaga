using UnityEngine;

public class BulletComportamiento : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField, Min(0f)]
    private float speed = 20f;

    [SerializeField]
    private Vector2 direction = Vector2.up;

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
        // Solo procesar si esta bala es del jugador
        if (!gameObject.CompareTag("PlayerAttack"))
        {
            return;
        }

        // Destruir balas enemigas
        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        // Dañar enemigos
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }
}
