using System.Collections;
using UnityEngine;

public class SandStorm : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] Transform teleportPosition; // El prefab SandStorm tiene una position para teletransportar al player
    //[SerializeField] PlayerController playerController;
    [SerializeField] TademiusController playerController;
    [SerializeField] Transform playerPosition;

    [Header("Warning Settings")]
    [SerializeField] GameObject warningCanvas;
    [SerializeField] Animator warningCanvasAnimation;

    [Header("Minigame completed")]
    [SerializeField] GameObject sandStormToDestroy; // Será si mismo
    public bool minigameCompleted; // Este sera el booleano que se mandara al minijuego en cuestion cuando se complete.

    [Header("Player fainting")]
    [SerializeField] GameObject faintCanvas;
    [SerializeField] Animator faintAnimator;

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
        faintCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
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

            StartCoroutine(WakingUpPlayer());
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
        faintCanvas.SetActive(true);

        yield return new WaitForSeconds(2f);

        warningCanvas.SetActive(false);
        playerPosition.position = teleportPosition.position;
    }

    IEnumerator WakingUpPlayer()
    {
        faintAnimator.SetBool("isFainting", true);
        yield return new WaitForSeconds(3f);
        faintCanvas.SetActive(false);
    }
}
