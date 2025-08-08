using UnityEngine;

public class SetActiveOnTrigger : MonoBehaviour
{
    [SerializeField] GameObject gameObjectToDesactive;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObjectToDesactive.SetActive(false);
            
        }
    }
}
