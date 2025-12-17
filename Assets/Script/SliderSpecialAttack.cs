using UnityEngine;
using UnityEngine.UI;

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
        if (specialAttack != null)
            specialAttack.OnChargeChanged += UpdateBar;
    }

    private void OnDisable()
    {
        if (specialAttack != null)
            specialAttack.OnChargeChanged -= UpdateBar;
    }

    private void Start()
    {
        if (specialAttack != null)
            UpdateBar(specialAttack.Charge, specialAttack.MaxCharge);
    }

    private void UpdateBar(float current, float max)
    {
        if (slider == null) return;

        slider.minValue = 0f;
        slider.maxValue = max;   // <- importantísimo
        slider.value = current;
    }
}

