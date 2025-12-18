using UnityEngine;
using System.Collections;

public class Enemy2bComportamiento : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField]
    private Vector2 initialPosition;

    [SerializeField, Min(0f)]
    private float moveSpeed = 2.5f;

    [SerializeField, Min(0f)]
    private float moveSpeedBetweenAttacks = 1.5f;

    [SerializeField, Min(0f)]
    private float arriveThreshold = 0.1f;

    [Header("Ataques")]
    [SerializeField]
    private GameObject bigBulletPrefab;

    [SerializeField]
    private GameObject normalBulletPrefab;

    [SerializeField, Min(0f)]
    private float attackCooldown = 2.5f;

    [SerializeField, Min(0f)]
    private float bigBulletSpeed = 3f;

    [SerializeField, Min(0f)]
    private float bigBulletSpawnDistance = 1f;

    [SerializeField, Min(0f)]
    private float bigBulletSpawnDelay = 0.2f;

    [SerializeField, Min(0f)]
    private float bigBulletFinalSpeed = 15f;

    [SerializeField, Min(0f)]
    private float bigBulletTimeBeforeChange = 0.8f;

    [SerializeField, Min(0f)]
    private float bigBulletAccelDuration = 0.3f;

    [SerializeField, Min(1)]
    private int rainBulletCount = 12;

    [SerializeField, Min(0f)]
    private float rainInterval = 0.1f;

    [SerializeField, Min(0f)]
    private float rainBulletSpeed = 10f;

    [SerializeField, Min(0f)]
    private float attack2AimBloomDegrees = 8f;

    [SerializeField, Min(0f)]
    private float rainBulletSpawnOffset = 0.5f;

    [SerializeField, Min(0f)]
    private float rainBulletXMovement = 0.3f;

    [Header("Movimiento Vertical")]
    [SerializeField, Min(0f)]
    private float verticalMoveDistance = 2f;

    [SerializeField, Min(0f)]
    private float centerY = 0f;

    [SerializeField]
    private float minY = -4f;

    [SerializeField]
    private float maxY = 4f;

    private Enemy2bAnimation _animation;
    private Rigidbody2D _rb;
    private Coroutine _aiRoutine;
    private bool _arrived;
    private Transform _playerTransform;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
        }
        _rb.gravityScale = 0f;
        _animation = GetComponent<Enemy2bAnimation>();
        
        // Buscar al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    private void OnEnable()
    {
        if (_aiRoutine == null)
        {
            _aiRoutine = StartCoroutine(BossAI());
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

    private IEnumerator BossAI()
    {
        // Fase 1: Mover a posición inicial
        while (!_arrived)
        {
            Vector2 pos = transform.position;
            Vector2 dir = (initialPosition - pos);
            float dist = dir.magnitude;
            if (dist <= arriveThreshold)
            {
                _arrived = true;
                break;
            }

            Vector2 step = dir.normalized * moveSpeed * Time.deltaTime;
            transform.position = pos + step;
            yield return null;
        }

        // Fase 2: Ciclo de ataques
        bool useAttack1 = true;
        int direction = 1; // 1 = arriba, -1 = abajo

        while (true)
        {
            // Mover al centro para atacar
            yield return StartCoroutine(MoveToY(centerY));

            // Ejecutar ataque
            if (useAttack1)
            {
                yield return StartCoroutine(ExecuteAttack1());
            }
            else
            {
                // Ejecuta ataque 2 y luego regresa a la posición inicial
                yield return StartCoroutine(ExecuteAttack2());
                yield return StartCoroutine(MoveToPosition(initialPosition));
            }

            useAttack1 = !useAttack1; // Alternar ataques

            // Mover arriba o abajo
            float targetY = centerY + (direction * verticalMoveDistance);
            yield return StartCoroutine(MoveToY(targetY));
            direction *= -1; // Alternar dirección

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private IEnumerator MoveToY(float targetY)
    {
        // Aplicar límites a la posición objetivo
        targetY = Mathf.Clamp(targetY, minY, maxY);
        
        while (Mathf.Abs(transform.position.y - targetY) > arriveThreshold)
        {
            Vector3 pos = transform.position;
            float dir = Mathf.Sign(targetY - pos.y);
            pos.y += dir * moveSpeedBetweenAttacks * Time.deltaTime;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
            yield return null;
        }

        // Ajustar a posición exacta
        Vector3 finalPos = transform.position;
        finalPos.y = Mathf.Clamp(targetY, minY, maxY);
        transform.position = finalPos;
    }

    private IEnumerator MoveToPosition(Vector2 target)
    {
        // Aplicar límites a Y
        target.y = Mathf.Clamp(target.y, minY, maxY);

        while (Vector2.Distance((Vector2)transform.position, target) > arriveThreshold)
        {
            Vector2 pos = transform.position;
            Vector2 dir = (target - pos);
            if (dir.sqrMagnitude > 0.0001f)
            {
                Vector2 step = dir.normalized * moveSpeedBetweenAttacks * Time.deltaTime;
                pos += step;
                pos.y = Mathf.Clamp(pos.y, minY, maxY);
                transform.position = pos;
            }
            yield return null;
        }

        // Ajustar a posición exacta
        Vector3 snap = transform.position;
        snap.x = target.x;
        snap.y = Mathf.Clamp(target.y, minY, maxY);
        transform.position = snap;
    }

    private IEnumerator ExecuteAttack1()
    {
        // Llamar animación de ataque 1
        if (_animation != null)
        {
            _animation.StartAttack1();
        }

        // Esperar antes de disparar la bala grande
        yield return new WaitForSeconds(bigBulletSpawnDelay);

        // Disparar bala grande
        if (bigBulletPrefab != null)
        {
            // Calcular posición de spawn con offset
            Vector3 spawnPosition = transform.position + Vector3.left * bigBulletSpawnDistance;
            
            GameObject bullet = Instantiate(bigBulletPrefab, spawnPosition, Quaternion.identity);
            BossBigBullet bigBullet = bullet.GetComponent<BossBigBullet>();
            if (bigBullet != null)
            {
                bigBullet.Initialize(Vector2.left, bigBulletSpeed, bigBulletFinalSpeed, bigBulletTimeBeforeChange, bigBulletAccelDuration);
            }
        }

        // Esperar duración del ataque
        yield return new WaitForSeconds(1f);

        // Terminar animación
        if (_animation != null)
        {
            _animation.EndAttack();
        }
    }

    private IEnumerator ExecuteAttack2()
    {
        // Llamar animación de ataque 2
        if (_animation != null)
        {
            _animation.StartAttack2();
        }

        // Dos ráfagas de balas hacia el jugador
        if (normalBulletPrefab != null && _playerTransform != null)
        {
            bool useLeftSpawn = true;

            // Primera ráfaga
            for (int i = 0; i < rainBulletCount; i++)
            {
                // Alternar entre spawn izquierdo y derecho
                float xOffset = useLeftSpawn ? -rainBulletSpawnOffset : rainBulletSpawnOffset;
                Vector3 spawnPos = transform.position + new Vector3(xOffset, 0f, 0f);

                // Calcular dirección hacia el jugador con bloom angular
                Vector2 toPlayer = (_playerTransform.position - spawnPos).normalized;
                float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
                float offset = Random.Range(-attack2AimBloomDegrees, attack2AimBloomDegrees);
                float finalAngle = baseAngle + offset;
                Vector2 finalDir = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad)).normalized;

                GameObject bullet = Instantiate(normalBulletPrefab, spawnPos, Quaternion.identity);
                EnemyBulletComportamiento enemyBullet = bullet.GetComponent<EnemyBulletComportamiento>();
                if (enemyBullet != null)
                {
                    enemyBullet.SetDirection(finalDir);
                    enemyBullet.SetSpeed(rainBulletSpeed);
                }

                if (!bullet.CompareTag("EnemyBullet"))
                {
                    bullet.tag = "EnemyBullet";
                }

                // Alternar spawn
                useLeftSpawn = !useLeftSpawn;

                // Mover ligeramente en X entre disparos
                Vector3 newPos = transform.position;
                newPos.x += Random.Range(-rainBulletXMovement, rainBulletXMovement);
                newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
                transform.position = newPos;

                yield return new WaitForSeconds(rainInterval);
            }

            // Pausa entre ráfagas
            yield return new WaitForSeconds(0.3f);

            // Segunda ráfaga
            for (int i = 0; i < rainBulletCount; i++)
            {
                // Alternar entre spawn izquierdo y derecho
                float xOffset = useLeftSpawn ? -rainBulletSpawnOffset : rainBulletSpawnOffset;
                Vector3 spawnPos = transform.position + new Vector3(xOffset, 0f, 0f);

                // Calcular dirección hacia el jugador con bloom angular
                Vector2 toPlayer = (_playerTransform.position - spawnPos).normalized;
                float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
                float offset = Random.Range(-attack2AimBloomDegrees, attack2AimBloomDegrees);
                float finalAngle = baseAngle + offset;
                Vector2 finalDir = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad)).normalized;

                GameObject bullet = Instantiate(normalBulletPrefab, spawnPos, Quaternion.identity);
                EnemyBulletComportamiento enemyBullet = bullet.GetComponent<EnemyBulletComportamiento>();
                if (enemyBullet != null)
                {
                    enemyBullet.SetDirection(finalDir);
                    enemyBullet.SetSpeed(rainBulletSpeed);
                }

                if (!bullet.CompareTag("EnemyBullet"))
                {
                    bullet.tag = "EnemyBullet";
                }

                // Alternar spawn
                useLeftSpawn = !useLeftSpawn;

                // Mover ligeramente en X entre disparos
                Vector3 newPos = transform.position;
                newPos.x += Random.Range(-rainBulletXMovement, rainBulletXMovement);
                newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
                transform.position = newPos;

                yield return new WaitForSeconds(rainInterval);
            }
        }

        // Esperar un poco más
        yield return new WaitForSeconds(0.5f);

        // Terminar animación
        if (_animation != null)
        {
            _animation.EndAttack();
        }
    }
}
