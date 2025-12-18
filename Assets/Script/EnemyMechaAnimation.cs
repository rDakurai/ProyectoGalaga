using UnityEngine;
using System.Collections;

public class EnemyMechaAnimation : MonoBehaviour
{
    [Header("Sprite Renderer")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Animaciones - Frames")]
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite[] movementFrames;
    [SerializeField] private Sprite[] initialAttackFrames;
    [SerializeField] private Sprite[] preRangedAttackFrames;
    [SerializeField] private Sprite[] rangedAttackFrames;
    [SerializeField] private Sprite[] preRangedChargedFrames;
    [SerializeField] private Sprite[] rangedChargedFrames;
    [SerializeField] private Sprite[] preSpindashFrames;
    [SerializeField] private Sprite[] spindashFrames;
    [SerializeField] private Sprite[] chargeRecoveryFrames;
    [SerializeField] private Sprite[] postReubicacionFrames;
    [SerializeField] private Sprite[] repositionFrames;

    [Header("Configuración de Animación")]
    [SerializeField] private float frameRate = 10f; // Frames por segundo

    private Sprite[] currentAnimation;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private bool isAnimating = true;
    private bool loopAnimation = true;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        PlayIdle();
    }

    void Update()
    {
        if (isAnimating && currentAnimation != null && currentAnimation.Length > 0)
        {
            frameTimer += Time.deltaTime;
            
            if (frameTimer >= 1f / frameRate)
            {
                frameTimer = 0f;
                currentFrame++;

                if (currentFrame >= currentAnimation.Length)
                {
                    if (loopAnimation)
                    {
                        currentFrame = 0;
                    }
                    else
                    {
                        currentFrame = currentAnimation.Length - 1;
                        isAnimating = false;
                    }
                }

                spriteRenderer.sprite = currentAnimation[currentFrame];
            }
        }
    }

    public void PlayIdle()
    {
        PlayAnimation(idleFrames, true);
    }

    public void PlayMovement()
    {
        PlayAnimation(movementFrames, true);
    }

    public void PlayInitialAttack()
    {
        PlayAnimation(initialAttackFrames, false);
    }

    public void PlayPreRangedAttack()
    {
        PlayAnimation(preRangedAttackFrames, false);
    }

    public void PlayRangedAttack()
    {
        PlayAnimation(rangedAttackFrames, false);
    }

    public void PlayPreRangedCharged()
    {
        PlayAnimation(preRangedChargedFrames, false);
    }

    public void PlayRangedCharged()
    {
        PlayAnimation(rangedChargedFrames, false);
    }

    public void PlayPreSpindash()
    {
        PlayAnimation(preSpindashFrames, false);
    }

    public void PlaySpindash()
    {
        PlayAnimation(spindashFrames, false);
    }

    public void PlayChargeRecovery()
    {
        PlayAnimation(chargeRecoveryFrames, false);
    }

    public void PlayPostReubicacion()
    {
        PlayAnimation(postReubicacionFrames, false);
    }

    public void PlayReposition()
    {
        PlayAnimation(repositionFrames, false);
    }

    private void PlayAnimation(Sprite[] animation, bool loop)
    {
        if (animation == null || animation.Length == 0)
            return;

        currentAnimation = animation;
        currentFrame = 0;
        frameTimer = 0f;
        loopAnimation = loop;
        isAnimating = true;
        spriteRenderer.sprite = currentAnimation[0];
    }

    public void StopAnimation()
    {
        isAnimating = false;
    }

    public void ResumeAnimation()
    {
        isAnimating = true;
    }

    public bool IsAnimationFinished()
    {
        return !isAnimating && !loopAnimation;
    }
}
