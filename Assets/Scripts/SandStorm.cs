using System.Collections;
using UnityEngine;

public class SandStorm : MonoBehaviour
{
    [SerializeField] Transform teleportPosition; // El prefab SandStorm tiene una position para teletransportar al player
    //[SerializeField] PlayerController playerController;
    [SerializeField] TademiusController playerController;
    [SerializeField] Transform playerPosition;

    [SerializeField] GameObject warningCanvas;
    [SerializeField] Animator warningCanvasAnimation;

    [SerializeField] GameObject sandStormToDestroy;
    public bool minigameCompleted; // Este sera el booleano que se mandara al minijuego en cuestion cuando se complete.

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
            StartCoroutine(FaintingPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.playerSpeed *= 2;
            playerController.runningSpeed *= 2;
        }
    }

    IEnumerator FaintingPlayer()
    {
        playerController.playerSpeed /= 2;
        playerController.runningSpeed /= 2;

        warningCanvas.SetActive(true);
        yield return new WaitForSeconds(8f);
        warningCanvasAnimation.SetBool("isFading", true);

        yield return new WaitForSeconds(3f);
        Debug.Log("El player se desmaya.");

        warningCanvas.SetActive(false);
        playerPosition.position = teleportPosition.position;
    }
}
