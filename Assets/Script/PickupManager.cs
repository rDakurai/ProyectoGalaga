using UnityEngine;
using System;

public class PickupManager : MonoBehaviour
{
    [System.Serializable]
    public class PickupAudio
    {
        [Header("Sonidos de Recolección")]
        public AudioClip[] pickupClips;
        public float volume = 1f;
    }

    private static PickupManager _instance;

    [Header("Volumen Global")]
    [SerializeField, Range(0f, 1.5f)] private float masterVolume = 1f;

    [Header("Audio por Tipo (Fallback: 'Audio Genérico')")]
    [SerializeField] private PickupAudio rocketAudio = new PickupAudio();
    [SerializeField] private PickupAudio heartAudio = new PickupAudio();
    [SerializeField] private PickupAudio containerAudio = new PickupAudio();

    [Header("Audio Genérico (si un tipo no tiene clips)")]
    [SerializeField] private PickupAudio pickupAudio = new PickupAudio();

    [Header("Configuración")]
    [SerializeField] private bool trackStats = true;

    private AudioSource _audioSource;
    private int _rocketsCollected;
    private int _heartsCollected;
    private int _totalItemsCollected;
    private int _containersCollected;

    public event Action<string, int> OnItemPickedUp; // itemType, amount

    public static PickupManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PickupManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("PickupManager");
                    _instance = obj.AddComponent<PickupManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSource();
    }

    private void SetupAudioSource()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _audioSource.playOnAwake = false;
    }

    private void PlayFromPool(PickupAudio pool)
    {
        if (pool != null && pool.pickupClips != null && pool.pickupClips.Length > 0)
        {
            AudioClip clip = pool.pickupClips[UnityEngine.Random.Range(0, pool.pickupClips.Length)];
            if (clip != null)
            {
                _audioSource.PlayOneShot(clip, pool.volume * masterVolume);
                return;
            }
        }
        // Fallback al audio genérico
        if (pickupAudio != null && pickupAudio.pickupClips != null && pickupAudio.pickupClips.Length > 0)
        {
            AudioClip fallback = pickupAudio.pickupClips[UnityEngine.Random.Range(0, pickupAudio.pickupClips.Length)];
            if (fallback != null)
            {
                _audioSource.PlayOneShot(fallback, pickupAudio.volume * masterVolume);
            }
        }
    }

    /// <summary>
    /// Registra la recolección de un rocket
    /// </summary>
    public void RegisterRocketPickup(int amount = 1)
    {
        if (trackStats)
        {
            _rocketsCollected += amount;
            _totalItemsCollected += amount;
        }
        OnItemPickedUp?.Invoke("Rocket", amount);
        PlayFromPool(rocketAudio);
    }

    /// <summary>
    /// Registra la recolección de un corazón (vida curativa)
    /// </summary>
    public void RegisterHeartPickup(int amount = 1)
    {
        if (trackStats)
        {
            _heartsCollected += amount;
            _totalItemsCollected += amount;
        }
        OnItemPickedUp?.Invoke("Heart", amount);
        PlayFromPool(heartAudio);
    }

    /// <summary>
    /// Registra la recolección de un contenedor (aumenta vida máxima)
    /// </summary>
    public void RegisterHeartContainerPickup(int amount = 1)
    {
        if (trackStats)
        {
            _containersCollected += amount;
            _totalItemsCollected += amount;
        }
        OnItemPickedUp?.Invoke("Container", amount);
        PlayFromPool(containerAudio);
    }

    /// <summary>
    /// Obtiene estadísticas de recolección
    /// </summary>
    public void GetPickupStats(out int rockets, out int hearts, out int total)
    {
        rockets = _rocketsCollected;
        hearts = _heartsCollected;
        total = _totalItemsCollected;
    }

    /// <summary>
    /// Reinicia las estadísticas
    /// </summary>
    public void ResetStats()
    {
        _rocketsCollected = 0;
        _heartsCollected = 0;
        _totalItemsCollected = 0;
        _containersCollected = 0;
    }

    public int RocketsCollected => _rocketsCollected;
    public int HeartsCollected => _heartsCollected;
    public int TotalItemsCollected => _totalItemsCollected;
    public int ContainersCollected => _containersCollected;
}
