using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SpecialAttack : MonoBehaviour
{
    [Header("Carga")]
    [SerializeField] private float maxCharge = 100f;
    [SerializeField] private float chargePerSecond = 20f;

    [Header("Carga por golpe")]
    [SerializeField] private float chargePerHit = 10f;

    [Header("Input")]
    [SerializeField] private KeyCode specialKey = KeyCode.Space;

    [Header("Prefab del especial")]
    [SerializeField] private GameObject specialPrefab;
    [SerializeField] private Transform firePoint;

    public float Charge { get; private set; }
    public float MaxCharge => maxCharge;

    public event Action<float, float> OnChargeChanged;

    private void Start()
    {
        Charge = 0f;
        OnChargeChanged?.Invoke(Charge, maxCharge);
    }

    private void Update()
    {
        AddCharge(chargePerSecond * Time.deltaTime);

        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame && Charge >= maxCharge)
            UseSpecial();
    }

    public void AddCharge(float amount)
    {
        if (amount <= 0f) return;

        float old = Charge;
        Charge = Mathf.Clamp(Charge + amount, 0f, maxCharge);

        if (!Mathf.Approximately(old, Charge))
            OnChargeChanged?.Invoke(Charge, maxCharge);
    }

    public void AddChargeFromHit()
    {
        AddCharge(chargePerHit);
    }

    private void UseSpecial()
    {
        if (specialPrefab == null || firePoint == null) return;

        Instantiate(specialPrefab, firePoint.position, firePoint.rotation);

        // Reproducir sonido de ataque especial
        PlayerAudioManager.PlaySpecialAttack();

        Charge = 0f;
        OnChargeChanged?.Invoke(Charge, maxCharge);
    }
}
