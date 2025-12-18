using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentHUD : MonoBehaviour
{
    public static PersistentHUD Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Opcional: Método para activar/desactivar elementos del HUD
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
