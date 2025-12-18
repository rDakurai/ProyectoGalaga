using  System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ParallaxMovements : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] bool scrollLeft;
    [SerializeField] bool useSeamlessLoop = true; // Activar/desactivar el clon

    float singleTextureWidth;
    GameObject clonedBackground;

    void Start()
    { 
        SetupTexture();
        if(scrollLeft)
        {
            moveSpeed = -moveSpeed;
        }
        
        // Crear un clon del fondo para loop seamless solo si está activado
        if (useSeamlessLoop)
        {
            CreateClone();
        }
    }

    void SetupTexture()
    { 
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        singleTextureWidth = sprite.texture.width / sprite.pixelsPerUnit;
    }
    
    void CreateClone()
    {
        // Crear duplicado del objeto
        clonedBackground = Instantiate(gameObject, transform.parent);
        clonedBackground.name = gameObject.name + "_Clone";
        
        // Posicionar el clon al lado según la dirección del scroll
        if (moveSpeed < 0) // Scroll a izquierda
        {
            clonedBackground.transform.position = transform.position + new Vector3(singleTextureWidth, 0f, 0f);
        }
        else // Scroll a derecha
        {
            clonedBackground.transform.position = transform.position - new Vector3(singleTextureWidth, 0f, 0f);
        }
        
        // Desactivar el script del clon para que solo este maneje ambos
        ParallaxMovements cloneScript = clonedBackground.GetComponent<ParallaxMovements>();
        if (cloneScript != null)
        {
            cloneScript.enabled = false;
        }
    }

    void Scroll()
    { 
        float delta = moveSpeed * Time.deltaTime;
        transform.position += new Vector3(delta, 0f, 0f);
        
        // Mover el clon también
        if (clonedBackground != null)
        {
            clonedBackground.transform.position += new Vector3(delta, 0f, 0f);
        }
    }
    
    void CheckReset()
    {
        if (useSeamlessLoop)
        {
            // Loop con clon - reiniciar cuando sale completamente del ancho de la textura
            if (moveSpeed < 0) // Scroll izquierda
            {
                if (transform.position.x <= -singleTextureWidth)
                {
                    transform.position += new Vector3(singleTextureWidth * 2, 0f, 0f);
                }
                if (clonedBackground != null && clonedBackground.transform.position.x <= -singleTextureWidth)
                {
                    clonedBackground.transform.position += new Vector3(singleTextureWidth * 2, 0f, 0f);
                }
            }
            else // Scroll derecha
            {
                if (transform.position.x >= singleTextureWidth)
                {
                    transform.position -= new Vector3(singleTextureWidth * 2, 0f, 0f);
                }
                if (clonedBackground != null && clonedBackground.transform.position.x >= singleTextureWidth)
                {
                    clonedBackground.transform.position -= new Vector3(singleTextureWidth * 2, 0f, 0f);
                }
            }
        }
        else
        {
            // Loop simple sin clon - reiniciar al centro
            if (moveSpeed < 0 && transform.position.x <= -singleTextureWidth)
            {
                transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
            }
            else if (moveSpeed > 0 && transform.position.x >= singleTextureWidth)
            {
                transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
            }
        }
    }

    void Update()
    {
        Scroll();
        CheckReset();
    }
    
    void OnDestroy()
    {
        // Limpiar el clon cuando se destruya el original
        if (clonedBackground != null)
        {
            Destroy(clonedBackground);
        }
    }
}