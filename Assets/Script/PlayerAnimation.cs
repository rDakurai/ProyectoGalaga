using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private const int STATE_IDLE = 0;
    private const int STATE_UP = 1;
    private const int STATE_DOWN = 2;
    private const int STATE_LEFT = 3;
    private const int STATE_RIGHT = 4;

    [Header("Sprites")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite[] idleSprites;

    [SerializeField]
    private Sprite[] upSprites;

    [SerializeField]
    private Sprite[] downSprites;

    [SerializeField]
    private Sprite[] leftSprites;

    [SerializeField]
    private Sprite[] rightSprites;

    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    private Rigidbody2D targetRb;

    [SerializeField, Min(0f)]
    private float idleThreshold = 0.05f;

    [SerializeField, Min(0.1f)]
    private float frameRate = 10f;

    private int _currentState = -1;
    private int _frameIndex;
    private float _frameTimer;

    private void Reset()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        controller = GetComponent<PlayerController>();
        targetRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Vector2 inputDir = controller != null ? controller.MoveInput : Vector2.zero;
        Vector2 velocity = targetRb != null ? targetRb.linearVelocity : Vector2.zero;

        // Usa input WASD primero; si no hay, cae a la velocidad real (por empujes externos).
        Vector2 sourceDir = inputDir.sqrMagnitude > 0.0001f ? inputDir : velocity;

        int state = DetermineState(sourceDir);
        if (state != _currentState)
        {
            _currentState = state;
            _frameIndex = 0;
            _frameTimer = 0f;
            ApplyFrame();
        }

        AdvanceFrame();
    }

    private int DetermineState(Vector2 velocity)
    {
        if (velocity.sqrMagnitude <= idleThreshold * idleThreshold)
        {
            return STATE_IDLE;
        }

        float absX = Mathf.Abs(velocity.x);
        float absY = Mathf.Abs(velocity.y);

        if (absX > absY)
        {
            return velocity.x > 0f ? STATE_RIGHT : STATE_LEFT;
        }

        return velocity.y > 0f ? STATE_UP : STATE_DOWN;
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
            case STATE_UP:
                return upSprites;
            case STATE_DOWN:
                return downSprites;
            case STATE_LEFT:
                return leftSprites;
            case STATE_RIGHT:
                return rightSprites;
            default:
                return idleSprites;
        }
    }
}
