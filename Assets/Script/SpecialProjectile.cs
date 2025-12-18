using UnityEngine;
using System.Collections;

public class SpecialProjectile : MonoBehaviour
{
    [Header("Animación (4 sprites)")]
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameRate = 12f;

    [Header("Movimiento")]
    [SerializeField] private float speed = 10f;

    [Header("Daño")]
    [SerializeField] private int damage = 10;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 2f;

    private SpriteRenderer sr;
    private int index;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
        StartCoroutine(Animate());
    }

    private void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
    }

    private IEnumerator Animate()
    {
        if (frames == null || frames.Length == 0 || sr == null) yield break;

        float wait = 1f / Mathf.Max(1f, frameRate);

        while (true)
        {
            sr.sprite = frames[index];
            index = (index + 1) % frames.Length;
            yield return new WaitForSeconds(wait);
        }
    }
}

