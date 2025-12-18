using UnityEngine;

public class PlayerBoundsRestriction : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField, Min(0f)]
    private float padding = 0.5f; // Distancia desde el borde de la pantalla

    [Header("Límites Personalizados (opcional)")]
    [SerializeField]
    private bool useCustomBounds = false;

    [SerializeField]
    private float customMinX = -10f;

    [SerializeField]
    private float customMaxX = 10f;

    [SerializeField]
    private float customMinY = -5f;

    [SerializeField]
    private float customMaxY = 5f;

    private Camera _mainCamera;
    private float _minX, _maxX, _minY, _maxY;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("PlayerBoundsRestriction: No se encontró Main Camera");
            enabled = false;
            return;
        }

        CalculateBounds();
    }

    private void CalculateBounds()
    {
        if (useCustomBounds)
        {
            _minX = customMinX;
            _maxX = customMaxX;
            _minY = customMinY;
            _maxY = customMaxY;
        }
        else
        {
            // Calcular límites basados en la cámara
            float height = _mainCamera.orthographicSize;
            float width = height * _mainCamera.aspect;

            Vector3 cameraPos = _mainCamera.transform.position;

            _minX = cameraPos.x - width + padding;
            _maxX = cameraPos.x + width - padding;
            _minY = cameraPos.y - height + padding;
            _maxY = cameraPos.y + height - padding;
        }
    }

    private void LateUpdate()
    {
        // Clampear posición del jugador dentro de los límites
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
        pos.y = Mathf.Clamp(pos.y, _minY, _maxY);
        transform.position = pos;
    }

    private void OnDrawGizmos()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_mainCamera == null) return;

        // Recalcular bounds para el editor
        float tempMinX, tempMaxX, tempMinY, tempMaxY;

        if (useCustomBounds)
        {
            tempMinX = customMinX;
            tempMaxX = customMaxX;
            tempMinY = customMinY;
            tempMaxY = customMaxY;
        }
        else
        {
            float height = _mainCamera.orthographicSize;
            float width = height * _mainCamera.aspect;
            Vector3 cameraPos = _mainCamera.transform.position;

            tempMinX = cameraPos.x - width + padding;
            tempMaxX = cameraPos.x + width - padding;
            tempMinY = cameraPos.y - height + padding;
            tempMaxY = cameraPos.y + height - padding;
        }

        // Dibujar límites en el editor
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);

        Vector3 topLeft = new Vector3(tempMinX, tempMaxY, 0f);
        Vector3 topRight = new Vector3(tempMaxX, tempMaxY, 0f);
        Vector3 bottomLeft = new Vector3(tempMinX, tempMinY, 0f);
        Vector3 bottomRight = new Vector3(tempMaxX, tempMinY, 0f);

        // Dibujar rectángulo de límites
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Dibujar esquinas
        Gizmos.DrawWireSphere(topLeft, 0.2f);
        Gizmos.DrawWireSphere(topRight, 0.2f);
        Gizmos.DrawWireSphere(bottomLeft, 0.2f);
        Gizmos.DrawWireSphere(bottomRight, 0.2f);
    }
}
