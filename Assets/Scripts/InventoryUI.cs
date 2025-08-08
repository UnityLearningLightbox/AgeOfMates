using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Imágenes en el Canvas (orden: Objeto1, Objeto2, Objeto3, Objeto4)")]
    public Image[] imagenesObjetos;

    [Header("Sprites para cada objeto")]
    public Sprite[] spritesObjetos; // 0 = Objeto1, etc.

    [Header("Colores")]
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = Color.yellow;

    private bool[] objetosConseguidos = new bool[4];
    private int indiceSeleccionado = 0;

    [Header("Tag del objeto cercano para probar la recogida")]
    public string objetoCercanoTag = ""; // Pon aquí "Objeto1", "Objeto2", etc. para simular

    void Start()
    {
        for (int i = 0; i < imagenesObjetos.Length; i++)
            imagenesObjetos[i].gameObject.SetActive(false);

        ActualizarSeleccionVisual();
    }

    void Update()
    {
        // Cambiar selección con rueda del ratón
        float rueda = Input.GetAxis("Mouse ScrollWheel");
        if (rueda > 0f)
            CambiarSeleccion(-1);
        else if (rueda < 0f)
            CambiarSeleccion(1);

        // Detectar pulsación E para recoger objeto cercano (simulado)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!string.IsNullOrEmpty(objetoCercanoTag))
            {
                RecogerObjeto(objetoCercanoTag);
                // Para test, borra el tag para no añadirlo varias veces
                objetoCercanoTag = "";
            }
            else
            {
                Debug.Log("No hay objeto cercano para recoger");
            }
        }
    }

    private void CambiarSeleccion(int direccion)
    {
        int anterior = indiceSeleccionado;
        do
        {
            indiceSeleccionado = (indiceSeleccionado + direccion + objetosConseguidos.Length) % objetosConseguidos.Length;
        }
        while (!objetosConseguidos[indiceSeleccionado] && indiceSeleccionado != anterior);

        ActualizarSeleccionVisual();
    }

    private void ActualizarSeleccionVisual()
    {
        for (int i = 0; i < imagenesObjetos.Length; i++)
        {
            if (imagenesObjetos[i].gameObject.activeSelf)
                imagenesObjetos[i].color = (i == indiceSeleccionado) ? colorSeleccionado : colorNormal;
        }
    }

    public void RecogerObjeto(string tag)
    {
        int index = -1;
        if (tag == "Objeto1") index = 0;
        else if (tag == "Objeto2") index = 1;
        else if (tag == "Objeto3") index = 2;
        else if (tag == "Objeto4") index = 3;

        if (index != -1 && !objetosConseguidos[index])
        {
            objetosConseguidos[index] = true;
            imagenesObjetos[index].sprite = spritesObjetos[index];
            imagenesObjetos[index].gameObject.SetActive(true);
            indiceSeleccionado = index;
            ActualizarSeleccionVisual();

            Debug.Log($"Objeto {tag} añadido al inventario");
        }
        else if (index != -1 && objetosConseguidos[index])
        {
            Debug.Log($"Objeto {tag} ya está en el inventario");
        }
        else
        {
            Debug.Log($"Tag {tag} inválido");
        }
    }

    public int GetObjetoSeleccionado()
    {
        return indiceSeleccionado;
    }
}