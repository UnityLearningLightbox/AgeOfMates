using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CompassScript : MonoBehaviour
{
    [Header("Quest Markers")]
    // Este será el prefab creado IconQuest.
    // Si se modifica el prefab o se cambia, DEBERA ser una imagen, es decir, hay que crear dentro del canvas una imagen vacia, colocarla donde se quiera y hacerla prefab
    // Luego se añade ese prefab al script en el inspector. Esto es lo que se vera en la brujula
    [SerializeField] GameObject questIconPrefab; 

    [SerializeField] CompassQuestMarker[] questsArray;
    [SerializeField] float maxDistance = 200f;

    List<CompassQuestMarker> questMarkers = new List<CompassQuestMarker>(); // Lista de elementos que tendran un icono de quest y saldran en la brujula
    float compassUnit; // Para mostrar con precision los iconos en la brujula

    [Header("Compass Settings")]
    [SerializeField] TextMeshProUGUI compassText;
    [SerializeField] Transform playerCamera;
    [SerializeField] int showedChars = 20; // Caracteres qe se muestran
    [SerializeField] int betweenCharDistance = 10;

    [SerializeField] RawImage compassImage; // Corresponde a la imagen de la linea, la que esta encima de las coordenadas

    [SerializeField]
    string[] coordinates = new string[]
    {
        //"N", "NE", "E", "SE", "S", "SW", "W", "NW"
        "N", "15", "30", "NE", "60", "75", "E", "105", "120", "SE", "150", "165", "S", "195", "210", "SW", "240", "255", "W", "285", "300", "NW", "330", "345"
    };
    [SerializeField] int virtualCompassWidth = 200;
    private string compassString;

    private void Start()
    {
        InitialSettings();
    }

    private void Update()
    {
        UpdateCompassInfo();
        UpdateMarkerInfo();

        compassImage.uvRect = new Rect(playerCamera.eulerAngles.y / 360f, 0f, 1f, 1f); // Para que la imagen de la linea gire en X junto con la camara, 360º
    }

    void InitialSettings()
    {
        compassString = "";

        while (compassString.Length < virtualCompassWidth)
        {
            foreach (string coord in coordinates)
            {
                compassString += coord.PadRight(betweenCharDistance);
            }
        }

        compassUnit = compassImage.rectTransform.rect.width / 360f; // compassUnit = ancho de la imagen de la brujula / 360
        // Nos da una unidad respecto al tamaño de la pantalla equivalente a un grado de rotacion en el mundo real
        // Osea se, si el jugador se gira un grado, eso equivale a una cierta cantidad de tamaño de pantalla sobre la brujula

        if(questsArray.Length > 0)
        {
            AddQuestObjective();
        }
    }

    void UpdateCompassInfo()
    {
        float yRotation = playerCamera.eulerAngles.y;

        int indexCoordinates = Mathf.RoundToInt((yRotation / 360f) * compassString.Length);

        int startIndex = indexCoordinates - showedChars / 2;

        if (startIndex < 0) startIndex += compassString.Length;

        if ((startIndex + betweenCharDistance) > compassString.Length)
        {
            showedChars = 60;
            betweenCharDistance = 60;

            string part1 = compassString.Substring(startIndex); // Compruebas longitud y hasta donde tienes caracteres
            string part2 = compassString.Substring(0, (startIndex + showedChars) % compassString.Length);// Concatenas caracteres si te pasas

            compassText.text = part1 + part2;
        }
        else
        {
            compassText.text = compassString.Substring(startIndex, showedChars);

        }
    }

    void UpdateMarkerInfo()
    {
        foreach (CompassQuestMarker marker in questMarkers)
        {
            marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);

            float dst = Vector2.Distance(new Vector2(playerCamera.transform.position.x, playerCamera.transform.position.z), marker.position); // distancia del jugador al objeto que tiene el icon quest
            float scale = 0f; // 0f es el tamaño por defecto del icono, su 100%

            if (dst < maxDistance) // si se aleja, la escala disminuye y se hace mas pequeño
            {
                scale = 1f - (dst / maxDistance);
            }

            marker.image.rectTransform.localScale = Vector3.one * scale;
        }
    }

    void AddQuestMarker(CompassQuestMarker marker)
    {
        GameObject newMarker = Instantiate(questIconPrefab, compassImage.transform); // Esto hará que el prefab automaticamente sea un hijo de compassImage
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        questMarkers.Add(marker);
    }
    
    void AddQuestObjective()
    {
        foreach (CompassQuestMarker quest in questsArray)
        {
            AddQuestMarker(quest);
        }
    }

    Vector2 GetPosOnCompass(CompassQuestMarker marker) // Para obtener la posicion (en vector2) del icono en la brujula
    {
        Vector2 playerPos = new Vector2(playerCamera.transform.position.x, playerCamera.transform.position.z);
        Vector2 playerFwd = new Vector2(playerCamera.transform.forward.x, playerCamera.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd); // posicion del marker - posicion del jugador, da la direccion desde el jugdor hacia el icono
        // playerFwd es la dirección a la que el jugador mira y SignedAngle es el angulo entre esas dos direcciones

        return new Vector2(compassUnit * angle, 0f);
    }
}
