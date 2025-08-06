using UnityEngine;
using UnityEngine.UI;

public class CompassQuestMarker : MonoBehaviour
{
    public Sprite icon; // Icono que se verá en la brujula
    public Image image; // A esta variable se le da uso en el CompassScript, no tiene que recibir nada en el inspector

    public Vector2 position // Posición del objeto que tendrá el icono
    {
        get {  return new Vector2(transform.position.x, transform.position.z); }
    }
}
