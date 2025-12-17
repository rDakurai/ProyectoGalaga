using UnityEngine;

public class PickupPulse : MonoBehaviour
{
    [Header("Pulso")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float scaleMultiplier = 1.1f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float scale = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
        float finalScale = Mathf.Lerp(1f, scaleMultiplier, scale);

        transform.localScale = originalScale * finalScale;
    }
}
