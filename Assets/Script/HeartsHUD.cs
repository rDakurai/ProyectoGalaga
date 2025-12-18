using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HeartsUI : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerHealth playerHealth;
    public Transform heartsPanel;          // VidaPlayer
    public GameObject heartIconPrefab;     // HeartIcon.prefab

    [Header("Sprites")]
    public Sprite heartFull;
    public Sprite heartEmpty;

    private List<Image> hearts = new();

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHearts;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHearts;
    }

    private void Start()
    {
        UpdateHearts(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindPlayer();
    }

    private void RebindPlayer()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHearts;

        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHearts;
            UpdateHearts(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
        else
        {
            // Si no hay PlayerHealth en la nueva escena, mostrar vacíos
            for (int i = 0; i < hearts.Count; i++)
                hearts[i].sprite = heartEmpty;
        }
    }

    private void UpdateHearts(int current, int max)
    {
        // Crear corazones si faltan
        while (hearts.Count < max)
        {
            GameObject go = Instantiate(heartIconPrefab, heartsPanel);
            hearts.Add(go.GetComponent<Image>());
        }

        // Eliminar si sobran
        while (hearts.Count > max)
        {
            Destroy(hearts[^1].gameObject);
            hearts.RemoveAt(hearts.Count - 1);
        }

        // Actualizar sprites
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].sprite = (i < current) ? heartFull : heartEmpty;
        }
    }
}
