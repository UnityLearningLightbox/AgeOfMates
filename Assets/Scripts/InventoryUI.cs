using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Configuración de Objetos")]
    public GameObject[] prefabsObjetos;
    public string[] nombresObjetos;

    [Header("Drop")]
    public Transform puntoDrop;

    [Header("UI")]
    public Image[] imagenesObjetosUI;   // Imagenes fijas para cada objeto
    public GameObject selectorUI;       // Un objeto UI (ej: un marco) que indica el seleccionado

    private int[] inventario;
    private int indiceSeleccionado = 0;

    void Start()
    {
        inventario = new int[prefabsObjetos.Length];

        if (puntoDrop == null)
        {
            puntoDrop = transform;
        }

        ActualizarUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            IntentarRecoger();

        if (Input.GetKeyDown(KeyCode.Q))
            DropearObjeto();

        CambiarObjetoConScroll();
        CambiarObjetoConTeclas();
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
                    Destroy(hit.collider.gameObject);
                    ActualizarUI();
                    return;
                }
            }
        }
    }

    void DropearObjeto()
    {
        if (inventario[indiceSeleccionado] > 0)
        {
            Vector3 dropPos = puntoDrop.position + puntoDrop.forward * 1.5f;
            dropPos.y += 0.5f;

            GameObject obj = Instantiate(prefabsObjetos[indiceSeleccionado], dropPos, Quaternion.identity);
            if (obj.GetComponent<Rigidbody>() == null)
                obj.AddComponent<Rigidbody>();

            inventario[indiceSeleccionado]--;
            ActualizarUI();
        }
    }

    void CambiarObjetoConScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SiguienteObjeto();
        else if (scroll < 0f) ObjetoAnterior();
    }

    void CambiarObjetoConTeclas()
    {
        for (int i = 0; i < nombresObjetos.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && inventario[i] > 0)
            {
                indiceSeleccionado = i;
                ActualizarUI();
            }
        }
    }

    void SiguienteObjeto()
    {
        int startIndex = indiceSeleccionado;
        do
        {
            indiceSeleccionado = (indiceSeleccionado + 1) % inventario.Length;
            if (inventario[indiceSeleccionado] > 0)
            {
                ActualizarUI();
                return;
            }
        } while (indiceSeleccionado != startIndex);
    }

    void ObjetoAnterior()
    {
        int startIndex = indiceSeleccionado;
        do
        {
            indiceSeleccionado = (indiceSeleccionado - 1 + inventario.Length) % inventario.Length;
            if (inventario[indiceSeleccionado] > 0)
            {
                ActualizarUI();
                return;
            }
        } while (indiceSeleccionado != startIndex);
    }

    void ActualizarUI()
    {
        for (int i = 0; i < imagenesObjetosUI.Length; i++)
        {
            imagenesObjetosUI[i].gameObject.SetActive(inventario[i] > 0);
            // Cambiar color para el seleccionado, ejemplo:
            if (i == indiceSeleccionado && inventario[i] > 0)
            {
                imagenesObjetosUI[i].color = Color.yellow;
                if (selectorUI != null)
                {
                    selectorUI.transform.position = imagenesObjetosUI[i].transform.position;
                    selectorUI.SetActive(true);
                }
            }
            else
            {
                imagenesObjetosUI[i].color = Color.white;
            }
        }
    }
}
