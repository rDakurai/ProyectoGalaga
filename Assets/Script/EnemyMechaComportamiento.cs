using UnityEngine;
using System.Collections;

public class EnemyMechaComportamiento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private EnemyMechaAnimation animationController;
    [SerializeField] private Transform player;
    
    [Header("Configuración de Posición")]
    [SerializeField] private Vector2 originPoint; // Punto de origen para ataques
    [SerializeField] private Vector2 reubicacionPoint; // Punto de reubicación inicial
    
    [Header("Velocidades")]
    [SerializeField] private float initialAttackSpeed = 20f;
    [SerializeField] private float reubicacionSpeed = 8f;
    [SerializeField] private float spindashSpeed = 15f;
    [SerializeField] private float rangedChargedSpeed = 3f;
    [SerializeField] private float rangedChargedAcceleration = 2f;
    [SerializeField] private float idleMovementSpeed = 2f;
    
    [Header("Configuración de Ataques")]
    [SerializeField] private float spindashDistance = 10f;
    [SerializeField] private int rangedBulletCount = 5;
    [SerializeField] private float rangedArcAngle = 45f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject rangedChargedBulletPrefab;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float rangedChargedFireRate = 0.3f;
    [SerializeField] private float rangedChargedDuration = 3f;
    
    [Header("Tiempos de Espera")]
    [SerializeField] private float timeBetweenAttacks = 10f;
    [SerializeField] private float idleDuration = 10f;
    
    [Header("Movimiento Idle")]
    [SerializeField] private Vector2 idleMovementRangeVertical = new Vector2(-2f, 2f);
    [SerializeField] private Vector2 idleMovementRangeHorizontal = new Vector2(-1f, 1f);
    [SerializeField] private Vector2 cameraBoundsMin = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 cameraBoundsMax = new Vector2(8f, 4f);
    
    private enum State
    {
        InitialAttack,
        Reubicacion,
        PostReubicacion,
        Idle,
        PreparingAttack,
        PreSpindash,
        Spindash,
        SpindashRecovery,
        PreRanged,
        Ranged,
        PreRangedCharged,
        RangedCharged,
        PostReubicacionAfterAttack
    }
    
    private State currentState;
    private float stateTimer;
    private Vector2 targetPosition;
    private Vector2 idleTargetPosition;
    private float currentSpeed;
    private int nextAttackIndex = 0;
    
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (animationController == null)
            animationController = GetComponent<EnemyMechaAnimation>();
        
        idleTargetPosition = transform.position;
        StartCoroutine(BossBehavior());
    }
    
    void Update()
    {
        HandleMovement();
    }
    
    private IEnumerator BossBehavior()
    {
        // Fase 1: Initial Attack (solo una vez al inicio)
        yield return StartCoroutine(ExecuteInitialAttack());
        
        // Fase 2: Reubicación inicial (solo después del initial attack)
        yield return StartCoroutine(ExecuteReubicacion(reubicacionPoint));
        
        // Fase 3: Post Reubicación (solo después del initial attack)
        yield return StartCoroutine(ExecutePostReubicacion());
        
        // Fase 4: Loop de combate
        while (true)
        {
            // Idle
            yield return StartCoroutine(ExecuteIdle());
            
            // Reubicarse al punto de origen antes del ataque
            yield return StartCoroutine(ExecuteReubicacion(originPoint));
            
            // Ejecutar ataque
            yield return StartCoroutine(ExecuteAttack());
            
            nextAttackIndex = (nextAttackIndex + 1) % 3;
        }
    }
    
    private IEnumerator ExecuteInitialAttack()
    {
        currentState = State.InitialAttack;
        animationController.PlayInitialAttack();
        
        targetPosition = new Vector2(20f, transform.position.y); // Ir hacia la derecha
        currentSpeed = initialAttackSpeed;
        
        // Esperar a llegar al destino
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPosition) < 0.5f);
    }
    
    private IEnumerator ExecuteReubicacion(Vector2 destination)
    {
        currentState = State.Reubicacion;
        animationController.PlayReposition();
        
        targetPosition = destination;
        currentSpeed = reubicacionSpeed;
        
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPosition) < 0.1f);
        yield return new WaitUntil(() => animationController.IsAnimationFinished());
    }
    
    private IEnumerator ExecutePostReubicacion()
    {
        currentState = State.PostReubicacion;
        animationController.PlayPostReubicacion();
        
        yield return new WaitUntil(() => animationController.IsAnimationFinished());
    }
    
    private IEnumerator ExecuteIdle()
    {
        currentState = State.Idle;
        animationController.PlayIdle();
        
        float idleTime = 0f;
        while (idleTime < idleDuration)
        {
            // Generar nueva posición aleatoria cada cierto tiempo
            if (Vector2.Distance(transform.position, idleTargetPosition) < 0.2f)
            {
                float randomY = Random.Range(idleMovementRangeVertical.x, idleMovementRangeVertical.y);
                float randomX = Random.Range(idleMovementRangeHorizontal.x, idleMovementRangeHorizontal.y);
                
                Vector2 newPosition = new Vector2(
                    transform.position.x + randomX,
                    transform.position.y + randomY
                );
                
                // Clampear la posición dentro de los límites de la cámara
                newPosition.x = Mathf.Clamp(newPosition.x, cameraBoundsMin.x, cameraBoundsMax.x);
                newPosition.y = Mathf.Clamp(newPosition.y, cameraBoundsMin.y, cameraBoundsMax.y);
                
                idleTargetPosition = newPosition;
            }
            
            targetPosition = idleTargetPosition;
            currentSpeed = idleMovementSpeed;
            
            idleTime += Time.deltaTime;
            yield return null;
        }
    }
    
    private IEnumerator ExecuteAttack()
    {
        switch (nextAttackIndex)
        {
            case 0:
                yield return StartCoroutine(ExecuteRangedAttack());
                break;
            case 1:
                yield return StartCoroutine(ExecuteSpindashAttack());
                break;
            case 2:
                yield return StartCoroutine(ExecuteRangedChargedAttack());
                break;
        }
    }
    
    private IEnumerator ExecuteSpindashAttack()
    {
        // Pre Spindash
        currentState = State.PreSpindash;
        animationController.PlayPreSpindash();
        yield return new WaitUntil(() => animationController.IsAnimationFinished());
        
        // Spindash
        currentState = State.Spindash;
        animationController.PlaySpindash();
        
        Vector2 playerDirection = (player.position - transform.position).normalized;
        targetPosition = (Vector2)transform.position + playerDirection * spindashDistance;
        currentSpeed = spindashSpeed;
        
        float distanceTraveled = 0f;
        Vector2 startPos = transform.position;
        
        while (distanceTraveled < spindashDistance)
        {
            distanceTraveled = Vector2.Distance(startPos, transform.position);
            yield return null;
        }
        
        // Recovery
        currentState = State.SpindashRecovery;
        animationController.PlayChargeRecovery();
        yield return new WaitUntil(() => animationController.IsAnimationFinished());
    }
    
    private IEnumerator ExecuteRangedAttack()
    {
        // Pre Ranged
        currentState = State.PreRanged;
        animationController.PlayPreRangedAttack();
        yield return new WaitUntil(() => animationController.IsAnimationFinished());
        
        // Ranged
        currentState = State.Ranged;
        animationController.PlayRangedAttack();
        
        // Disparar balas en arco
        float startAngle = -rangedArcAngle / 2f;
        float angleStep = rangedArcAngle / (rangedBulletCount - 1);
        
        for (int i = 0; i < rangedBulletCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.down;
            
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            BulletHomingBehavior homing = bullet.AddComponent<BulletHomingBehavior>();
            homing.Initialize(direction * bulletSpeed, player);
        }
        
        yield return new WaitUntil(() => animationController.IsAnimationFinished());
    }
    
    private IEnumerator ExecuteRangedChargedAttack()
    {
        // Pre Ranged Charged
        currentState = State.PreRangedCharged;
        animationController.PlayPreRangedCharged();
        yield return new WaitUntil(() => animationController.IsAnimationFinished());
        
        // Ranged Charged
        currentState = State.RangedCharged;
        animationController.PlayRangedCharged();
        
        float chargedTime = 0f;
        float fireTimer = 0f;
        currentSpeed = rangedChargedSpeed;
        targetPosition = new Vector2(transform.position.x - 10f, transform.position.y); // Moverse hacia la izquierda
        
        while (chargedTime < rangedChargedDuration)
        {
            // Acelerar gradualmente
            currentSpeed += rangedChargedAcceleration * Time.deltaTime;
            
            // Disparar balas con bloom
            fireTimer += Time.deltaTime;
            if (fireTimer >= rangedChargedFireRate)
            {
                fireTimer = 0f;
                Vector2 bloomOffset = Random.insideUnitCircle * 0.5f;
                Vector2 direction = (Vector2.left + bloomOffset).normalized;
                
                GameObject bullet = Instantiate(rangedChargedBulletPrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.linearVelocity = direction * bulletSpeed;
            }
            
            chargedTime += Time.deltaTime;
            yield return null;
        }
    }
    
    private void HandleMovement()
    {
        if (currentState == State.InitialAttack || currentState == State.Reubicacion || 
            currentState == State.Spindash || currentState == State.RangedCharged || currentState == State.Idle)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        }
    }
}

// Clase auxiliar para balas teledirigidas
public class BulletHomingBehavior : MonoBehaviour
{
    private Vector2 velocity;
    private Transform target;
    private float homingStrength = 5f;
    
    public void Initialize(Vector2 initialVelocity, Transform targetTransform)
    {
        velocity = initialVelocity;
        target = targetTransform;
    }
    
    void Update()
    {
        if (target != null)
        {
            Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
            velocity = Vector2.Lerp(velocity, direction * velocity.magnitude, homingStrength * Time.deltaTime);
        }
        
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}
