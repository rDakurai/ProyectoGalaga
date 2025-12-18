using UnityEngine;
using System.Collections;

public class Coins : MonoBehaviour
{
    [Header("Animación (sprites)")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 12f;

    [Header("Pickup")]
    [SerializeField] private bool destroyOnCollect = true;
    [SerializeField] private AudioClip collectSfx; // opcional

    [Header("Score")]
    [SerializeField] private int scoreValue = 25;

    private int index;
    private Coroutine animCo;

    private void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (animCo != null) StopCoroutine(animCo);
        animCo = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        if (sr == null || frames == null || frames.Length == 0) yield break;

        float wait = 1f / Mathf.Max(1f, fps);

        while (true)
        {
            sr.sprite = frames[index];
            index = (index + 1) % frames.Length;
            yield return new WaitForSeconds(wait);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);
        else
            Debug.LogWarning("No existe ScoreManager en escena.");

        if (collectSfx != null)
            AudioSource.PlayClipAtPoint(collectSfx, transform.position);

        if (destroyOnCollect)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}


