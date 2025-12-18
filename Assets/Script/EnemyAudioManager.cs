using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource shootSound;

    [SerializeField]
    private WeightedAudioClip[] shootClips;

    [Header("Shooter Attack 1")]
    [SerializeField]
    private AudioSource shooterAttack1Sound;

    [SerializeField]
    private WeightedAudioClip[] shooterAttack1Clips;

    [SerializeField]
    private bool shooterAttack1Loop = false;

    [Header("Shooter Attack 2")]
    [SerializeField]
    private AudioSource shooterAttack2Sound;

    [SerializeField]
    private WeightedAudioClip[] shooterAttack2Clips;

    [SerializeField]
    private bool shooterAttack2Loop = false;

    [SerializeField]
    private AudioSource hitSound;

    [SerializeField]
    private WeightedAudioClip[] hitClips;

    [SerializeField]
    private AudioSource deathSound;

    [SerializeField]
    private WeightedAudioClip[] deathClips;

    [Header("Kamikaze Sounds")]
    [SerializeField]
    private AudioSource kamikazeRushSound;

    [SerializeField]
    private WeightedAudioClip[] kamikazeRushClips;

    [Header("Boss Sounds")]
    [SerializeField]
    private AudioSource bossAttack1Sound;

    [SerializeField]
    private WeightedAudioClip[] bossAttack1Clips;

    [SerializeField, Min(0f)]
    private float bossAttack1Delay = 0f;

    [SerializeField]
    private bool bossAttack1Loop = false;

    [SerializeField]
    private AudioSource bossAttack2Sound;

    [SerializeField]
    private WeightedAudioClip[] bossAttack2Clips;

    [SerializeField, Min(0f)]
    private float bossAttack2Delay = 0f;

    [SerializeField]
    private bool bossAttack2Loop = false;

    [SerializeField]
    private AudioSource bossDeathSound;

    [SerializeField]
    private WeightedAudioClip[] bossDeathClips;

    [SerializeField, Min(0f)]
    private float bossDeathDelay = 0f;

    [SerializeField]
    private bool bossDeathLoop = false;

    private static EnemyAudioManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // Asegurar que no se reproduzcan al iniciar
        if (shootSound != null) shootSound.playOnAwake = false;
        if (shooterAttack1Sound != null) shooterAttack1Sound.playOnAwake = false;
        if (shooterAttack2Sound != null) shooterAttack2Sound.playOnAwake = false;
        if (hitSound != null) hitSound.playOnAwake = false;
        if (deathSound != null) deathSound.playOnAwake = false;
        if (kamikazeRushSound != null) kamikazeRushSound.playOnAwake = false;
        if (bossAttack1Sound != null) bossAttack1Sound.playOnAwake = false;
        if (bossAttack2Sound != null) bossAttack2Sound.playOnAwake = false;
        if (bossDeathSound != null) bossDeathSound.playOnAwake = false;
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

    public static void PlayShooterAttack1()
    {
        if (_instance != null && _instance.shooterAttack1Sound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.shooterAttack1Clips);
            if (clip != null)
            {
                if (_instance.shooterAttack1Loop)
                {
                    _instance.PlayAudio(_instance.shooterAttack1Sound, clip, true);
                }
                else
                {
                    _instance.shooterAttack1Sound.PlayOneShot(clip);
                }
            }
        }
    }

    public static void PlayShooterAttack2()
    {
        if (_instance != null && _instance.shooterAttack2Sound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.shooterAttack2Clips);
            if (clip != null)
            {
                if (_instance.shooterAttack2Loop)
                {
                    _instance.PlayAudio(_instance.shooterAttack2Sound, clip, true);
                }
                else
                {
                    _instance.shooterAttack2Sound.PlayOneShot(clip);
                }
            }
        }
    }

    public static void StopShooterAttack1()
    {
        if (_instance != null && _instance.shooterAttack1Sound != null)
        {
            _instance.shooterAttack1Sound.Stop();
        }
    }

    public static void StopShooterAttack2()
    {
        if (_instance != null && _instance.shooterAttack2Sound != null)
        {
            _instance.shooterAttack2Sound.Stop();
        }
    }

    public static bool ShooterAttack1LoopEnabled => _instance != null && _instance.shooterAttack1Loop;
    public static bool ShooterAttack2LoopEnabled => _instance != null && _instance.shooterAttack2Loop;

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

    public static void PlayKamikazeRush()
    {
        if (_instance != null && _instance.kamikazeRushSound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.kamikazeRushClips);
            if (clip != null)
            {
                _instance.kamikazeRushSound.PlayOneShot(clip);
            }
        }
    }

    public static void PlayBossAttack1()
    {
        if (_instance != null && _instance.bossAttack1Sound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.bossAttack1Clips);
            if (clip != null)
            {
                if (_instance.bossAttack1Delay > 0f)
                {
                    _instance.StartCoroutine(_instance.PlayAudioWithDelay(_instance.bossAttack1Sound, clip, _instance.bossAttack1Delay, _instance.bossAttack1Loop));
                }
                else
                {
                    _instance.PlayAudio(_instance.bossAttack1Sound, clip, _instance.bossAttack1Loop);
                }
            }
        }
    }

    public static void PlayBossAttack2()
    {
        if (_instance != null && _instance.bossAttack2Sound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.bossAttack2Clips);
            if (clip != null)
            {
                if (_instance.bossAttack2Delay > 0f)
                {
                    _instance.StartCoroutine(_instance.PlayAudioWithDelay(_instance.bossAttack2Sound, clip, _instance.bossAttack2Delay, _instance.bossAttack2Loop));
                }
                else
                {
                    _instance.PlayAudio(_instance.bossAttack2Sound, clip, _instance.bossAttack2Loop);
                }
            }
        }
    }

    public static void PlayBossDeath()
    {
        if (_instance != null && _instance.bossDeathSound != null)
        {
            AudioClip clip = _instance.SelectRandomClip(_instance.bossDeathClips);
            if (clip != null)
            {
                if (_instance.bossDeathDelay > 0f)
                {
                    _instance.StartCoroutine(_instance.PlayAudioWithDelay(_instance.bossDeathSound, clip, _instance.bossDeathDelay, _instance.bossDeathLoop));
                }
                else
                {
                    _instance.PlayAudio(_instance.bossDeathSound, clip, _instance.bossDeathLoop);
                }
            }
        }
    }

    private void PlayAudio(AudioSource audioSource, AudioClip clip, bool loop)
    {
        if (loop)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private System.Collections.IEnumerator PlayAudioWithDelay(AudioSource audioSource, AudioClip clip, float delay, bool loop)
    {
        yield return new WaitForSeconds(delay);
        PlayAudio(audioSource, clip, loop);
    }

    public static void StopBossAttack1()
    {
        if (_instance != null && _instance.bossAttack1Sound != null)
        {
            _instance.bossAttack1Sound.Stop();
        }
    }

    public static void StopBossAttack2()
    {
        if (_instance != null && _instance.bossAttack2Sound != null)
        {
            _instance.bossAttack2Sound.Stop();
        }
    }

    public static void StopBossDeath()
    {
        if (_instance != null && _instance.bossDeathSound != null)
        {
            _instance.bossDeathSound.Stop();
        }
    }

    private AudioClip SelectRandomClip(WeightedAudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;

        float totalWeight = 0f;
        foreach (var weightedClip in clips)
        {
            if (weightedClip.clip != null)
            {
                totalWeight += weightedClip.weight;
            }
        }

        if (totalWeight <= 0f) return null;

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
