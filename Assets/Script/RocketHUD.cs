using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RocketsUI : MonoBehaviour
{
    [Header("Referencias")]
    public RocketLauncher rocketLauncher;
    public Transform rocketsPanel;           // Contenedor UI (debajo del de vida)
    public GameObject rocketIconPrefab;      // Prefab con Image

    [Header("Sprites")]
    public Sprite rocketFull;
    public Sprite rocketEmpty;

    [Header("Config")]
    [SerializeField] private int maxSlots = 3;

    private readonly List<Image> rockets = new();

    private void Awake()
    {
        if (rocketLauncher == null)
            rocketLauncher = FindObjectOfType<RocketLauncher>();
    }

    private void OnEnable()
    {
        if (rocketLauncher != null)
            rocketLauncher.OnRocketsChanged += UpdateRockets;
    }

    private void OnDisable()
    {
        if (rocketLauncher != null)
            rocketLauncher.OnRocketsChanged -= UpdateRockets;
    }

    private void Start()
    {
        if (rocketLauncher != null)
            UpdateRockets(rocketLauncher.CurrentRockets, rocketLauncher.MaxRockets);
        else
            UpdateRockets(0, maxSlots);
    }

    private void UpdateRockets(int current, int max)
    {
        int slots = Mathf.Clamp(max, 0, maxSlots);
        current = Mathf.Clamp(current, 0, slots);

        // Crear iconos si faltan (hasta slots)
        while (rockets.Count < slots)
        {
            GameObject go = Instantiate(rocketIconPrefab, rocketsPanel);
            rockets.Add(go.GetComponent<Image>());
        }

        // Eliminar si sobran (si algún día max cambia)
        while (rockets.Count > slots)
        {
            Destroy(rockets[^1].gameObject);
            rockets.RemoveAt(rockets.Count - 1);
        }

        // Actualizar sprites
        for (int i = 0; i < rockets.Count; i++)
        {
            rockets[i].sprite = (i < current) ? rocketFull : rocketEmpty;
        }
    }
}
