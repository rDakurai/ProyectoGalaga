using UnityEngine;

public class BulletGarbageCollector : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField, Min(0f)]
    private float borderThickness = 2f; // Grosor de las zonas de destrucción

    [SerializeField]
    private bool destroyEnemyBullets = true;

    [SerializeField]
    private bool destroyPlayerBullets = false;

    [SerializeField]
    private bool destroyEnemies = false;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("BulletGarbageCollector: No se encontró Main Camera");
            return;
        }

        CreateBorderTriggers();
    }

    private void CreateBorderTriggers()
    {
        // Obtener límites de la cámara en world space
        float height = _mainCamera.orthographicSize * 2f;
        float width = height * _mainCamera.aspect;

        Vector3 cameraPos = _mainCamera.transform.position;

        // Crear 4 triggers en los bordes
        CreateBorderTrigger("LeftBorder", 
            new Vector3(cameraPos.x - width / 2f - borderThickness / 2f, cameraPos.y, 0f),
            new Vector2(borderThickness, height + borderThickness * 2f));

        CreateBorderTrigger("RightBorder",
            new Vector3(cameraPos.x + width / 2f + borderThickness / 2f, cameraPos.y, 0f),
            new Vector2(borderThickness, height + borderThickness * 2f));

        CreateBorderTrigger("TopBorder",
            new Vector3(cameraPos.x, cameraPos.y + height / 2f + borderThickness / 2f, 0f),
            new Vector2(width + borderThickness * 2f, borderThickness));

        CreateBorderTrigger("BottomBorder",
            new Vector3(cameraPos.x, cameraPos.y - height / 2f - borderThickness / 2f, 0f),
            new Vector2(width + borderThickness * 2f, borderThickness));
    }

    private void CreateBorderTrigger(string name, Vector3 position, Vector2 size)
    {
        GameObject border = new GameObject(name);
        border.transform.SetParent(transform);
        border.transform.position = position;
        border.layer = gameObject.layer;

        BoxCollider2D collider = border.AddComponent<BoxCollider2D>();
        collider.size = size;
        collider.isTrigger = true;

        BulletDestroyZone destroyZone = border.AddComponent<BulletDestroyZone>();
        destroyZone.Initialize(destroyEnemyBullets, destroyPlayerBullets, destroyEnemies);
    }

    private void OnDrawGizmos()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_mainCamera == null) return;

        // Dibujar los bordes en el editor
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);

        float height = _mainCamera.orthographicSize * 2f;
        float width = height * _mainCamera.aspect;
        Vector3 cameraPos = _mainCamera.transform.position;

        // Izquierda
        Gizmos.DrawCube(
            new Vector3(cameraPos.x - width / 2f - borderThickness / 2f, cameraPos.y, 0f),
            new Vector3(borderThickness, height + borderThickness * 2f, 0.1f));

        // Derecha
        Gizmos.DrawCube(
            new Vector3(cameraPos.x + width / 2f + borderThickness / 2f, cameraPos.y, 0f),
            new Vector3(borderThickness, height + borderThickness * 2f, 0.1f));

        // Arriba
        Gizmos.DrawCube(
            new Vector3(cameraPos.x, cameraPos.y + height / 2f + borderThickness / 2f, 0f),
            new Vector3(width + borderThickness * 2f, borderThickness, 0.1f));

        // Abajo
        Gizmos.DrawCube(
            new Vector3(cameraPos.x, cameraPos.y - height / 2f - borderThickness / 2f, 0f),
            new Vector3(width + borderThickness * 2f, borderThickness, 0.1f));
    }
}

public class BulletDestroyZone : MonoBehaviour
{
    private bool _destroyEnemyBullets;
    private bool _destroyPlayerBullets;
    private bool _destroyEnemies;

    public void Initialize(bool destroyEnemy, bool destroyPlayer, bool destroyEnemies)
    {
        _destroyEnemyBullets = destroyEnemy;
        _destroyPlayerBullets = destroyPlayer;
        _destroyEnemies = destroyEnemies;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_destroyEnemyBullets && other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            return;
        }

        if (_destroyPlayerBullets && other.CompareTag("PlayerAttack"))
        {
            Destroy(other.gameObject);
            return;
        }

        if (_destroyEnemies && other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            return;
        }
    }
}
