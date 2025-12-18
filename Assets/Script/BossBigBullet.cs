using UnityEngine;

public class BossBigBullet : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField, Min(0f)]
    private float initialSpeed = 3f;

    [SerializeField, Min(0f)]
    private float finalSpeed = 15f;

    [SerializeField, Min(0f)]
    private float timeBeforeSpeedChange = 0.8f; // tiempo antes de cambiar velocidad

    [SerializeField, Min(0f)]
    private float accelerationDuration = 0.3f; // duración de la transición (0 = instantáneo)

    [SerializeField]
    private Vector2 direction = Vector2.left;

    [SerializeField, Min(0f)]
    private float lifetime = 8f;

    private Rigidbody2D _rb;
    private float _currentSpeed;
    private float _timeElapsed;
    private bool _hasChangedSpeed;

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
        _currentSpeed = initialSpeed;
        if (_rb != null)
        {
            _rb.linearVelocity = direction.normalized * _currentSpeed;
        }
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 dir, float initSpeed, float finSpeed, float timeBeforeChange = 0.8f, float accelDuration = 0.3f)
    {
        direction = dir;
        initialSpeed = initSpeed;
        finalSpeed = finSpeed;
        timeBeforeSpeedChange = timeBeforeChange;
        accelerationDuration = accelDuration;
        _currentSpeed = initialSpeed;
    }

    private void FixedUpdate()
    {
        _timeElapsed += Time.fixedDeltaTime;

        // Cambiar velocidad después del tiempo especificado
        if (!_hasChangedSpeed && _timeElapsed >= timeBeforeSpeedChange)
        {
            _hasChangedSpeed = true;
        }

        // Si ya es momento de cambiar velocidad
        if (_hasChangedSpeed)
        {
            float transitionTime = _timeElapsed - timeBeforeSpeedChange;

            if (accelerationDuration <= 0f)
            {
                // Cambio instantáneo
                _currentSpeed = finalSpeed;
            }
            else if (transitionTime < accelerationDuration)
            {
                // Transición gradual
                float t = transitionTime / accelerationDuration;
                _currentSpeed = Mathf.Lerp(initialSpeed, finalSpeed, t);
            }
            else
            {
                // Ya completó la transición
                _currentSpeed = finalSpeed;
            }
        }

        if (_rb != null)
        {
            _rb.linearVelocity = direction.normalized * _currentSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si choca con una bala del jugador, destruir la bala del jugador
        if (other.CompareTag("PlayerAttack"))
        {
            Destroy(other.gameObject);
            return;
        }

        // Si choca con el jugador, aplica daño
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(2);
            }
            // BigBullet es indestructible: no se destruye al golpear al jugador
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.collider;

        if (other.CompareTag("PlayerAttack"))
        {
            Destroy(other.gameObject);
            return;
        }

        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(2);
            }
            // BigBullet es indestructible: no se destruye al golpear al jugador
        }
    }
}
