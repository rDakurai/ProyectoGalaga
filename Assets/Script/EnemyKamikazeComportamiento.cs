using UnityEngine;
using System.Collections;

public class EnemyKamikazeComportamiento : MonoBehaviour
{
    [Header("Movimiento inicial")]
    [SerializeField]
    private Vector2 targetPosition;

    [SerializeField, Min(0f)]
    private float moveSpeed = 3.5f;

    [SerializeField, Min(0f)]
    private float arriveThreshold = 0.15f;

    [Header("Ataque kamikaze")]
    [SerializeField]
    private Transform playerTransform; // opcional; si es null se busca por tag "Player"

    [SerializeField, Min(0f)]
    private float restDuration = 0.6f; // pausa antes de embestir

    [SerializeField, Min(0f)]
    private float rushSpeed = 12f; // velocidad de embestida

    [SerializeField, Min(0f)]
    private float rushLifetime = 5f; // tiempo máximo antes de autodestruirse

    [Header("Evitar superposición con otros Kamikaze")]
    [SerializeField]
    private bool avoidOtherKamikaze = true;

    [SerializeField, Min(0f)]
    private float separationRadius = 0.6f;

    [SerializeField, Min(0f)]
    private float separationWeight = 1.2f;

    private Rigidbody2D _rb;
    private Coroutine _routine;
    private bool _arrived;
    private EnemyDropper _dropper;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
        }
        _rb.gravityScale = 0f;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        _dropper = GetComponent<EnemyDropper>();
    }

    private void OnEnable()
    {
        if (_routine == null)
        {
            _routine = StartCoroutine(RunBehavior());
        }
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    private IEnumerator RunBehavior()
    {
        // Fase 1: mover hasta la posición objetivo
        while (!_arrived)
        {
            Vector2 pos = transform.position;
            Vector2 dir = (targetPosition - pos);
            float dist = dir.magnitude;
            if (dist <= arriveThreshold)
            {
                _arrived = true;
                break;
            }

            Vector2 moveDir = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.zero;

            if (avoidOtherKamikaze)
            {
                Vector2 sep = ComputeSeparationVector();
                if (sep != Vector2.zero)
                {
                    moveDir = (moveDir + sep * separationWeight).normalized;
                }
            }

            Vector2 step = moveDir * moveSpeed * Time.deltaTime;
            transform.position = pos + step;
            yield return null;
        }

        // Fase 2: descansar
        if (restDuration > 0f)
        {
            yield return new WaitForSeconds(restDuration);
        }

        // Reproducir sonido antes de embestir
        EnemyAudioManager.PlayKamikazeRush();

        // Fase 3: embestir hacia la ubicación actual del jugador
        Vector2 targetRushPos = GetPlayerPosition();
        Vector2 rushDir = (targetRushPos - (Vector2)transform.position).normalized;

        float timer = 0f;
        while (rushLifetime <= 0f || timer < rushLifetime)
        {
            Vector2 finalDir = rushDir;
            if (avoidOtherKamikaze)
            {
                Vector2 sep = ComputeSeparationVector();
                if (sep != Vector2.zero)
                {
                    finalDir = (finalDir + sep * separationWeight).normalized;
                }
            }

            _rb.linearVelocity = finalDir * rushSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        // Si aún existe, destruir para evitar objetos eternos
        if (gameObject != null)
        {
            // No dropear items por timeout, solo cuando lo mate el jugador
            Destroy(gameObject);
        }
    }

    private static readonly Collider2D[] _overlapResults = new Collider2D[16];

    private Vector2 ComputeSeparationVector()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, separationRadius, _overlapResults);
        if (count <= 0) return Vector2.zero;

        Vector2 repel = Vector2.zero;
        Vector2 selfPos = transform.position;

        for (int i = 0; i < count; i++)
        {
            var col = _overlapResults[i];
            if (col == null || col.attachedRigidbody == _rb) continue;

            var otherKamikaze = col.GetComponent<EnemyKamikazeComportamiento>();
            if (otherKamikaze == null) continue;

            Vector2 toSelf = (Vector2)selfPos - (Vector2)col.transform.position;
            float d = toSelf.magnitude;
            if (d > 0.0001f)
            {
                // Más repulsión cuanto más cerca
                repel += toSelf / (d * d);
            }
        }

        if (repel == Vector2.zero) return Vector2.zero;
        return repel.normalized;
    }

    private Vector2 GetPlayerPosition()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        return playerTransform != null ? (Vector2)playerTransform.position : (Vector2)transform.position + Vector2.left;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Daño al jugador si colisiona durante la embestida
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(1);
            }
            // Al chocar, usar EnemyHealth para reproducir animación (sin dropear)
            EnemyHealth health = GetComponent<EnemyHealth>();
            if (health != null)
            {
                // Desactivar dropper temporalmente para que no dropee en colisión
                if (_dropper != null)
                {
                    _dropper.enabled = false;
                }
                health.Die();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
