using System.Collections;
using UnityEngine;
using TMPro;

public class Level2Controller : MonoBehaviour
{
    public LevelManager levelManager;
    public float surviveSeconds = 60f;

    [Header("Boss Spawn (ya lo tienen)")]
    public GameObject bossSpawnerOrBossController; 
    // si tu sistema ya spawnea al jefe, aquí solo lo activas

    float timeLeft;
    bool bossPhase = false;

    void Start()
    {
        if (levelManager == null) levelManager = FindObjectOfType<LevelManager>();

        timeLeft = surviveSeconds;

        levelManager.ShowMessage("Esquiva o destruye los meteoritos", 3f);

        if (bossSpawnerOrBossController != null)
            bossSpawnerOrBossController.SetActive(false);

        StartCoroutine(SurviveRoutine());
    }

    IEnumerator SurviveRoutine()
    {
        while (timeLeft > 0f && !bossPhase)
        {
            timeLeft -= Time.deltaTime;

            if (levelManager.timerText != null)
                levelManager.timerText.text = "TIEMPO: " + Mathf.CeilToInt(timeLeft).ToString();

            yield return null;
        }

        bossPhase = true;

        if (levelManager.timerText != null)
            levelManager.timerText.text = "";

        levelManager.ShowMessage("¡Aparece el jefe! Elimínalo", 3f);

        if (bossSpawnerOrBossController != null)
            bossSpawnerOrBossController.SetActive(true);

        // Aquí termina: cuando el jefe muera, lo llaman desde el jefe igual que nivel 1
    }

    // Llamar desde el jefe cuando muera en Nivel 2
    public void OnBossKilled()
    {
        levelManager.ShowMessage("¡Jefe eliminado! Preparando siguiente nivel...", 2f);
        levelManager.LoadNextLevelAfterDelay();
    }
}
