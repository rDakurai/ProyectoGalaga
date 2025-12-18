using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class RocketLauncher : MonoBehaviour
{
    [Header("Rockets")]
    [SerializeField] private int maxRockets = 3;
    [SerializeField] private int currentRockets = 0;

    [Header("Shoot")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private KeyCode shootKey = KeyCode.R;
    [SerializeField] private float cooldown = 0.25f;

    public int CurrentRockets => currentRockets;
    public int MaxRockets => maxRockets;
    public event Action<int, int> OnRocketsChanged;

    private float nextShootTime;

    private void Start()
    {
        OnRocketsChanged?.Invoke(currentRockets, maxRockets);
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
            TryShoot();
    }

    public void AddRockets(int amount = 1)
    {
        if (amount <= 0) return;

        currentRockets = Mathf.Clamp(currentRockets + amount, 0, maxRockets);
        OnRocketsChanged?.Invoke(currentRockets, maxRockets);
    }

    private void TryShoot()
    {
        if (Time.time < nextShootTime) return;
        if (currentRockets <= 0) return;
        if (rocketPrefab == null || firePoint == null) return;

        Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);

        currentRockets--;
        OnRocketsChanged?.Invoke(currentRockets, maxRockets);

        nextShootTime = Time.time + cooldown;
    }
}
