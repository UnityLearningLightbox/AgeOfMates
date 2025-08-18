using System.Collections;
using TMPro;
using UnityEngine;

public class LabyrinthCompleted : MonoBehaviour
{
    [SerializeField] LabyrinthMinigame startCollider;
    [SerializeField] SandStorm storm; // Esta sera la tormenta que desaparecerá al completar el minijuego. Concretamente el muro invisible que es el que tiene el script
    [SerializeField] Canvas timerCanvas;
    public bool minigameCompleted;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Minijuego completado");
            startCollider.startingTimer = false;
            minigameCompleted = true;

            if(storm != null)
            {
                storm.minigameCompleted = minigameCompleted;
            }

            StartCoroutine(TurnOffCanvas());
        }
    }

    IEnumerator TurnOffCanvas()
    {
        yield return new WaitForSeconds(1f);
        timerCanvas.enabled = false;
    }
}
