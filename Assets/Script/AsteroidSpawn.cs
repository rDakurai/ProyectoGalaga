using UnityEngine;
using System.Collections.Generic;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid")]
    [SerializeField] private GameObject asteroidPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnEvery = 1.5f;
    [SerializeField] private int maxAlive = 20;
    [SerializeField] private float verticalMargin = 0.5f;

    [Header("Duración")]
    [SerializeField] private float durationSeconds = 60f;

    private float spawnTimer;
    private float lifeTimer;

    private Camera cam;
    private readonly List<GameObject> asteroidsAlive = new();
    private bool active = true;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!active) return;
        if (asteroidPrefab == null) return;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= durationSeconds)
        {
            active = false;
            return;
        }

        asteroidsAlive.RemoveAll(a => a == null);

        if (asteroidsAlive.Count >= maxAlive)
            return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnEvery)
        {
            spawnTimer = 0f;
            SpawnAsteroid();
        }
    }

    private void SpawnAsteroid()
    {
        float camHeight = cam.orthographicSize;
        float y = Random.Range(
            -camHeight + verticalMargin,
             camHeight - verticalMargin
        );

        Vector3 spawnPos = new Vector3(transform.position.x, y, 0f);

        GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);
        asteroidsAlive.Add(asteroid);
    }

    public bool IsFinished => !active;
}


