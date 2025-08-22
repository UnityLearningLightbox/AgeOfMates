using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [System.Serializable]
    public class ItemData
    {
        public string tagObjeto;
        public GameObject iconoUI;
        public GameObject prefab;
        public bool tiene;
    }

    [Header("Objetos del Inventario")]
    public List<ItemData> objetos = new List<ItemData>();

    [Header("Drop")]
    public Transform puntoDrop;

    [Header("Raycast")]
    public float distanciaRecogida = 3f;
    public LayerMask capaObjetos;

    [Header("UI")]
    public Slider recogerSlider;
    public float tiempoParaRecoger = 3f;

    private int indiceSeleccionado = 0;
    private RaycastHit hitActual;
    private bool mirandoObjeto = false;
    private float progresoRecogida = 0f;

    void Start()
    {
        if (recogerSlider != null)
        {
            //recogerSlider.gameObject.SetActive(false);
            recogerSlider.minValue = 0f;
            recogerSlider.maxValue = 1f;
            recogerSlider.value = 0f;
        }
    }

    void Update()
    {
        DetectarObjetoFrente();

        if (mirandoObjeto && hitActual.collider != null)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!recogerSlider.gameObject.activeSelf)
                    recogerSlider.gameObject.SetActive(true);

                progresoRecogida += Time.deltaTime / tiempoParaRecoger;
                recogerSlider.value = progresoRecogida;

                Debug.Log($"Progreso: {progresoRecogida}");

                if (progresoRecogida >= 1f)
                {
                    Debug.Log("Recogida completada. Ejecutando RecogerObjeto()");
                    RecogerObjeto(hitActual);
                    ReiniciarSlider();
                }
            }
            else
            {
                ReiniciarSlider();
            }
        }
        else
        {
            ReiniciarSlider();
        }

        if (Input.GetKeyDown(KeyCode.Q))
            DropearObjetoSeleccionado();

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            indiceSeleccionado = (indiceSeleccionado + (scroll > 0 ? 1 : -1) + objetos.Count) % objetos.Count;
            Debug.Log("Objeto seleccionado: " + objetos[indiceSeleccionado].tagObjeto);
        }
    }

    void DetectarObjetoFrente()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hitActual, distanciaRecogida, capaObjetos))
        {
            Debug.Log("Mirando objeto: " + hitActual.collider.name);
            mirandoObjeto = true;
        }
        else
        {
            mirandoObjeto = false;
        }
    }

    void RecogerObjeto(RaycastHit hit)
    {
        foreach (var item in objetos)
        {
            if (hit.collider != null && hit.collider.CompareTag(item.tagObjeto) && !item.tiene)
            {
                item.tiene = true;
                if (item.iconoUI != null) item.iconoUI.SetActive(true);
                Destroy(hit.collider.gameObject);
                Debug.Log("Recogido: " + item.tagObjeto);
                return;
            }
        }
    }

    void DropearObjetoSeleccionado()
    {
        var item = objetos[indiceSeleccionado];
        if (item.tiene && item.prefab != null && puntoDrop != null)
        {
            Vector3 dropPos = puntoDrop.position + puntoDrop.forward * 1.5f;
            dropPos.y += 0.5f;

            GameObject nuevoObjeto = Instantiate(item.prefab, dropPos, Quaternion.identity);
            nuevoObjeto.tag = item.tagObjeto;

            if (nuevoObjeto.GetComponent<Rigidbody>() == null)
                nuevoObjeto.AddComponent<Rigidbody>();

            if (item.iconoUI != null) item.iconoUI.SetActive(false);
            item.tiene = false;
            Debug.Log("Dropped: " + item.tagObjeto);
        }
    }

    void ReiniciarSlider()
    {
        progresoRecogida = 0f;
        if (recogerSlider != null)
        {
            recogerSlider.value = 0f;
            recogerSlider.gameObject.SetActive(false);
        }
    }
}
