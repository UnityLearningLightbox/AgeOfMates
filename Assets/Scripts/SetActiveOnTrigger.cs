using UnityEngine;

public class SetActiveOnTrigger : MonoBehaviour
{
    [SerializeField] GameObject desactiveDialogueCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            desactiveDialogueCanvas.SetActive(true);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            desactiveDialogueCanvas.SetActive(false);
        }
    }
}
