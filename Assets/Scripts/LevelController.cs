using System.Collections;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] Transform startPosition;   // posición inicial para la primera vez
    [SerializeField] Transform playerPosition;  // referencia al transform del jugador

    [Header("Canvas")]
    [SerializeField] GameObject startCanvas;
    [SerializeField] GameObject compassCanvas;
    [SerializeField] GameObject startCanvasParent;
    [SerializeField] GameObject timeline;

    [Header("Player Controller")]
    [SerializeField] TademiusController playerController;

    [Header("Cinematic State")]
    public bool cinematicPlayed; // se guarda/carga desde PauseMenu

    private Coroutine cinematicRoutine;

    /// <summary>
    /// Llamado desde PauseMenu después de LoadGame()
    /// Decide si reproducir o saltar la intro
    /// </summary>
    public void InitCinematic()
    {
        if (!cinematicPlayed)
        {
            Debug.Log("NO se ha ejecutado la cinemática, iniciando...");

            if (startCanvas != null) startCanvas.SetActive(true);
            if (compassCanvas != null) compassCanvas.SetActive(false);
            if (playerController != null) playerController.enabled = false;

            // Solo mover al jugador la primera vez
            if (playerPosition != null && startPosition != null)
                playerPosition.position = startPosition.position;

            cinematicRoutine = StartCoroutine(IntroScene());
        }
        else
        {
            Debug.Log("Cinemática ya ejecutada previamente, saltando intro.");

            if (startCanvasParent != null) Destroy(startCanvasParent);
            if (timeline != null) Destroy(timeline);

            if (startCanvas != null) startCanvas.SetActive(false);
            if (compassCanvas != null) compassCanvas.SetActive(true);
            if (playerController != null) playerController.enabled = true;

            // Aquí NO tocamos la posición del jugador,
            // porque ya la restauró PauseMenu.LoadGame()
        }
    }

    private IEnumerator IntroScene()
    {
        yield return new WaitForSeconds(3f);
        if (startCanvas != null) startCanvas.SetActive(true);

        yield return new WaitForSeconds(7f);
        if (startCanvas != null) startCanvas.SetActive(false);
        if (playerController != null) playerController.enabled = true;

        yield return new WaitForSeconds(2f);
        if (compassCanvas != null) compassCanvas.SetActive(true);

        cinematicPlayed = true; // marcar como vista
    }
}

