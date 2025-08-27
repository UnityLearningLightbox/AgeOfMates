using UnityEngine;

public class NPCEntrega : MonoBehaviour
{
    public string objetoEsperadoTag;
    public float rangoEntrega = 2f;
    public bool objetoEntregado = false;
    public bool objetoSoltado = false;

    [SerializeField] GameObject activeDialogue;
    [SerializeField] GameObject canvasDialogue;
    [SerializeField] CompassQuestMarker compassCanvas;

    private void Start()
    {
        compassCanvas = GetComponent<CompassQuestMarker>();
    }

    private void Update()
    {
        if (objetoEntregado)
        {
            activeDialogue.SetActive(false);
            canvasDialogue.SetActive(false);
            compassCanvas.image.enabled = false;
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
