using UnityEngine;

[System.Serializable]
public class WeightedAudioClip
{
    public AudioClip clip;
    
    [Range(0f, 100f)]
    public float weight = 100f;
}

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Sonidos del Jugador")]
    [SerializeField]
    private AudioSource shootSound;

    [SerializeField]
    private WeightedAudioClip[] shootClips; // Lista de clips de disparo

    [SerializeField]
    private AudioSource rocketSound;

    [SerializeField]
    private WeightedAudioClip[] rocketClips; // Lista de clips de cohete

    [SerializeField]
    private AudioSource specialAttackSound;

    [SerializeField]
    private WeightedAudioClip[] specialAttackClips; // Lista de clips de especial

    [SerializeField]
    private AudioSource hitSound;

    [SerializeField]
    private WeightedAudioClip[] hitClips; // Lista de clips de golpe

    [SerializeField]
    private AudioSource deathSound;

    [SerializeField]
    private WeightedAudioClip[] deathClips; // Lista de clips de muerte

    private static PlayerAudioManager _instance;

    private void Awake()
    {
        // Singleton para acceso global
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // Asegurar que no se reproduzcan al iniciar
        if (shootSound != null) shootSound.playOnAwake = false;
        if (rocketSound != null) rocketSound.playOnAwake = false;
        if (specialAttackSound != null) specialAttackSound.playOnAwake = false;
        if (hitSound != null) hitSound.playOnAwake = false;
        if (deathSound != null) deathSound.playOnAwake = false;
    }

    public static void PlayShoot()
    {
        if (_instance != null && _instance.shootSound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.shootClips);
            if (clip != null)
            {
                _instance.shootSound.PlayOneShot(clip);
            }
        }
    }

    public static void PlayRocket()
    {
        if (_instance != null && _instance.rocketSound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.rocketClips);
            if (clip != null)
            {
                _instance.rocketSound.PlayOneShot(clip);
            }
        }
    }

    public static void PlaySpecialAttack()
    {
        if (_instance != null && _instance.specialAttackSound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.specialAttackClips);
            if (clip != null)
            {
                _instance.specialAttackSound.PlayOneShot(clip);
            }
        }
    }

    public static void PlayHit()
    {
        if (_instance != null && _instance.hitSound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.hitClips);
            if (clip != null)
            {
                _instance.hitSound.PlayOneShot(clip);
            }
        }
    }

    public static void PlayDeath()
    {
        if (_instance != null && _instance.deathSound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.deathClips);
            if (clip != null)
            {
                _instance.deathSound.PlayOneShot(clip);
            }
        }
    }

    private AudioClip SelectRandomClip(WeightedAudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;

        // Calcular peso total
        float totalWeight = 0f;
        foreach (var weightedClip in clips)
        {
            if (weightedClip.clip != null)
            {
                totalWeight += weightedClip.weight;
            }
        }

        if (totalWeight <= 0f) return null;

        // Seleccionar clip basado en probabilidad
        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float accumulated = 0f;

        foreach (var weightedClip in clips)
        {
            if (weightedClip.clip == null) continue;

            accumulated += weightedClip.weight;
            if (roll < accumulated)
            {
                return weightedClip.clip;
            }
        }

        return null;
    }
}
