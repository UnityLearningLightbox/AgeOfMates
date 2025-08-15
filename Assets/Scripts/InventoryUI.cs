using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI del Inventario")]
    public GameObject objeto1UI;   // Icono en la UI
    public GameObject prefabObjeto1; // Prefab que se va a soltar
    public Transform puntoDrop;    // Empty delante del jugador

    [Header("Raycast")]
    public float distanciaRecogida = 3f;
    public LayerMask capaObjetos;

    private bool tieneObjeto1 = false; // Flag para saber si lo tenemos

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            IntentarRecoger();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropearObjeto();
        }
    }

    void IntentarRecoger()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaRecogida, capaObjetos))
        {
            if (hit.collider.CompareTag("Objeto1") && !tieneObjeto1)
            {
                // Activar icono UI
                if (objeto1UI != null)
                    objeto1UI.SetActive(true);

                // Guardar que lo tenemos
                tieneObjeto1 = true;

                // Quitar el objeto de la escena
                Destroy(hit.collider.gameObject);
            }
        }
    }

    void DropearObjeto()
    {
        if (tieneObjeto1 && prefabObjeto1 != null && puntoDrop != null)
        {
            // Calcular posición delante del puntoDrop
            Vector3 dropPos = puntoDrop.position + puntoDrop.forward * 1.5f;
            dropPos.y += 0.5f;

            // Instanciar objeto
            GameObject nuevoObjeto = Instantiate(prefabObjeto1, dropPos, Quaternion.identity);

            // Añadir Rigidbody si no lo tiene
            if (nuevoObjeto.GetComponent<Rigidbody>() == null)
            {
                nuevoObjeto.AddComponent<Rigidbody>();
            }

            // Desactivar icono UI
            if (objeto1UI != null)
                objeto1UI.SetActive(false);

            // Marcar que ya no lo tenemos
            tieneObjeto1 = false;
        }
    }
}
