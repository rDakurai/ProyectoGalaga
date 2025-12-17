using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private const int STATE_IDLE = 0;
    private const int STATE_MOVING = 1;
    private const int STATE_ATTACKING = 2;

    [Header("Sprites")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite[] idleSprites;

    [SerializeField]
    private Sprite[] movingSprites;

    [SerializeField]
    private Sprite[] attackingSprites;

    [SerializeField, Min(0.1f)]
    private float frameRate = 10f;

    [SerializeField, Min(0f)]
    private float idleThreshold = 0.05f;

    private int _currentState = -1;
    private int _frameIndex;
    private float _frameTimer;
    private Rigidbody2D _rb;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        int newState = DetermineState();
        if (newState != _currentState)
        {
            _currentState = newState;
            _frameIndex = 0;
            _frameTimer = 0f;
            ApplyFrame();
        }

        AdvanceFrame();
    }

    private int DetermineState()
    {
        // Por defecto idle; modifica aquí según tu lógica de enemigo
        // (por ej., detecta si se está moviendo o atacando)
        if (_rb != null && _rb.linearVelocity.sqrMagnitude > idleThreshold * idleThreshold)
        {
            return STATE_MOVING;
        }

        return STATE_IDLE;
    }

    public void SetAttacking(bool attacking)
    {
        // Puedes llamar a esto desde tu lógica de combate
        if (attacking && _currentState != STATE_ATTACKING)
        {
            _currentState = STATE_ATTACKING;
            _frameIndex = 0;
            _frameTimer = 0f;
            ApplyFrame();
        }
    }

    private void AdvanceFrame()
    {
        Sprite[] frames = GetFramesForState(_currentState);
        if (frames == null || frames.Length == 0)
        {
            return;
        }

        _frameTimer += Time.deltaTime;
        float frameDuration = 1f / frameRate;
        if (_frameTimer >= frameDuration)
        {
            _frameTimer -= frameDuration;
            _frameIndex = (_frameIndex + 1) % frames.Length;
            ApplyFrame();
        }
    }

    private void ApplyFrame()
    {
        Sprite[] frames = GetFramesForState(_currentState);
        if (frames == null || frames.Length == 0)
        {
            return;
        }

        _frameIndex = Mathf.Clamp(_frameIndex, 0, frames.Length - 1);
        spriteRenderer.sprite = frames[_frameIndex];
    }

    private Sprite[] GetFramesForState(int state)
    {
        switch (state)
        {
            case STATE_MOVING:
                return movingSprites;
            case STATE_ATTACKING:
                return attackingSprites;
            default:
                return idleSprites;
        }
    }
}
