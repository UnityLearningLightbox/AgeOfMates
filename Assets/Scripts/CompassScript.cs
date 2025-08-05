using UnityEngine;
using TMPro;

public class CompassScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI compassText;
    [SerializeField] Transform playerCamera;
    [SerializeField] int showedChars = 20; // Caracteres qe se muestran

    [SerializeField] int betweenCharDistance = 10;

    [SerializeField]
    string[] coordinates = new string[]
    {
        "N", "NE", "E", "SE", "S", "SW", "W", "NW"
        //"N", "15", "30", "NE", "60", "75", "E", "105", "120", "SE", "150", "165", "S", "195", "210", "SW", "240", "255", "W", "285", "300", "NW", "330", "345"
    };

    //private const int screenWidth = 80;
    [SerializeField] int virtualCompassWidth = 200;
    private string compassString;

    private void Start()
    {
        InitialSettings();
    }

    private void Update()
    {
        UpdateCompassInformation();
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

        //for (int i = 0; i < 5; i++)
        //{
        //    foreach(string coord in coordinates)
        //    {
        //        compassString += coord.PadRight(betweenCharDistance);
        //    }
        //}
    }

    void UpdateCompassInformation()
    {
        float yRotation = playerCamera.eulerAngles.y;

        int indexCoordinates = Mathf.RoundToInt((yRotation / 360f) * compassString.Length); // Revisar resolucion de pantalla

        int startIndex = indexCoordinates - showedChars / 2; // Revisar el "/2"

        if (startIndex < 0) startIndex += compassString.Length;

        if (startIndex + betweenCharDistance > compassString.Length)
        {
            string part1 = compassString.Substring(startIndex); // Compruebas longitud y hasta donde tienes caracteres
            string part2 = compassString.Substring(0, (startIndex + showedChars) % compassString.Length);// Concatenas caracteres si te pasas

            compassText.text = part1 + part2;

        }
        else
        {
            compassText.text = compassString.Substring(startIndex, showedChars);
        }
    }
}
