using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        [Header("Configuración de Oleada")]
        public string waveName = "Wave 1";
        
        [Header("Enemigos a Generar")]
        public GameObject enemyPrefab;
        [Min(1)] public int enemyCount = 5;
        
        [Header("Modo de Generación")]
        public bool burstMode = false;
        [Min(1)] public int enemiesPerBurst = 3;
        [Min(1)] public int burstCount = 2;
        [Min(1)] public int simultaneousSpawns = 1; // Cuántos enemigos spawnean a la vez
        
        [Header("Tiempos")]
        [Min(0f)] public float spawnInterval = 1f;
        [Min(0f)] public float burstInterval = 0.2f;
        [Min(0f)] public float burstCooldown = 2f;
        [Min(0f)] public float delayBeforeWave = 2f;
        
        [Header("Posiciones de Spawn")]
        public Vector2 spawnPosition = new Vector2(10f, 0f);
        public bool randomizeYPosition = true;
        [Min(0f)] public float yRandomRange = 4f;
    }

    [Header("Oleadas")]
    [SerializeField] private Wave[] waves;
    
    [Header("Configuración General")]
    [SerializeField] private bool autoStart = true;
    [SerializeField, Min(0f)] private float initialDelay = 0f; // Delay inicial antes de comenzar
    [SerializeField] private bool loopWaves = false;
    [SerializeField, Min(0f)] private float delayBetweenLoops = 5f;

    private int _currentWaveIndex = 0;
    private bool _isGenerating = false;

    private void Start()
    {
        if (autoStart && waves != null && waves.Length > 0)
        {
            StartGeneration();
        }
    }

    public void StartGeneration()
    {
        if (!_isGenerating)
        {
            _currentWaveIndex = 0;
            StartCoroutine(GenerateWaves());
        }
    }

    public void StopGeneration()
    {
        _isGenerating = false;
        StopAllCoroutines();
    }

    private IEnumerator GenerateWaves()
    {
        _isGenerating = true;

        // Esperar el delay inicial
        yield return new WaitForSeconds(initialDelay);

        while (_isGenerating)
        {
            // Generar todas las oleadas
            for (int i = 0; i < waves.Length; i++)
            {
                if (!_isGenerating) yield break;

                _currentWaveIndex = i;
                yield return StartCoroutine(GenerateWave(waves[i]));
                
                // Si no es la última wave, esperar a que terminen los enemigos de esta wave
                if (i < waves.Length - 1)
                {
                    yield return StartCoroutine(WaitForWaveComplete());
                }
            }

            // Si no se debe hacer loop, detener
            if (!loopWaves)
            {
                _isGenerating = false;
                break;
            }

            // Esperar antes de reiniciar el loop
            yield return new WaitForSeconds(delayBetweenLoops);
        }
    }

    private IEnumerator GenerateWave(Wave wave)
    {
        if (wave.enemyPrefab == null)
        {
            Debug.LogWarning($"Wave '{wave.waveName}' no tiene prefab asignado.");
            yield break;
        }

        // Esperar antes de iniciar la oleada
        yield return new WaitForSeconds(wave.delayBeforeWave);

        if (wave.burstMode)
        {
            // Modo ráfaga
            yield return StartCoroutine(GenerateWaveInBursts(wave));
        }
        else
        {
            // Modo normal
            yield return StartCoroutine(GenerateWaveNormal(wave));
        }
    }

    private IEnumerator GenerateWaveNormal(Wave wave)
    {
        // Generar enemigos uno por uno o varios a la vez según simultaneousSpawns
        for (int i = 0; i < wave.enemyCount; i += wave.simultaneousSpawns)
        {
            if (!_isGenerating) yield break;

            // Spawnear varios enemigos a la vez
            int enemiesToSpawn = Mathf.Min(wave.simultaneousSpawns, wave.enemyCount - i);
            for (int j = 0; j < enemiesToSpawn; j++)
            {
                SpawnEnemy(wave);
            }

            // Esperar intervalo antes del siguiente lote
            if (i + wave.simultaneousSpawns < wave.enemyCount)
            {
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }
    }

    private IEnumerator GenerateWaveInBursts(Wave wave)
    {
        int totalSpawned = 0;

        // Generar ráfagas hasta alcanzar el total de enemigos
        for (int burst = 0; burst < wave.burstCount && totalSpawned < wave.enemyCount; burst++)
        {
            if (!_isGenerating) yield break;

            // Determinar cuántos enemigos generar en esta ráfaga
            int enemiesToSpawn = Mathf.Min(wave.enemiesPerBurst, wave.enemyCount - totalSpawned);

            // Generar enemigos de la ráfaga
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                if (!_isGenerating) yield break;

                SpawnEnemy(wave);
                totalSpawned++;

                // Esperar intervalo entre enemigos de la ráfaga
                if (i < enemiesToSpawn - 1)
                {
                    yield return new WaitForSeconds(wave.burstInterval);
                }
            }

            // Esperar cooldown entre ráfagas (solo si no es la última y no hemos alcanzado el total)
            if (burst < wave.burstCount - 1 && totalSpawned < wave.enemyCount)
            {
                yield return new WaitForSeconds(wave.burstCooldown);
            }
        }
    }

    private IEnumerator WaitForWaveComplete()
    {
        // Esperar a que no haya enemigos (o un tiempo máximo de 300 segundos)
        float maxWaitTime = 300f;
        float waitedTime = 0f;
        
        while (true)
        {
            // Contar enemigos activos
            GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            if (activeEnemies.Length == 0)
            {
                // No hay más enemigos, la wave ha terminado
                break;
            }
            
            // Seguridad: si esperamos demasiado, continuar de todas formas
            if (waitedTime > maxWaitTime)
            {
                Debug.LogWarning("Tiempo máximo de espera para terminar la wave excedido. Continuando a la siguiente wave.");
                break;
            }
            
            waitedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void SpawnEnemy(Wave wave)
    {
        // Calcular posición de spawn basada en la posición del generador
        Vector3 spawnPos = transform.position + (Vector3)wave.spawnPosition;
        if (wave.randomizeYPosition)
        {
            spawnPos.y += Random.Range(-wave.yRandomRange, wave.yRandomRange);
        }

        // Instanciar enemigo
        GameObject enemy = Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);
        
        // Asegurar que tenga el tag correcto
        if (!enemy.CompareTag("Enemy"))
        {
            enemy.tag = "Enemy";
        }
    }

    // Métodos públicos para control manual
    public void StartNextWave()
    {
        if (_currentWaveIndex < waves.Length - 1)
        {
            _currentWaveIndex++;
            StartCoroutine(GenerateWave(waves[_currentWaveIndex]));
        }
    }

    public void GenerateSpecificWave(int waveIndex)
    {
        if (waveIndex >= 0 && waveIndex < waves.Length)
        {
            StartCoroutine(GenerateWave(waves[waveIndex]));
        }
    }

    private void OnDrawGizmos()
    {
        if (waves == null || waves.Length == 0) return;

        // Dibujar puntos de spawn en el editor
        Gizmos.color = Color.red;
        foreach (var wave in waves)
        {
            if (wave.randomizeYPosition)
            {
                Vector3 top = new Vector3(wave.spawnPosition.x, wave.spawnPosition.y + wave.yRandomRange, 0);
                Vector3 bottom = new Vector3(wave.spawnPosition.x, wave.spawnPosition.y - wave.yRandomRange, 0);
                Gizmos.DrawLine(top, bottom);
                Gizmos.DrawWireSphere(top, 0.2f);
                Gizmos.DrawWireSphere(bottom, 0.2f);
            }
            else
            {
                Gizmos.DrawWireSphere(wave.spawnPosition, 0.3f);
            }
        }
    }
}
