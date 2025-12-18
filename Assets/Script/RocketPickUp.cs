using UnityEngine;

public class RocketPickup : MonoBehaviour
{
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var launcher = other.GetComponent<RocketLauncher>();
        if (launcher == null) return;

        launcher.AddRockets(amount);
        Destroy(gameObject);
    }
}
