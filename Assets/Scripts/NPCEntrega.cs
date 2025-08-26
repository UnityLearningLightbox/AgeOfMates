using UnityEngine;

public class NPCEntrega : MonoBehaviour
{
    public string objetoEsperadoTag;
    public float rangoEntrega = 2f;
    public bool objetoEntregado = false;
    public bool objetoSoltado = false;

    [SerializeField] private GameObject activeDialogue;
    [SerializeField] private GameObject canvasDialogue;

    private void Update()
    {
        if (objetoEntregado)
        {
            if (activeDialogue.activeSelf || canvasDialogue.activeSelf)
            {
                activeDialogue.SetActive(false);
                canvasDialogue.SetActive(false);
            }
            return;
        }

        if (objetoSoltado)
        {
            GameObject objeto = GameObject.FindWithTag(objetoEsperadoTag);
            if (objeto != null)
            {
                float distancia = Vector3.Distance(transform.position, objeto.transform.position);
                Debug.Log($"Distancia al objeto: {distancia}");

                if (distancia <= rangoEntrega)
                {
                    Debug.Log($"¡Gracias por entregar {objetoEsperadoTag}!");
                    objetoEntregado = true;
                    Destroy(objeto);
                }
            }
            else
            {
                Debug.Log("No encuentro el objeto con el tag: " + objetoEsperadoTag);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangoEntrega);
    }
}
