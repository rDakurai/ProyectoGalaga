using UnityEngine;

public class Enemy2bAnimation : MonoBehaviour
{
    private const int STATE_IDLE = 0;
    private const int STATE_MOVING = 1;
    private const int STATE_ATTACK1 = 2;
    private const int STATE_ATTACK2 = 3;

    [Header("Sprites")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite[] idleSprites;

    [SerializeField]
    private Sprite[] movingSprites;

    [SerializeField]
    private Sprite[] attack1Sprites;

    [SerializeField]
    private Sprite[] attack2Sprites;

    [SerializeField, Min(0.1f)]
    private float frameRate = 10f;

    [SerializeField, Min(0f)]
    private float idleThreshold = 0.05f;

    private int _currentState = -1;
    private int _frameIndex;
    private float _frameTimer;
    private Rigidbody2D _rb;
    private bool _isAttacking1;
    private bool _isAttacking2;
    private bool _attack1Finished;
    private bool _attack2Finished;

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
        // Prioridad: Attack2 > Attack1 > Moving > Idle
        if (_isAttacking2 && !_attack2Finished)
        {
            return STATE_ATTACK2;
        }

        if (_isAttacking1 && !_attack1Finished)
        {
            return STATE_ATTACK1;
        }

        if (_rb != null && _rb.linearVelocity.sqrMagnitude > idleThreshold * idleThreshold)
        {
            return STATE_MOVING;
        }

        return STATE_IDLE;
    }

    public void StartAttack1()
    {
        _isAttacking1 = true;
        _isAttacking2 = false; // asegura prioridad
        _attack1Finished = false;
        _currentState = STATE_ATTACK1;
        _frameIndex = 0;
        _frameTimer = 0f;
        ApplyFrame();
    }

    public void StartAttack2()
    {
        _isAttacking2 = true;
        _isAttacking1 = false; // asegura prioridad
        _attack2Finished = false;
        _currentState = STATE_ATTACK2;
        _frameIndex = 0;
        _frameTimer = 0f;
        ApplyFrame();
    }

    public void EndAttack()
    {
        _isAttacking1 = false;
        _isAttacking2 = false;
        _attack1Finished = false;
        _attack2Finished = false;
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
            
            int nextFrame = _frameIndex + 1;
            
            // Si es animación de ataque y termina el ciclo, marcar como terminada
            if (nextFrame >= frames.Length)
            {
                if (_currentState == STATE_ATTACK1)
                {
                    _attack1Finished = true;
                }
                else if (_currentState == STATE_ATTACK2)
                {
                    _attack2Finished = true;
                }
            }
            
            _frameIndex = nextFrame % frames.Length;
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
            case STATE_ATTACK1:
                return attack1Sprites;
            case STATE_ATTACK2:
                return attack2Sprites;
            default:
                return idleSprites;
        }
    }
}
