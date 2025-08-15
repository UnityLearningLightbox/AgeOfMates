using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [System.Serializable]
    public class ItemData
    {
        public string tagObjeto;      // Ejemplo: "Objeto1"
        public GameObject iconoUI;    // Icono en la UI
        public GameObject prefab;     // Prefab para soltar
        public bool tiene;            // Si el jugador lo posee
    }

    [Header("Objetos del Inventario")]
    public List<ItemData> objetos = new List<ItemData>();

    [Header("Drop")]
    public Transform puntoDrop;

    [Header("Raycast")]
    public float distanciaRecogida = 3f;
    public LayerMask capaObjetos;

    private int indiceSeleccionado = 0; // Para elegir qué dropear

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            IntentarRecoger();

        if (Input.GetKeyDown(KeyCode.Q))
            DropearObjetoSeleccionado();

        // Cambiar selección con la rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            indiceSeleccionado = (indiceSeleccionado + (scroll > 0 ? 1 : -1) + objetos.Count) % objetos.Count;
            Debug.Log("Objeto seleccionado: " + objetos[indiceSeleccionado].tagObjeto);
        }
    }

    void IntentarRecoger()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaRecogida, capaObjetos))
        {
            foreach (var item in objetos)
            {
                if (hit.collider.CompareTag(item.tagObjeto) && !item.tiene)
                {
                    item.tiene = true;
                    if (item.iconoUI != null) item.iconoUI.SetActive(true);
                    Destroy(hit.collider.gameObject);
                    Debug.Log("Recogido: " + item.tagObjeto);
                    return;
                }
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

            //  Asignar la tag correcta
            nuevoObjeto.tag = item.tagObjeto;

            if (nuevoObjeto.GetComponent<Rigidbody>() == null)
                nuevoObjeto.AddComponent<Rigidbody>();

            if (item.iconoUI != null) item.iconoUI.SetActive(false);
            item.tiene = false;
            Debug.Log("Dropped: " + item.tagObjeto);
        }
    }


    public bool TieneObjeto(string tagObjeto)
    {
        foreach (var item in objetos)
        {
            if (item.tagObjeto == tagObjeto && item.tiene)
                return true;
        }
        return false;
    }

    public void EliminarObjeto(string tagObjeto)
    {
        foreach (var item in objetos)
        {
            if (item.tagObjeto == tagObjeto && item.tiene)
            {
                item.tiene = false;
                if (item.iconoUI != null) item.iconoUI.SetActive(false);
                return;
            }
        }
    }
}
