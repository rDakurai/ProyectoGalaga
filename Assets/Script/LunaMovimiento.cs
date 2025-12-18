using UnityEngine;

public class LunaMovimiento : MonoBehaviour
{
    [SerializeField] private float velocidad = 2f; // Velocidad del movimiento configurable
    [SerializeField] private float alturaArco = 3f; // Altura del arco (cuánto baja)
    [SerializeField] private float distanciaRecorrido = 15f; // Distancia horizontal total a recorrer
    
    private float posicionInicial; // Posición X inicial
       private float posicionInicialY; // Posición Y inicial
    private float tiempoTranscurrido = 0f; // Tiempo total transcurrido
    private float tiempoTotal; // Tiempo para completar un ciclo
    private Camera camara;
    
    void Start()
    {
        camara = Camera.main;
        posicionInicial = transform.position.x;
           posicionInicialY = transform.position.y; // Guardar posición Y inicial
        
        // Calcular el tiempo total necesario basado en la velocidad
        tiempoTotal = distanciaRecorrido / velocidad;
    }

    void Update()
    {
        // Incrementar el tiempo transcurrido
        tiempoTranscurrido += Time.deltaTime;
        
        // Si completó el ciclo, reiniciar
        if (tiempoTranscurrido >= tiempoTotal)
        {
            tiempoTranscurrido = 0f;
        }
        
        // Calcular progreso normalizado (0 a 1)
        float progreso = tiempoTranscurrido / tiempoTotal;
        
        // Movimiento horizontal (de derecha a izquierda)
        float nuevaX = posicionInicial - (distanciaRecorrido * progreso);
        
        // Movimiento vertical en forma de arco (baja lentamente)
        // Usando una parábola invertida para simular el arco
            float nuevaY = posicionInicialY - (alturaArco * progreso);
        
        // Aplicar la nueva posición
        transform.position = new Vector3(nuevaX, nuevaY, transform.position.z);
    }
}
