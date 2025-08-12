using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Configuración de Objetos")]
    public GameObject[] prefabsObjetos;   // Prefabs para dropear
    public string[] nombresObjetos;       // Tags de objetos que puedes recoger

    [Header("Drop")]
    public Transform puntoDrop; // Empty delante del jugador para dropear

    private int[] inventario;
    private int indiceSeleccionado = 0; // Objeto seleccionado para dropear

    void Start()
    {
        inventario = new int[prefabsObjetos.Length];

        if (puntoDrop == null)
        {
            Debug.LogWarning("No asignaste puntoDrop, se usará el objeto mismo.");
            puntoDrop = transform;
        }
    }

    void Update()
    {
        // Recoger con E
        if (Input.GetKeyDown(KeyCode.E))
        {
            IntentarRecoger();
        }

        // Dropear con Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropearObjeto();
        }

        // Cambiar objeto seleccionado con teclas 1,2,3...
        CambiarObjetoSeleccionado();
    }

    void IntentarRecoger()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            for (int i = 0; i < nombresObjetos.Length; i++)
            {
                if (hit.collider.CompareTag(nombresObjetos[i]))
                {
                    inventario[i]++;
                    indiceSeleccionado = i;
                    Debug.Log($"Recogido {nombresObjetos[i]}. Total: {inventario[i]}");
                    Destroy(hit.collider.gameObject);
                    return;
                }
            }
            Debug.Log("El objeto tocado no está en la lista de recogibles.");
        }
        else
        {
            Debug.Log("No hay objeto para recoger delante.");
        }
    }

    void DropearObjeto()
    {
        if (inventario[indiceSeleccionado] > 0)
        {
            Vector3 dropPos = puntoDrop.position + puntoDrop.forward * 1.5f;
            dropPos.y += 0.5f; // Para que no quede hundido

            GameObject obj = Instantiate(prefabsObjetos[indiceSeleccionado], dropPos, Quaternion.identity);
            if (obj.GetComponent<Rigidbody>() == null)
            {
                obj.AddComponent<Rigidbody>();
            }

            inventario[indiceSeleccionado]--;
            Debug.Log($"Dropeado {nombresObjetos[indiceSeleccionado]}. Quedan {inventario[indiceSeleccionado]}");
        }
        else
        {
            Debug.Log($"No tienes {nombresObjetos[indiceSeleccionado]} para dropear.");
        }
    }

    void CambiarObjetoSeleccionado()
    {
        for (int i = 0; i < nombresObjetos.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (inventario[i] > 0)
                {
                    indiceSeleccionado = i;
                    Debug.Log($"Seleccionado: {nombresObjetos[i]}");
                }
                else
                {
                    Debug.Log($"No tienes {nombresObjetos[i]} para seleccionar.");
                }
            }
        }
    }
}
