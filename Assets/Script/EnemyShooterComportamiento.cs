using UnityEngine;
using System.Collections;

public class EnemyShooterComportamiento : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField]
    private Vector2 targetPosition;

    [SerializeField, Min(0f)]
    private float moveSpeed = 3f;

    [SerializeField, Min(0f)]
    private float arriveThreshold = 0.1f;

    [Header("Disparo")]
    [SerializeField]
    private GameObject enemyBulletPrefab;

    [SerializeField, Min(0f)]
    private float bulletSpeed = 12f;

    [SerializeField, Min(0f)]
    private float attackCooldown = 1.5f;

    [Header("Patrón Arco")]
    [SerializeField, Min(1)]
    private int arcBulletCount = 8; // configurable

    [SerializeField, Min(0f)]
    private float arcSpreadDegrees = 40f; // total spread around la izquierda

    [Header("Patrón Central")]
    [SerializeField, Min(1)]
    private int centralShotsCount = 3; // configurable

    [SerializeField, Min(0f)]
    private float centralShotInterval = 0.15f; // tiempo entre disparos del patrón central

    [Header("Movimiento Vertical")]
    [SerializeField]
    private bool verticalStrafe = true;

    [SerializeField, Min(0f)]
    private float verticalSpeed = 1.2f; // unidades por segundo

    [SerializeField, Min(0f)]
    private float verticalAmplitude = 1.0f; // desplazamiento máximo arriba/abajo

    [Header("Evitar superposición con otros Shooters")]
    [SerializeField]
    private bool avoidOtherShooters = true;

    [SerializeField, Min(0f)]
    private float separationRadius = 0.6f;

    [SerializeField, Min(0f)]
    private float separationWeight = 1.0f;

    private bool _arrived;
    private Coroutine _aiRoutine;
    private Coroutine _verticalRoutine;

    private void OnEnable()
    {
        if (_aiRoutine == null)
        {
            _aiRoutine = StartCoroutine(AIRoutine());
        }
    }

    private void OnDisable()
    {
        if (_aiRoutine != null)
        {
            StopCoroutine(_aiRoutine);
            _aiRoutine = null;
        }
    }

    private IEnumerator AIRoutine()
    {
        // 1) Mover hasta la posición objetivo
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
            if (avoidOtherShooters)
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

        // 2) Alternar patrones de ataque
        // Inicia movimiento vertical continuo (arriba/abajo) al llegar
        if (verticalStrafe && _verticalRoutine == null)
        {
            _verticalRoutine = StartCoroutine(VerticalStrafeRoutine());
        }

        while (true)
        {
            // Patrón Arco
            FireArc();
            yield return new WaitForSeconds(attackCooldown);

            // Patrón Central consecutivo
            yield return StartCoroutine(FireCentralBurst());
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private void FireArc()
    {
        if (enemyBulletPrefab == null || arcBulletCount <= 0)
        {
            return;
        }

        // Dirección base: izquierda
        Vector2 baseDir = Vector2.left;

        // Distribuir arcBulletCount a lo largo de un arco simétrico
        float halfSpread = arcSpreadDegrees * 0.5f;
        for (int i = 0; i < arcBulletCount; i++)
        {
            float t = (arcBulletCount == 1) ? 0f : (i / (float)(arcBulletCount - 1));
            float angle = Mathf.Lerp(-halfSpread, halfSpread, t);
            Vector2 dir = Rotate(baseDir, angle).normalized;
            SpawnEnemyBullet(transform.position, dir);
        }
    }

    private IEnumerator FireCentralBurst()
    {
        if (enemyBulletPrefab == null || centralShotsCount <= 0)
        {
            yield break;
        }

        for (int i = 0; i < centralShotsCount; i++)
        {
            SpawnEnemyBullet(transform.position, Vector2.left);
            if (i < centralShotsCount - 1 && centralShotInterval > 0f)
            {
                yield return new WaitForSeconds(centralShotInterval);
            }
        }
    }

    private void SpawnEnemyBullet(Vector3 position, Vector2 direction)
    {
        GameObject go = Instantiate(enemyBulletPrefab, position, Quaternion.identity);
        EnemyBulletComportamiento bullet = go.GetComponent<EnemyBulletComportamiento>();
        if (bullet != null)
        {
            bullet.SetSpeed(bulletSpeed);
            bullet.SetDirection(direction);
        }
        // Asegura el tag correcto si el prefab no lo trae (opcional)
        if (!go.CompareTag("EnemyBullet"))
        {
            go.tag = "EnemyBullet";
        }
    }

    private static Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    private IEnumerator VerticalStrafeRoutine()
    {
        float baseY = transform.position.y;
        float dir = 1f; // 1 = arriba, -1 = abajo
        float offset = 0f;

        while (true)
        {
            float step = verticalSpeed * Time.deltaTime * dir;
            offset += step;

            // Invertir dirección al alcanzar amplitud
            if (Mathf.Abs(offset) >= verticalAmplitude)
            {
                offset = Mathf.Sign(offset) * verticalAmplitude;
                dir *= -1f;
            }

            Vector2 sep = Vector2.zero;
            if (avoidOtherShooters)
            {
                sep = ComputeSeparationVector();
            }

            Vector3 p = transform.position;
            // aplicar separación en X
            p.x += sep.x * separationWeight * Time.deltaTime;
            // mantener movimiento vertical base + separación en Y
            p.y = baseY + offset + sep.y * separationWeight * Time.deltaTime;
            transform.position = p;

            yield return null;
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
            if (col == null) continue;

            var other = col.GetComponent<EnemyShooterComportamiento>();
            if (other == null || other == this) continue;

            Vector2 toSelf = (Vector2)selfPos - (Vector2)col.transform.position;
            float d = toSelf.magnitude;
            if (d > 0.0001f)
            {
                repel += toSelf / (d * d);
            }
        }

        if (repel == Vector2.zero) return Vector2.zero;
        return repel.normalized;
    }
}
