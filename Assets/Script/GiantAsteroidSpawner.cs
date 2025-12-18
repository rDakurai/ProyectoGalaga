using UnityEngine;

public class GiantAsteroidSpawner : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float spawnAfterSeconds = 60f;

    [Header("Boss")]
    [SerializeField] private GameObject giantAsteroidPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnXOffset = 2f;
    [SerializeField] private float verticalMargin = 0.5f;

    private float timer;
    private bool spawned;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        if (spawned) return;

        timer += Time.deltaTime;
        if (timer >= spawnAfterSeconds)
        {
            SpawnGiantAsteroid();
            spawned = true;
        }
    }

    private void SpawnGiantAsteroid()
    {
        if (giantAsteroidPrefab == null) return;

        float camHeight = cam.orthographicSize;
        float y = Random.Range(
            -camHeight + verticalMargin,
             camHeight - verticalMargin
        );

        Vector3 spawnPos = new Vector3(
            transform.position.x + spawnXOffset,
            y,
            0f
        );

        GameObject boss = Instantiate(giantAsteroidPrefab, spawnPos, Quaternion.identity);

        var ctrl = boss.GetComponent<GiantAsteroidController>();
        if (ctrl != null)
            ctrl.SetTarget(player);
    }
}


