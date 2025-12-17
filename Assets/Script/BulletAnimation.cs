using UnityEngine;

public class BulletAnimation : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite[] animationFrames;

    [SerializeField, Min(0.1f)]
    private float frameRate = 12f;

    private int _frameIndex;
    private float _frameTimer;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if (animationFrames != null && animationFrames.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = animationFrames[0];
        }
    }

    private void Update()
    {
        if (spriteRenderer == null || animationFrames == null || animationFrames.Length == 0)
        {
            return;
        }

        _frameTimer += Time.deltaTime;
        float frameDuration = 1f / frameRate;
        
        if (_frameTimer >= frameDuration)
        {
            _frameTimer -= frameDuration;
            _frameIndex = (_frameIndex + 1) % animationFrames.Length;
            spriteRenderer.sprite = animationFrames[_frameIndex];
        }
    }
}
