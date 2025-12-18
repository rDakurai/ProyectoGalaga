using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SliderSpecialAttack : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SpecialAttack specialAttack;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        if (specialAttack == null)
            specialAttack = FindObjectOfType<SpecialAttack>();

        if (slider == null)
            slider = GetComponentInChildren<Slider>(); // por si el script está en el padre
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (specialAttack != null)
            specialAttack.OnChargeChanged += UpdateBar;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (specialAttack != null)
            specialAttack.OnChargeChanged -= UpdateBar;
    }

    private void Start()
    {
        if (specialAttack != null)
            UpdateBar(specialAttack.Charge, specialAttack.MaxCharge);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindSpecial();
    }

    private void RebindSpecial()
    {
        if (specialAttack != null)
            specialAttack.OnChargeChanged -= UpdateBar;

        specialAttack = FindObjectOfType<SpecialAttack>();

        if (specialAttack != null)
        {
            specialAttack.OnChargeChanged += UpdateBar;
            UpdateBar(specialAttack.Charge, specialAttack.MaxCharge);
        }
        else if (slider != null)
        {
            slider.value = 0f;
        }
    }

    private void UpdateBar(float current, float max)
    {
        if (slider == null) return;

        slider.minValue = 0f;
        slider.maxValue = max;   // <- importantísimo
        slider.value = current;
    }
}

