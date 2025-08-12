using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Configuración de Objetos")]
    public GameObject[] prefabsObjetos;   // Prefabs de los objetos para dropear
    public string[] nombresObjetos;       // Tags de los objetos que se pueden recoger (deben coincidir con prefab y en escena)

    [Header("Raycast para recoger")]
    public float distanciaRecoger = 3f;
    public LayerMask capaObjetos;

    [Header("Drop")]
    public Transform puntoDrop; // Empty delante del jugador donde aparecerán los objetos dropeados

    private int[] inventario;   // Cantidad de cada objeto en inventario

    private int indiceSeleccionado = 0; // Para elegir qué objeto dropear (puedes ampliar para cambiar con teclas)

    void Start()
    {
        inventario = new int[prefabsObjetos.Length];
        if (puntoDrop == null)
        {
            Debug.LogWarning("No asignaste el punto de drop, se usará la posición del jugador.");
            puntoDrop = transform;
        }
    }

    void Update()
    {
        // Recoger con E
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, distanciaRecoger, capaObjetos))
            {
                for (int i = 0; i < nombresObjetos.Length; i++)
                {
                    if (hit.collider.CompareTag(nombresObjetos[i]))
                    {
                        inventario[i]++;
                        Debug.Log($"Recogido {nombresObjetos[i]}. Total: {inventario[i]}");
                        Destroy(hit.collider.gameObject);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("No hay objeto para recoger delante.");
            }
        }

        // Cambiar objeto seleccionado con teclas 1,2,3,...
        for (int i = 0; i < nombresObjetos.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (inventario[i] > 0)
                {
                    indiceSeleccionado = i;
                    Debug.Log($"Objeto seleccionado para dropear: {nombresObjetos[i]}");
                }
                else
                {
                    Debug.Log($"No tienes {nombresObjetos[i]} en el inventario para seleccionar.");
                }
            }
        }

        // Dropear objeto seleccionado con Q (puedes cambiar tecla)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (inventario[indiceSeleccionado] > 0)
            {
                Vector3 dropPos = puntoDrop.position + puntoDrop.forward * 2f;
                Instantiate(prefabsObjetos[indiceSeleccionado], dropPos, Quaternion.identity);
                inventario[indiceSeleccionado]--;
                Debug.Log($"Dropeado {nombresObjetos[indiceSeleccionado]}. Quedan {inventario[indiceSeleccionado]}");
            }
            else
            {
                Debug.Log($"No tienes {nombresObjetos[indiceSeleccionado]} para dropear.");
            }
        }
    }
}
