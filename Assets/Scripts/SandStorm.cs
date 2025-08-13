using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SandStorm : MonoBehaviour
{
    [SerializeField] Transform teleportPosition; // El prefab SandStorm tiene una position para teletransportar al player
    [SerializeField] PlayerController playerController;
    //[SerializeField] TademiusController playerController;
    [SerializeField] Transform playerPosition;

    [SerializeField] GameObject warningCanvas;

    [SerializeField] GameObject sandStormToDestroy;
    [SerializeField] bool minigameCompleted; // Este sera el booleano que se reciba del minijuego en cuestion cuando se complete. En vez de ser bool sera del script correspondiente

    private void Start()
    {
        InitalSettings();
    }

    private void Update()
    {
        if(minigameCompleted == true && sandStormToDestroy != null)
        {
            Debug.Log("Minujuego completado");
            sandStormToDestroy.SetActive(false);
        }
    }

    void InitalSettings()
    {
        if (warningCanvas != null)
        {
            warningCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("El player entra en el collider?");

        if (other.CompareTag("Player"))
        {
            //playerController.playerSpeed /= 2;
            //playerController.runningSpeed /= 2;

            StartCoroutine(FaintingPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //playerController.playerSpeed *= 2;
            //playerController.runningSpeed *= 2;
        }
    }

    IEnumerator FaintingPlayer()
    {
        warningCanvas.SetActive(true);
        playerController.playerSpeed /= 2;
        playerController.runningSpeed /= 2;

        yield return new WaitForSeconds(10f);

        Debug.Log("El player se desmaya.");
        warningCanvas.SetActive(false);
        playerPosition.position = teleportPosition.position;
    }
}
