using UnityEngine;

public class NPCEntrega : MonoBehaviour
{
    public string objetoEsperadoTag;  // Tag del objeto que espera
    public float rangoEntrega = 2f;   // Distancia máxima para recogerlo

    private bool objetoEntregado = false;

    [SerializeField] GameObject activeDialogue;
    [SerializeField] GameObject canvasDialogue;

    private void Update()
    {
        //if (objetoEntregado) return;
        if (objetoEntregado == true)
        {
            Debug.Log("Por que leches no entras perro?");
            // Desactivar la quest y el icono en la brujula al entregar el objeto
            activeDialogue.SetActive(false);
            canvasDialogue.SetActive(false);
        }

        // Buscar el objeto dropeado en la escena
        GameObject objeto = GameObject.FindWithTag(objetoEsperadoTag);

        if (objeto != null)
        {
            float distancia = Vector3.Distance(transform.position, objeto.transform.position);

            if (distancia <= rangoEntrega)
            {
                // "Recoger" el objeto
                Destroy(objeto); // Borra el objeto del mapa
                Debug.Log("¡Gracias por entregar " + objetoEsperadoTag + "!");
                objetoEntregado = true;
            }
        }
    }

    // Dibujar el rango en la escena con Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangoEntrega);
    }
}
